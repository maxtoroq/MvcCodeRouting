// Copyright 2012 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MvcCodeRouting.Controllers;
using MvcCodeRouting.ParameterBinding;
using MvcCodeRouting.Web.Routing;

namespace MvcCodeRouting {
   
   abstract class RouteFactory {

      public abstract object OptionalParameterValue { get; }

      public static TRoute[] CreateRoutes<TRoute>(RegisterSettings registerSettings) where TRoute : class {

         ActionInfo[] actions = registerSettings.GetControllers()
            .SelectMany(c => c.Actions)
            .ToArray();

         CheckNoAmbiguousUrls(actions);

         var groupedActions = GroupActions(actions);
         object config = registerSettings.Settings.Configuration;

         var routes = new List<TRoute>();

         foreach (var group in groupedActions) {

            ControllerInfo controller = group.First().Controller;
            RouteFactory routeFactory = controller.Provider.RouteFactory;

            RouteSettings routeSettings = routeFactory.CreateRouteSettings(group);

            if (config != null) {
               routeSettings.DataTokens[DataTokenKeys.Configuration] = config;
            }

            object route = routeFactory.CreateRoute(routeSettings, registerSettings);

            if (route is TRoute) {
               routes.Add((TRoute)route);

            } else {

               TRoute convertedRoute = routeFactory.ConvertRoute(route, typeof(TRoute), registerSettings) as TRoute;

               if (convertedRoute != null) {
                  routes.Add(convertedRoute);
               }
               // TODO: else, throw exception?
            }
         }

         return routes.ToArray();
      }

      static void CheckNoAmbiguousUrls(IEnumerable<ActionInfo> actions) {

         var ambiguousController =
            (from a in actions
             where a.CustomRoute == null
             group a by a.ActionUrl into g
             where g.Count() > 1
             let distinctControllers = g.Select(a => a.Controller).Distinct().ToArray()
             where distinctControllers.Length > 1
             select new {
                ActionUrl = g.Key,
                DistinctControllers = distinctControllers
             }).ToList();

         if (ambiguousController.Count > 0) {

            var first = ambiguousController.First();

            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture,
                  "The URL '{0}' cannot be bound to more than one controller ({1}).",
                  first.ActionUrl,
                  String.Join(", ", first.DistinctControllers.Select(c => c.Type.FullName))
               )
            );
         }
      }

      static IEnumerable<IEnumerable<ActionInfo>> GroupActions(IEnumerable<ActionInfo> actions) {

         var groupedActions =
            (from a in actions
             let declaringType1 = a.DeclaringType
             let declaringType = (declaringType1.IsGenericType) ?
                declaringType1.GetGenericTypeDefinition()
                : declaringType1
             group a by new {
                Depth = a.Controller.CodeRoutingNamespace.Count,
                a.Controller.IsRootController,
                a.Controller.Namespace,
                NamespaceSegments = String.Join("/", a.Controller.NamespaceSegments),
                ControllerCustomRoute = a.Controller.CustomRoute,
                DeclaringType = declaringType,
                a.CustomRoute,
                HasRouteParameters = (a.RouteParameters.Count > 0)
             } into g
             orderby g.Key.IsRootController descending,
                g.Key.Depth,
                g.Key.Namespace,
                g.Key.HasRouteParameters descending
             select g
             ).ToList();

         var signatureComparer = new ActionSignatureComparer();
         var finalGrouping = new List<IEnumerable<ActionInfo>>();

         for (int i = 0; i < groupedActions.Count; i++) {

            var set = groupedActions[i];

            if (set.Key.HasRouteParameters) {

               var ordered = set.OrderByDescending(a => a.RouteParameters.Count).ToList();

               while (ordered.Count > 0) {
                  var firstInSet = ordered.First();
                  var similar = ordered.Skip(1).Where(a => signatureComparer.Equals(firstInSet, a)).ToList();

                  if (similar.Count > 0) {

                     var signatureCompat = new[] { firstInSet }.Concat(similar).ToArray();

                     var maxParamCounts =
                        (from a in signatureCompat
                         group a by a.ActionSegment into g
                         select g.Select(a => a.RouteParameters.Count).Max()
                        ).Distinct().ToArray();

                     foreach (var count in maxParamCounts) {

                        var sameMaxNumberOfParams =
                           (from a in signatureCompat
                            group a by a.ActionSegment into g
                            where g.Select(a => a.RouteParameters.Count).Max() == count
                            select g)
                           .SelectMany(g => g)
                           .Distinct()
                           .OrderByDescending(a => a.RouteParameters.Count)
                           .ToArray();

                        var index = 0;
                        var k = 0;
                        var overloadRanges =
                           (from a in sameMaxNumberOfParams
                            let idx = ++index
                            let next = sameMaxNumberOfParams.ElementAtOrDefault(idx)
                            let diff = (next == null) ? 0 : Math.Abs(a.RouteParameters.Count - next.RouteParameters.Count)
                            let key = (diff == 1 || diff == 0) ?
                            k : k++
                            group a by key into g
                            select g).ToArray();

                        foreach (var range in overloadRanges) {

                           if (range.Count() > 1) {

                              var first = range.First();
                              var last = range.Last();

                              foreach (var param in first.RouteParameters.Skip(last.RouteParameters.Count)) {
                                 param.IsOptional = true;
                              }
                           }

                           finalGrouping.Add(range);

                           foreach (var item in range) {
                              ordered.Remove(item);
                           }
                        }
                     }

                  } else {
                     finalGrouping.Add(new[] { firstInSet });
                     ordered.Remove(firstInSet);
                  }
               }

            } else {

               finalGrouping.Add(set);
            }
         }

         return finalGrouping;
      }

      RouteSettings CreateRouteSettings(IEnumerable<ActionInfo> actions) {

         if (actions == null) throw new ArgumentNullException("actions");

         ActionInfo first = actions.First();
         string baseRoute = first.Controller.Register.BaseRoute;

         var controllerMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         foreach (string name in actions.Select(a => a.Controller.Name).Distinct(controllerMapping.Comparer)) {
            controllerMapping.Add(name, actions.First(a => ControllerInfo.NameEquals(a.Controller.Name, name)).Controller.ControllerSegment);
         }

         string controllerCustomRoute = first.Controller.CustomRoute;

         bool includeControllerToken = (controllerCustomRoute != null) ?
            first.Controller.CustomRouteHasControllerToken
            : controllerMapping.Count > 1;

         bool controllerFormat = controllerMapping.Any(p => !String.Equals(p.Key, p.Value, StringComparison.Ordinal));
         bool requiresControllerMapping = controllerFormat && includeControllerToken;

         var segments = new List<string> { 
            (includeControllerToken ? 
               first.Controller.UrlTemplate
               : first.Controller.ControllerUrl 
            )
         };

         var actionMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         foreach (string name in actions.Select(a => a.Name).Distinct(actionMapping.Comparer)) {
            actionMapping.Add(name, actions.First(a => ActionInfo.NameEquals(a.Name, name)).ActionSegment);
         }

         string actionCustomRoute = first.CustomRoute;
         bool allEmptyActionSegment = actionMapping.Values.All(s => s.Length == 0);
         bool hasEmptyActionSegment = allEmptyActionSegment || actionMapping.Values.Any(s => s.Length == 0);

         bool includeActionToken = (actionCustomRoute != null) ?
            first.CustomRouteHasActionToken
            : actionMapping.Count > 1 || first.IsDefaultAction;

         bool actionFormat = actionMapping.Any(p => !String.Equals(p.Key, p.Value, StringComparison.Ordinal));
         bool requiresActionMapping = actionFormat && includeActionToken;
         
         if (actionCustomRoute != null) {

            if (first.CustomRouteIsAbsolute) {

               actionCustomRoute = actionCustomRoute.Substring(2);

               segments.Clear();

               if (baseRoute != null) {
                  segments.AddRange(baseRoute.Split('/'));
               }
            }

            segments.Add(actionCustomRoute);

         } else {
            segments.Add(!includeActionToken ? first.ActionSegment : "{action}");
            segments.AddRange(first.RouteParameters.Select(r => r.RouteSegment));
         }

         string url = String.Join("/", segments.Where(s => !String.IsNullOrEmpty(s)));

         var routeSettings = new RouteSettings(url, actions) {
            ActionMapping = (requiresActionMapping) ? actionMapping : null,
            ControllerMapping = (requiresControllerMapping) ? controllerMapping : null,
         };

         if (!includeControllerToken) {
            routeSettings.Defaults.Add("controller", controllerMapping.Keys.First());
         }

         ActionInfo defaultAction = (includeActionToken) ?
            actions.FirstOrDefault(a => a.IsDefaultAction)
            : first;

         string actionDefault = null;

         if (defaultAction != null) {
            
            actionDefault = (requiresActionMapping) ?
               defaultAction.ActionSegment
               : defaultAction.Name;
         
         } else if (hasEmptyActionSegment && !allEmptyActionSegment) {
            
            actionDefault = "";
         }
         
         if (actionDefault != null) {

            object actionDef = (actionDefault.Length == 0) ?
               (!allEmptyActionSegment) ? this.OptionalParameterValue
               : null
               : actionDefault;

            if (actionDef != null) {
               routeSettings.Defaults.Add("action", actionDef);
            }
         }

         RouteParameterCollection parameters = first.RouteParameters;

         foreach (var param in parameters.Where(p => p.IsOptional)) {
            routeSettings.Defaults.Add(param.Name, this.OptionalParameterValue);
         }

         if (includeControllerToken) {
            routeSettings.Constraints.Add("controller", first.Controller.Provider.CreateSetRouteConstraint(controllerMapping.Values.ToArray()));
         }

         if (includeActionToken) {
            routeSettings.Constraints.Add("action", first.Controller.Provider.CreateSetRouteConstraint(actionMapping.Values.Where(s => !String.IsNullOrEmpty(s)).ToArray()));
         }

         var binders = new Dictionary<string, ParameterBinder>(StringComparer.OrdinalIgnoreCase);

         foreach (var param in first.Controller.RouteProperties.Concat(parameters).Where(p => p.Constraint != null || p.Binder != null)) {

            object routeConstraint;

            if (param.Constraint != null) {
               routeConstraint = first.Controller.Provider.CreateRegexRouteConstraint(param.Constraint, param.ParameterType);
            
            } else {
               routeConstraint = first.Controller.Provider.CreateParameterBindingRouteConstraint(param.Binder);
            }

            if (param.Binder != null) {
               binders[param.Name] = param.Binder;
            }

            routeSettings.Constraints.Add(param.Name, routeConstraint);
         }

         routeSettings.DataTokens[DataTokenKeys.Namespaces] = new string[1] { first.Controller.Namespace };
         routeSettings.DataTokens[DataTokenKeys.BaseRoute] = baseRoute;
         routeSettings.DataTokens[DataTokenKeys.RouteContext] = String.Join("/", first.Controller.CodeRoutingContext);
         routeSettings.DataTokens[DataTokenKeys.ViewsLocation] = String.Join("/", first.Controller.CodeRoutingContext.Where(s => !s.Contains('{')));
         routeSettings.DataTokens[DataTokenKeys.ParameterBinders] = binders;

         return routeSettings;
      }

      public abstract object CreateRoute(RouteSettings routeSettings, RegisterSettings registerSettings);

      public virtual object ConvertRoute(object route, Type conversionType, RegisterSettings registerSettings) {
         return null;
      }
   }
}
