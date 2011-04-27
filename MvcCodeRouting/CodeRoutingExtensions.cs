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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Globalization;

namespace MvcCodeRouting {

   public static class CodeRoutingExtensions {

      static readonly List<ActionInfo> registeredActions = new List<ActionInfo>();

      public static void MapCodeRoutes(this RouteCollection routes, string baseNamespace) {
         MapCodeRoutes(routes, Assembly.GetCallingAssembly(), baseNamespace, (CodeRoutingSettings)null);
      }

      public static void MapCodeRoutes(this RouteCollection routes, string baseNamespace, CodeRoutingSettings settings) {
         MapCodeRoutes(routes, Assembly.GetCallingAssembly(), baseNamespace, settings);
      }

      public static void MapCodeRoutes(this RouteCollection routes, Assembly assembly, string baseNamespace) {
         MapCodeRoutes(routes, assembly, baseNamespace, (CodeRoutingSettings)null);
      }

      public static void MapCodeRoutes(this RouteCollection routes, Assembly assembly, string baseNamespace, CodeRoutingSettings settings) {

         if (routes == null) throw new ArgumentNullException("routes");
         if (assembly == null) throw new ArgumentNullException("assembly");
         if (baseNamespace == null) throw new ArgumentNullException("baseNamespace");

         if (settings == null)
            settings = new CodeRoutingSettings();

         var actions = ControllerInfo.GetControllers(assembly, baseNamespace, settings)
            .SelectMany(c => c.GetActions());

         registeredActions.AddRange(actions);

         CheckSingleRootController(registeredActions);
         CheckNoAmbiguousUrls(registeredActions);

         var groupedActions = GroupActions(actions);

         foreach (var route in groupedActions.Select(g => CodeRoute.Create(g)))
            routes.Add(route);
      }

      static void CheckSingleRootController(IEnumerable<ActionInfo> actions) {

         var rootControllers = actions
            .Select(a => a.Controller)
            .Where(c => c.IsRootController)
            .Distinct()
            .ToList();

         if (rootControllers.Count > 1) {

            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture,
                  "The root controller is ambiguous between {0}.",
                  String.Join(" and ", rootControllers.Select(c => c.Type.FullName))
               )
            );
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
                , a.Controller.BaseRouteSegments.Count
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

         var overloadsComparer = new RouteEqualityComparer();
         var finalGrouping = new List<IEnumerable<ActionInfo>>();

         for (int i = 0; i < groupedActions.Count; i++) {

            var set = groupedActions[i];

            if (set.Key.HasRouteParameters) {

               var ordered = set.OrderByDescending(a => a.RouteParameters.Count).ToList();

               while (ordered.Count > 0) {
                  var firstInSet = ordered.First();
                  var similar = ordered.Skip(1).Where(a => overloadsComparer.Equals(firstInSet, a)).ToList();

                  if (similar.Count > 0) {
                     var overloadCompat = new[] { firstInSet }.Concat(similar).ToArray();
                     
                     var index = 0;
                     var k = 0;
                     var overloadRanges =
                        (from a in overloadCompat
                           let idx = ++index
                           let next = overloadCompat.ElementAtOrDefault(idx)
                           let diff = (next == null) ? 0 : Math.Abs(a.RouteParameters.Count - next.RouteParameters.Count)
                           let key = (diff == 1 || diff == 0) ?
                           k : k++
                           group a by key into g
                           select g).ToArray();

                     foreach (var range in overloadRanges) {

                        var actionCounts =
                           (from a in range
                            group a by a.Name into g
                            select g.Count()).Distinct().ToArray();

                        foreach (var count in actionCounts) {

                           var sameRangeSameNumberOfOverloads =
                              (from a in range
                               group a by a.Name into g
                               where g.Count() == count
                               select g)
                              .SelectMany(g => g)
                              .Distinct()
                              .OrderByDescending(a => a.RouteParameters.Count)
                              .ToArray();

                           if (sameRangeSameNumberOfOverloads.Length > 1) {

                              var first = sameRangeSameNumberOfOverloads.First();
                              var last = sameRangeSameNumberOfOverloads.Last();

                              foreach (var param in first.RouteParameters.Skip(last.RouteParameters.Count))
                                 param.IsOptional = true; 
                           }

                           finalGrouping.Add(sameRangeSameNumberOfOverloads);

                           foreach (var item in sameRangeSameNumberOfOverloads)
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
            engines[i] = new CodeRoutingViewEngineWrapper(engine);
         }
      }
   }

   class RouteEqualityComparer : IEqualityComparer<ActionInfo> {

      public bool Equals(ActionInfo x, ActionInfo y) {

         if (x == null)
            return y == null;

         if (y == null)
            return x == null;

         return CheckRouteParameters(x, y)
            && CheckRouteParameters(y, x);
      }

      bool CheckRouteParameters(ActionInfo x, ActionInfo y) {

         for (int i = 0; i < x.RouteParameters.Count; i++) {
            var p = x.RouteParameters[i];

            if (y.RouteParameters.Count - 1 >= i) {
               var p2 = y.RouteParameters[i];

               if (!RouteParameterInfo.NameEquals(p.Name, p2.Name)
                  || p.Constraint != p2.Constraint)
                  return false;
            }
         }

         return true;
      }

      public int GetHashCode(ActionInfo obj) {
         throw new NotImplementedException();
      }
   }
}
