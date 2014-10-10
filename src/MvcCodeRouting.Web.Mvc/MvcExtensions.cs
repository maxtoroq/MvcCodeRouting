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
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using FromRouteAttr = MvcCodeRouting.FromRouteAttribute;

namespace MvcCodeRouting.Web.Mvc {

   /// <summary>
   /// Extensions methods that provide utility functions for various ASP.NET MVC classes.
   /// </summary>
   [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc")]
   public static class MvcExtensions {

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

         if (controllerData.Properties.Length == 0) {
            return;
         }

         var modelState = new ModelStateDictionary();

         var bindingContext = new ModelBindingContext {
            FallbackToEmptyPrefix = true,
            ModelState = modelState
         };

         for (int i = 0; i < controllerData.Properties.Length; i++) {

            var propertyData = controllerData.Properties[i];

            propertyData.BindProperty(controller.ControllerContext, bindingContext);

            if (!modelState.IsValid) {
               break;
            }
         }

         if (!modelState.IsValid) {

            ModelError error = modelState.First(m => m.Value.Errors.Count > 0).Value.Errors.First(); 
            
            int statusCode = 404;
            string message = "Not Found";

            if (error.Exception != null) {
               throw new HttpException(statusCode, message, error.Exception);
            }

            throw new HttpException(statusCode, message);
         }
      }

#pragma warning disable 0618

      class ControllerData {

         public readonly PropertyData[] Properties;

         public ControllerData(Type type) {

            var metadataProvider = new EmptyModelMetadataProvider();
            
            this.Properties =
               (from p in type.GetProperties()
                let attr = p.GetCustomAttributes(typeof(FromRouteAttr), inherit: true)
                  .Cast<FromRouteAttr>()
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
         readonly FromRouteAttr Attribute;
         readonly ModelMetadata Metadata;

         public PropertyData(PropertyInfo property, FromRouteAttr attribute, ModelMetadata metadata) {

            this.Property = property;
            this.Name = property.Name;
            this.Attribute = attribute;
            this.Metadata = metadata;
         }

         [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is rethrown by caller.")]
         public void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext) {

            bindingContext.ModelName = this.Name;
            bindingContext.ModelMetadata = this.Metadata;

            object value = this.Attribute.BindModel(controllerContext, bindingContext);

            try {
               this.Property.SetValue(controllerContext.Controller, value, null);

            } catch (Exception ex) {

               if (bindingContext.ModelState.IsValidField(this.Name)) {
                  bindingContext.ModelState.AddModelError(this.Name, ex);
               }
            }
         }
      }
   }

#pragma warning restore 0618

}
