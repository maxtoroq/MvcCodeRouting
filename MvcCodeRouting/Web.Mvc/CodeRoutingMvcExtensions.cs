// Copyright 2013 Max Toro Q.
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
using MvcCodeRouting.Web.Hosting;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {

   /// <summary>
   /// Extension methods for reflection-based route creation and related functionality.
   /// </summary>
   public static class CodeRoutingMvcExtensions {

      static readonly ConcurrentDictionary<Type, ControllerData> controllerDataCache = new ConcurrentDictionary<Type, ControllerData>();

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

         var bindingContext = new ModelBindingContext {
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

#pragma warning disable 0618

            var properties =
               (from p in type.GetProperties()
                let attr = p.GetCustomAttributes(typeof(MvcCodeRouting.FromRouteAttribute), inherit: true)
                  .Cast<IFromRouteAttribute>()
                  .SingleOrDefault()
                where attr != null
                select new {
                   PropertyName = p.Name,
                   CustomTokenName = (attr.Name.HasValue()
                      && !String.Equals(p.Name, attr.Name, StringComparison.OrdinalIgnoreCase)) ?
                         attr.Name
                         : null
                }).ToArray();

#pragma warning restore 0618

            this.Properties = properties.Select(p => p.PropertyName).ToArray();
            this.CustomTokens = properties.Select(p => p.CustomTokenName).ToArray();
            this.HasCustomTokens = this.CustomTokens.Any(s => s != null);
         }
      }
   }
}
