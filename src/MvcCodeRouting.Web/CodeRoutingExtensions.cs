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
using System.Web.Routing;
using MvcCodeRouting.Web.Hosting;

namespace MvcCodeRouting {

   /// <summary>
   /// Provides the extension methods to register and configure modules in a host ASP.NET MVC application.
   /// </summary>
   public static partial class CodeRoutingExtensions {

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
      /// provided base route to the URL of each created route.
      /// </summary>
      /// <param name="routes">A collection of routes for the application.</param>
      /// <param name="baseRoute">A base route to prepend to the URL of each created route. This parameter can be null.</param>
      /// <param name="rootController">The route controller for the provided base route.</param>
      /// <param name="settings">A settings object that customizes the route creation process. This parameter can be null.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<Route> MapCodeRoutes(this RouteCollection routes, string baseRoute, Type rootController, CodeRoutingSettings settings) {

         if (routes == null) throw new ArgumentNullException("routes");
         if (rootController == null) throw new ArgumentNullException("rootController");

         var registerSettings = new RegisterSettings(null, rootController) { 
            BaseRoute = baseRoute, 
            Settings = settings
         };

         Route[] newRoutes = RouteFactory.CreateRoutes<Route>(registerSettings);

         foreach (Route route in newRoutes) {
            routes.Add(route);
         }

         if (newRoutes.Length > 0 
            && registerSettings.Settings.EnableEmbeddedViews) {
            
            EmbeddedViewsVirtualPathProvider.RegisterAssembly(registerSettings);
         }

         return newRoutes;
      }
   }
}
