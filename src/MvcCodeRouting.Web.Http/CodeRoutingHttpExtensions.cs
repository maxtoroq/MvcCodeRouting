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
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using MvcCodeRouting.Web.Http.Controllers;
using MvcCodeRouting.Web.Http.Dispatcher;

namespace MvcCodeRouting {

   /// <summary>
   /// Provides the extension methods to register and configure modules in a host ASP.NET Web API application.
   /// </summary>
   public static class CodeRoutingHttpExtensions {

      static CodeRoutingHttpExtensions() {
         CodeRoutingProvider.RegisterProvider(new HttpCodeRoutingProvider());
      }

      internal static void Initialize() { }

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly.
      /// </summary>
      /// <param name="configuration">The <see cref="System.Web.Http.HttpConfiguration"/> configuration object.</param>
      /// <param name="rootController">The route controller for the application.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpConfiguration configuration, Type rootController) {
         return MapCodeRoutes(configuration, rootController, null);
      }

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly.
      /// </summary>
      /// <param name="configuration">The <see cref="System.Web.Http.HttpConfiguration"/> configuration object.</param>
      /// <param name="rootController">The route controller for the application.</param>
      /// <param name="settings">A settings object that customizes the route creation process. This parameter can be null.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpConfiguration configuration, Type rootController, CodeRoutingSettings settings) {
         return MapCodeRoutes(configuration, null, rootController, settings);
      }

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly, and prepends the
      /// provided base route to the URL of each created route.
      /// </summary>
      /// <param name="configuration">The <see cref="System.Web.Http.HttpConfiguration"/> configuration object.</param>
      /// <param name="baseRoute">A base route to prepend to the URL of each created route. This parameter can be null.</param>
      /// <param name="rootController">The route controller for the provided base route.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpConfiguration configuration, string baseRoute, Type rootController) {
         return MapCodeRoutes(configuration, baseRoute, rootController, null);
      }

      /// <summary>
      /// Creates routes for the specified root controller and all other controllers
      /// in the same namespace or any sub-namespace, in the same assembly, and prepends the
      /// provided base route to the URL of each created route.
      /// </summary>
      /// <param name="configuration">The <see cref="System.Web.Http.HttpConfiguration"/> configuration object.</param>
      /// <param name="baseRoute">A base route to prepend to the URL of each created route. This parameter can be null.</param>
      /// <param name="rootController">The route controller for the provided base route.</param>
      /// <param name="settings">A settings object that customizes the route creation process. This parameter can be null.</param>
      /// <returns>The created routes.</returns>
      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpConfiguration configuration, string baseRoute, Type rootController, CodeRoutingSettings settings) {

         if (configuration == null) throw new ArgumentNullException("configuration");
         if (rootController == null) throw new ArgumentNullException("rootController");

         if (settings != null) {
            settings = new CodeRoutingSettings(settings);
         }

         var registerSettings = new RegisterSettings(null, rootController) {
            BaseRoute = baseRoute,
            Settings = settings
         };

         registerSettings.Settings.HttpConfiguration(configuration);

         IHttpRoute[] newRoutes = RouteFactory.CreateRoutes<IHttpRoute>(registerSettings);

         foreach (IHttpRoute route in newRoutes) {
            // In Web API v1 name cannot be null
            configuration.Routes.Add(Guid.NewGuid().ToString(), route);
         }

         EnableCodeRouting(configuration);

         return newRoutes;
      }

      internal static HttpConfiguration HttpConfiguration(this CodeRoutingSettings settings) {

         if (settings == null) throw new ArgumentNullException("settings");

         object httpConfiguration;

         if (settings.Properties.TryGetValue("HttpConfiguration", out httpConfiguration)) {
            return httpConfiguration as HttpConfiguration;
         }

         return null;
      }

      internal static void HttpConfiguration(this CodeRoutingSettings settings, HttpConfiguration configuration) {

         if (settings == null) throw new ArgumentNullException("settings");

         settings.Properties["HttpConfiguration"] = configuration;
      }

      internal static void EnableCodeRouting(HttpConfiguration configuration) {

         if (!(configuration.Services.GetHttpControllerSelector() is CustomHttpControllerSelector)) {
            configuration.Services.Replace(typeof(IHttpControllerSelector), new CustomHttpControllerSelector(configuration));
         }

         if (!(configuration.Services.GetActionSelector() is CustomApiControllerActionSelector)) {
            configuration.Services.Replace(typeof(IHttpActionSelector), new CustomApiControllerActionSelector());
         }
      }
   }
}
