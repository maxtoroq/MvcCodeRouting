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
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Metadata;
using System.Web.Http.Metadata.Providers;
using System.Web.Http.ModelBinding;

namespace MvcCodeRouting.Web.Http {
   
   public static class HttpExtensions {

      static readonly ConcurrentDictionary<Type, ControllerData> controllerDataCache = new ConcurrentDictionary<Type, ControllerData>();

      /// <summary>
      /// Binds controller properties decorated with <see cref="FromRouteAttribute"/>
      /// using the current route values.
      /// </summary>
      /// <param name="controller">The controller to bind.</param>
      /// <remarks>You can call this method from <see cref="ApiController.Initialize"/>.</remarks>
      [Obsolete("This method is not implemented yet. Please wait for the next release.", error: true)]
      public static void BindRouteProperties(this ApiController controller) { 
         
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
            //PropertyFilter = (p) => controllerData.Properties.Contains(p, StringComparer.Ordinal),
         };

         throw new NotImplementedException();
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
                   CustomTokenName = (attr.Name.HasValue()
                      && !String.Equals(p.Name, attr.Name, StringComparison.OrdinalIgnoreCase)) ?
                         attr.Name
                         : null
                }).ToArray();

            this.Properties = properties.Select(p => p.PropertyName).ToArray();
            this.CustomTokens = properties.Select(p => p.CustomTokenName).ToArray();
            this.HasCustomTokens = this.CustomTokens.Any(s => s != null);
         }
      }
   }
}
