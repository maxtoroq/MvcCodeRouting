// Copyright 2011 Max Toro Q.
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
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {

   public static class CodeRoutingExtensions {

      static readonly List<ActionInfo> registeredActions = new List<ActionInfo>();

      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, string rootNamespace) {
         return MapCodeRoutes(routes, Assembly.GetCallingAssembly(), rootNamespace);
      }

      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, string rootNamespace, CodeRoutingSettings settings) {
         return MapCodeRoutes(routes, Assembly.GetCallingAssembly(), rootNamespace, settings);
      }

      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, Assembly assembly, string rootNamespace) {
         return MapCodeRoutes(routes, assembly, rootNamespace, (CodeRoutingSettings)null);
      }

      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, Assembly assembly, string rootNamespace, CodeRoutingSettings settings) {
         return MapCodeRoutes(routes, assembly, rootNamespace, null, settings);
      }

      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, Assembly assembly, string rootNamespace, string baseRoute) {
         return MapCodeRoutes(routes, assembly, rootNamespace, baseRoute, (CodeRoutingSettings)null);
      }

      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, Assembly assembly, string rootNamespace, string baseRoute, CodeRoutingSettings settings) {

         if (routes == null) throw new ArgumentNullException("routes");
         if (assembly == null) throw new ArgumentNullException("assembly");
         if (rootNamespace == null) throw new ArgumentNullException("rootNamespace");

         if (settings == null)
            settings = new CodeRoutingSettings();

         var actions = ControllerInfo.GetControllers(assembly, rootNamespace, baseRoute, settings)
            .SelectMany(c => c.GetActions());

         registeredActions.AddRange(actions);

         CheckSingleRootController(registeredActions);
         CheckNoAmbiguousUrls(registeredActions);

         var groupedActions = GroupActions(actions);

         var codeRoutes = groupedActions.Select(g => CodeRoute.Create(g)).ToArray();
         
         foreach (var route in codeRoutes)
            routes.Add(route);

         return codeRoutes;
      }

      static void CheckSingleRootController(IEnumerable<ActionInfo> actions) {

         var controllersByBaseRoute = 
            from c in actions.Select(a => a.Controller).Distinct()
            group c by c.BaseRoute into g
            select g;

         foreach (var g in controllersByBaseRoute) {

            var rootControllers = g.Where(c => c.IsRootController).ToList();

            if (rootControllers.Count > 1) {

               throw new InvalidOperationException(
                  String.Format(CultureInfo.InvariantCulture,
                     "The root controller{0} is ambiguous between {1}.",
                     (String.IsNullOrEmpty(g.Key)) ? "" : String.Concat(" for base route '" + g.Key, "'"),
                     String.Join(" and ", rootControllers.Select(c => c.Type.FullName))
                  )
               );
            }
         }
      }

      static void CheckNoAmbiguousUrls(IEnumerable<ActionInfo> actions) {

         var ambiguousController =
            (from a in actions
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
             orderby a.Controller.IsRootController descending
                , (a.Controller.IsRootController && a.IsDefaultAction) descending
                , a.Controller.NamespaceRouteParts.Count
                , a.Controller.Type.Namespace
                , a.Controller.Name
                , a.Name
                , a.RouteParameters.Count descending
             let declaringType1 = a.Method.DeclaringType
             let declaringType = (declaringType1.IsGenericType) ?
                declaringType1.GetGenericTypeDefinition()
                : declaringType1
             group a by new {
                a.Controller.Type.Namespace,
                DeclaringType = declaringType,
                HasRouteParameters = (a.RouteParameters.Count > 0)
             }).ToList();

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
                         group a by a.Name into g
                         select g.Select(a => a.RouteParameters.Count).Max()
                        ).Distinct().ToArray();

                     foreach (var count in maxParamCounts) {

                        var sameMaxNumberOfParams =
                           (from a in signatureCompat
                            group a by a.Name into g
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

      public static void EnableCodeRouting(this ViewEngineCollection engines) {

         if (engines == null) throw new ArgumentNullException("engines");

         for (int i = 0; i < engines.Count; i++) {
            
            IViewEngine engine = engines[i];

            if (engine.GetType() == typeof(CodeRoutingViewEngineWrapper))
               continue;

            engines[i] = new CodeRoutingViewEngineWrapper(engine);
         }
      }
   }
}
