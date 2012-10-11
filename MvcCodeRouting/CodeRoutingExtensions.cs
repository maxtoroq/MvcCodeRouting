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
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting.Web;
using MvcCodeRouting.Web.Hosting;

namespace MvcCodeRouting {

   /// <summary>
   /// Extension methods for reflection-based route creation and related functionality.
   /// </summary>
   public static class CodeRoutingExtensions {

      static readonly ConcurrentDictionary<Type, ControllerData> controllerDataCache = new ConcurrentDictionary<Type, ControllerData>();

      static CodeRoutingExtensions() {
         CodeRoutingProvider.RegisterProvider(new Web.Mvc.MvcCodeRoutingProvider());
      }

      internal static void Initialize() { }

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

         foreach (Route route in newRoutes) 
            routes.Add(route);

         if (newRoutes.Length > 0 
            && registerSettings.Settings.EnableEmbeddedViews) {
            
            EmbeddedViewsVirtualPathProvider.RegisterAssembly(registerSettings);
         }

         return newRoutes;
      }

      /// <summary>
      /// Enables namespace-aware views location. Always call after you are done adding view engines.
      /// </summary>
      /// <param name="engines">The view engine collection.</param>
      public static void EnableCodeRouting(this ViewEngineCollection engines) {

         if (engines == null) throw new ArgumentNullException("engines");

         for (int i = 0; i < engines.Count; i++) {

            IViewEngine engine = engines[i];

            if (engine.GetType() == typeof(Web.Mvc.ViewEngineWrapper))
               continue;

            engines[i] = new Web.Mvc.ViewEngineWrapper(engine);
         }

         EmbeddedViewsVirtualPathProvider.RegisterIfNecessary();
      }

      /// <summary>
      /// Binds controller properties decorated with <see cref="FromRouteAttribute"/>
      /// using the current route values.
      /// </summary>
      /// <param name="controller">The controller to bind.</param>
      /// <remarks>You can call this method from <see cref="ControllerBase.Initialize"/>.</remarks>
      public static void BindRouteProperties(this ControllerBase controller) {

         if (controller == null) throw new ArgumentNullException("controller");

         var controllerData = controllerDataCache
            .GetOrAdd(controller.GetType(), (type) => new ControllerData(type));

         if (controllerData.Properties.Length == 0)
            return;

         ModelMetadata metadata = controllerData.Metadata;
         metadata.Model = controller;

         var modelState = new ModelStateDictionary();

         ModelBindingContext bindingContext = new ModelBindingContext {
            FallbackToEmptyPrefix = true,
            ModelMetadata = metadata,
            ModelState = modelState,
            PropertyFilter = (p) => controllerData.Properties.Contains(p, StringComparer.Ordinal),
         };

         RouteValueDictionary values = null;
         bool hasCustomValues = false;

         if (controllerData.HasCustomTokens) {

            values = new RouteValueDictionary(controller.ControllerContext.RouteData.Values);
            
            for (int i = 0; i < controllerData.CustomTokens.Length; i++) {
               string tokenName = controllerData.CustomTokens[i];
               string propertyName = controllerData.Properties[i];

               if (tokenName != null 
                  && !values.ContainsKey(propertyName)) {
                  
                  values[propertyName] = values[tokenName];
                  hasCustomValues = true;
               }
            }
         }

         bindingContext.ValueProvider = (hasCustomValues) ?
            new DictionaryValueProvider<object>(values, CultureInfo.InvariantCulture)
            : new RouteDataValueProvider(controller.ControllerContext);

         IModelBinder binder = ModelBinders.Binders.GetBinder(bindingContext.ModelType, fallbackToDefault: true);

         binder.BindModel(controller.ControllerContext, bindingContext);

         if (!modelState.IsValid) {
            ModelError error = modelState.First(m => m.Value.Errors.Count > 0).Value.Errors.First(); 
            
            int statusCode = 404;
            string message = "Not Found";

            if (error.Exception != null)
               throw new HttpException(statusCode, message, error.Exception);

            throw new HttpException(statusCode, message);
         }
      }

      class ControllerData {

         public readonly ModelMetadata Metadata;
         public readonly string[] Properties;
         public readonly string[] CustomTokens;
         public readonly bool HasCustomTokens;

         public ControllerData(Type type) {

            var metadataProvider = new EmptyModelMetadataProvider();
            
            this.Metadata = metadataProvider.GetMetadataForType(null, type);

            var properties =
               (from p in type.GetProperties()
                let attr = p.GetCustomAttributes(typeof(FromRouteAttribute), inherit: true)
                  .Cast<FromRouteAttribute>()
                  .SingleOrDefault()
                where attr != null
                select new {
                   PropertyName = p.Name,
                   CustomTokenName = (attr.TokenName != null
                      && !String.Equals(p.Name, attr.TokenName, StringComparison.OrdinalIgnoreCase)) ?
                         attr.TokenName
                         : null
                }).ToArray();

            this.Properties = properties.Select(p => p.PropertyName).ToArray();
            this.CustomTokens = properties.Select(p => p.CustomTokenName).ToArray();
            this.HasCustomTokens = this.CustomTokens.Any(s => s != null);
         }
      }
   }
}
