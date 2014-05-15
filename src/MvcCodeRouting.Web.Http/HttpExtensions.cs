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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.Metadata.Providers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders.Providers;

namespace MvcCodeRouting.Web.Http {

   /// <summary>
   /// Extensions methods that provide utility functions for various ASP.NET Web API classes.
   /// </summary>
   public static class HttpExtensions {

      static readonly ConcurrentDictionary<Type, ControllerData> controllerDataCache = new ConcurrentDictionary<Type, ControllerData>();

      /// <summary>
      /// Binds controller properties decorated with <see cref="FromRouteAttribute"/>
      /// using the current route values.
      /// </summary>
      /// <param name="controller">The controller to bind.</param>
      /// <remarks>You can call this method from <see cref="ApiController.Initialize"/>.</remarks>
      public static void BindRouteProperties(this ApiController controller) { 
         
         if (controller == null) throw new ArgumentNullException("controller");

         var controllerData = controllerDataCache
            .GetOrAdd(controller.GetType(), (type) => new ControllerData(type));

         if (controllerData.Properties.Length == 0) {
            return;
         }

         var modelState = new ModelStateDictionary();

         var actionContext = new HttpActionContext { 
            ControllerContext = controller.ControllerContext 
         };

         var bindingContext = new ModelBindingContext {
            FallbackToEmptyPrefix = true,
            ModelState = modelState
         };

         for (int i = 0; i < controllerData.Properties.Length; i++) {

            var propertyData = controllerData.Properties[i];

            propertyData.BindProperty(actionContext, bindingContext);

            if (!modelState.IsValid) {
               break;
            }
         }

         if (!modelState.IsValid) {

            ModelError error = modelState.First(m => m.Value.Errors.Count > 0).Value.Errors.First();

            HttpStatusCode statusCode = HttpStatusCode.NotFound;
            string message = "Not Found";

            if (error.Exception != null) {
               throw new HttpResponseException(controller.Request.CreateErrorResponse(statusCode, message, error.Exception));
            }

            throw new HttpResponseException(controller.Request.CreateErrorResponse(statusCode, message));
         }
      }

      class ControllerData {

         public readonly PropertyData[] Properties;

         public ControllerData(Type type) {

            var metadataProvider = new EmptyModelMetadataProvider();

            this.Properties =
               (from p in type.GetProperties()
                let attr = p.GetCustomAttributes(typeof(FromRouteAttribute), inherit: true)
                  .Cast<FromRouteAttribute>()
                  .SingleOrDefault()
                where attr != null
                select new PropertyData(
                   property: p,
                   attribute: attr,
                   metadata: metadataProvider.GetMetadataForType(null, p.PropertyType)
                )).ToArray();
         }
      }

      class PropertyData {

         readonly PropertyInfo Property;
         readonly string Name;
         readonly FromRouteAttribute Attribute;
         readonly ModelMetadata Metadata;

         public PropertyData(PropertyInfo property, FromRouteAttribute attribute, ModelMetadata metadata) {

            this.Property = property;
            this.Name = property.Name;
            this.Attribute = attribute;
            this.Metadata = metadata;
         }

         [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is rethrown by caller.")]
         public void BindProperty(HttpActionContext actionContext, ModelBindingContext bindingContext) {

            bindingContext.ModelName = this.Name;
            bindingContext.ModelMetadata = this.Metadata;

            if (!this.Attribute.BindModel(actionContext, bindingContext)) {
               return;
            }

            object value = bindingContext.Model;

            try {
               this.Property.SetValue(actionContext.ControllerContext.Controller, value, null);

            } catch (Exception ex) {

               if (bindingContext.ModelState.IsValidField(bindingContext.ModelName)) {
                  bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
               }
            }
         }
      }
   }
}
