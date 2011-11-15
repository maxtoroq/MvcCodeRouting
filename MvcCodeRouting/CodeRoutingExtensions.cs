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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {

   /// <summary>
   /// Extension methods for reflection-based route creation and related functionality.
   /// </summary>
   public static class CodeRoutingExtensions {

      static readonly List<ActionInfo> registeredActions = new List<ActionInfo>();
      static readonly ConcurrentDictionary<Type, Tuple<ModelMetadata, string[]>> controllerDataCache = new ConcurrentDictionary<Type, Tuple<ModelMetadata, string[]>>();

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly.
      /// </summary>
      /// <param name="routes">A collection of routes for the application.</param>
      /// <param name="rootController">The route controller for the application.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, Type rootController) {
         return MapCodeRoutes(routes, rootController, null);
      }

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly.
      /// </summary>
      /// <param name="routes">A collection of routes for the application.</param>
      /// <param name="rootController">The route controller for the application.</param>
      /// <param name="settings">A settings object that customizes the route creation process. This parameter can be null.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, Type rootController, CodeRoutingSettings settings) {
         return MapCodeRoutes(routes, null, rootController, settings);
      }

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly, and prepends the
      /// provided base route to the URL of each created route.
      /// </summary>
      /// <param name="routes">A collection of routes for the application.</param>
      /// <param name="baseRoute">A base route to prepend to the URL of each created route. This parameter can be null.</param>
      /// <param name="rootController">The route controller for the provided base route.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, string baseRoute, Type rootController) {
         return MapCodeRoutes(routes, baseRoute, rootController, null);
      }

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly, and prepends the
      /// provided base route to the url of each created route.
      /// </summary>
      /// <param name="routes">A collection of routes for the application.</param>
      /// <param name="baseRoute">A base route to prepend to the URL of each created route. This parameter can be null.</param>
      /// <param name="rootController">The route controller for the provided base route.</param>
      /// <param name="settings">A settings object that customizes the route creation process. This parameter can be null.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, string baseRoute, Type rootController, CodeRoutingSettings settings) {

         if (routes == null) throw new ArgumentNullException("routes");
         if (rootController == null) throw new ArgumentNullException("rootController");

         var registerInfo = new RegisterInfo(rootController) { 
            BaseRoute = baseRoute, 
            Settings = settings 
         };

         var actions = ControllerInfo.GetControllers(registerInfo)
            .SelectMany(c => c.GetActions());

         registeredActions.AddRange(actions);

         CheckNoAmbiguousUrls(registeredActions);

         var groupedActions = GroupActions(actions);

         var codeRoutes = groupedActions.Select(g => CodeRoute.Create(g)).ToArray();
         
         foreach (var route in codeRoutes)
            routes.Add(route);

         return codeRoutes;
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
             let declaringType1 = a.DeclaringType
             let declaringType = (declaringType1.IsGenericType) ?
                declaringType1.GetGenericTypeDefinition()
                : declaringType1
             group a by new {
                Depth = a.Controller.ControllerBaseRouteSegments.Count
                , a.Controller.IsRootController
                , a.Controller.Namespace
                , DeclaringType = declaringType
                , HasRouteParameters = (a.RouteParameters.Count > 0)
             } into g
             orderby g.Key.IsRootController descending
                , g.Key.Depth
                , g.Key.Namespace
                , g.Key.HasRouteParameters descending
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

      /// <summary>
      /// Enables namespace-aware views location.
      /// </summary>
      /// <param name="engines">The view engine collection.</param>
      /// <remarks>Always call after you are done adding view engines.</remarks>
      public static void EnableCodeRouting(this ViewEngineCollection engines) {

         if (engines == null) throw new ArgumentNullException("engines");

         for (int i = 0; i < engines.Count; i++) {
            
            IViewEngine engine = engines[i];

            if (engine.GetType() == typeof(ViewEngineWrapper))
               continue;

            engines[i] = new ViewEngineWrapper(engine);
         }
      }

      /// <summary>
      /// Binds controller properties decorated with <see cref="FromRouteAttribute"/>
      /// using the current route values.
      /// </summary>
      /// <param name="controller">The controller to bind.</param>
      /// <remarks>You can call this method from <see cref="ControllerBase.Initialize"/>.</remarks>
      public static void BindRouteProperties(this ControllerBase controller) {

         if (controller == null) throw new ArgumentNullException("controller");

         var controllerData = controllerDataCache.GetOrAdd(controller.GetType(), (type) => {

            var metadataProvider = new EmptyModelMetadataProvider();
            var controllerMetadata = metadataProvider.GetMetadataForType(null, type);

            var properties =
               (from p in type.GetProperties()
                where p.IsDefined(typeof(FromRouteAttribute), inherit: true)
                select p.Name).ToArray();

            return Tuple.Create(controllerMetadata, properties);
         });

         var metadata = controllerData.Item1;
         metadata.Model = controller;

         var modelState = new ModelStateDictionary();

         ModelBindingContext bindingContext = new ModelBindingContext {
            FallbackToEmptyPrefix = true,
            ModelMetadata = metadata,
            ModelState = modelState,
            PropertyFilter = (p) => controllerData.Item2.Contains(p, StringComparer.Ordinal),
            ValueProvider = new RouteDataValueProvider(controller.ControllerContext)
         };

         ModelBinders.Binders.DefaultBinder.BindModel(controller.ControllerContext, bindingContext);

         if (!modelState.IsValid) {
            ModelError error = modelState.First(m => m.Value.Errors.Count > 0).Value.Errors.First(); 
            
            int statusCode = 404;
            string message = "Not Found";

            if (error.Exception != null)
               throw new HttpException(statusCode, message, error.Exception);

            throw new HttpException(statusCode, message);
         }
      }
   }
}
