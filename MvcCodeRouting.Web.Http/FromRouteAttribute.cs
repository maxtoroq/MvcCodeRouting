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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Http {

   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add route parameters after the {action} token 
   /// for each decorated action method parameter, and after the {controller} token for each 
   /// decorated controller property.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public sealed class FromRouteAttribute : ModelBinderAttribute, IFromRouteAttribute {

      /// <summary>
      /// Gets or sets a regular expression that specify valid values for the decorated parameter or property.
      /// </summary>
      public string Constraint { get; set; }

      /// <summary>
      /// true if the parameter represents a catch-all token; otherwise, false.
      /// This setting is ignored when used on controller properties.
      /// </summary>
      [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CatchAll", Justification = "Consistent with naming used in the .NET Framework.")]
      public bool CatchAll { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class.
      /// </summary>
      public FromRouteAttribute() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class 
      /// using the specified name.
      /// </summary>
      /// <param name="name">The name of the route parameter.</param>
      public FromRouteAttribute(string name) {
         this.Name = name;
      }

      /// <summary>
      /// Gets the value providers that will be fed to the model binder.
      /// </summary>
      /// <param name="configuration">The <see cref="HttpConfiguration"/> configuration object.</param>
      /// <returns>A collection of <see cref="ValueProviderFactory"/> instances.</returns>
      public override IEnumerable<ValueProviderFactory> GetValueProviderFactories(HttpConfiguration configuration) {

         if (configuration == null) throw new ArgumentNullException("configuration");

         foreach (ValueProviderFactory valueProviderFactory in base.GetValueProviderFactories(configuration)) {
            
            if (valueProviderFactory is RouteDataValueProviderFactory)
               return new[] { valueProviderFactory };
         }

         return new[] { new RouteDataValueProviderFactory() };
      }
   }
}
