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
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {
   
   static class RouteFactory {

      static readonly Regex TokenPattern = new Regex(@"\{(.+?)\}");

      public static Route[] CreateRoutes(RegisterInfo registerInfo) {

         ActionInfo[] actions = registerInfo.GetControllers()
            .SelectMany(c => c.Actions)
            .ToArray();

         CheckNoAmbiguousUrls(actions);

         var groupedActions = GroupActions(actions);

         CodeRoute[] codeRoutes = groupedActions.Select(g => CreateRoute(g)).ToArray();

         object config = registerInfo.Settings.Configuration;

         if (config != null) {
            for (int i = 0; i < codeRoutes.Length; i++) 
               codeRoutes[i].DataTokens[DataTokenKeys.Configuration] = config;
         }

         return codeRoutes;
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

                              foreach (var param in first.RouteParameters.Skip(last.RouteParameters.Count))
                                 param.IsOptional = true;
                           }

                           finalGrouping.Add(range);

                           foreach (var item in range)
                              ordered.Remove(item);
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

      static CodeRoute CreateRoute(IEnumerable<ActionInfo> actions) {

         if (actions == null) throw new ArgumentNullException("actions");

         ActionInfo first = actions.First();
         string baseRoute = first.Controller.Register.BaseRoute;

         var controllerMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         foreach (string name in actions.Select(a => a.Controller.Name).Distinct(controllerMapping.Comparer))
            controllerMapping.Add(name, actions.First(a => ControllerInfo.NameEquals(a.Controller.Name, name)).Controller.ControllerSegment);

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

         foreach (string name in actions.Select(a => a.Name).Distinct(actionMapping.Comparer))
            actionMapping.Add(name, actions.First(a => ActionInfo.NameEquals(a.Name, name)).ActionSegment);

         string actionCustomRoute = first.CustomRoute;

         bool includeActionToken = (actionCustomRoute != null) ?
            first.CustomRouteHasActionToken
            : actionMapping.Count > 1 || first.IsDefaultAction;

         bool actionFormat = actionMapping.Any(p => !String.Equals(p.Key, p.Value, StringComparison.Ordinal));
         bool requiresActionMapping = actionFormat && includeActionToken;

         if (actionCustomRoute != null) {

            if (actionCustomRoute.StartsWith("~/")) {
               actionCustomRoute = actionCustomRoute.Substring(2);

               segments.Clear();

               if (baseRoute != null)
                  segments.AddRange(baseRoute.Split('/'));
            }

            segments.Add(actionCustomRoute);
         } else {
            segments.Add(!includeActionToken ? first.ActionSegment : "{action}");
            segments.AddRange(first.RouteParameters.Select(r => r.RouteSegment));
         }

         string url = String.Join("/", segments.Where(s => !String.IsNullOrEmpty(s)));

         var defaults = new RouteValueDictionary();

         if (!includeControllerToken)
            defaults.Add("controller", controllerMapping.Keys.First());

         ActionInfo defaultAction = (includeActionToken) ?
            actions.FirstOrDefault(a => a.IsDefaultAction)
            : first;

         string actionDefault = null;

         if (defaultAction != null) {
            actionDefault = (requiresActionMapping) ?
               defaultAction.ActionSegment
               : defaultAction.Name;
         }

         if (actionDefault != null)
            defaults.Add("action", actionDefault);

         TokenInfoCollection parameters = first.RouteParameters;

         foreach (var param in parameters.Where(p => p.IsOptional))
            defaults.Add(param.Name, UrlParameter.Optional);

         var constraints = new RouteValueDictionary();

         if (includeControllerToken)
            constraints.Add("controller", String.Join("|", controllerMapping.Values));

         if (includeActionToken)
            constraints.Add("action", String.Join("|", actionMapping.Values));

         foreach (var param in first.Controller.RouteProperties.Concat(parameters).Where(p => p.Constraint != null)) {

            string regex = param.Constraint;

            if (param.IsOptional)
               regex = String.Concat("(", regex, ")?");

            constraints.Add(param.Name, regex);
         }

         constraints.Add(CodeRoutingConstraint.Key, new CodeRoutingConstraint());

         var dataTokens = new RouteValueDictionary { 
            { DataTokenKeys.Namespaces, new string[1] { first.Controller.Namespace } },
            { DataTokenKeys.BaseRoute, baseRoute },
            { DataTokenKeys.RouteContext, String.Join("/", first.Controller.CodeRoutingContext) },
            { DataTokenKeys.ViewsLocation, String.Join("/", first.Controller.CodeRoutingContext.Where(s => !s.Contains('{'))) }
         };

         var nonActionParameterTokens = new List<string>();

         if (baseRoute != null)
            nonActionParameterTokens.AddRange(TokenPattern.Matches(baseRoute).Cast<Match>().Select(m => m.Groups[1].Value));

         nonActionParameterTokens.AddRange(first.Controller.RouteProperties.Select(p => p.Name));

         return new CodeRoute(
            url: url,
            controllerMapping: (requiresControllerMapping) ? controllerMapping : null,
            actionMapping: (requiresActionMapping) ? actionMapping : null,
            nonActionParameterTokens: nonActionParameterTokens.ToArray()) {
               Constraints = constraints,
               DataTokens = dataTokens,
               Defaults = defaults
            };
      }
   }
}
