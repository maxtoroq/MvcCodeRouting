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

namespace MvcCodeRouting.Web.Http {

   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add token segments for each
   /// action method parameter after the {action} token, and for each controller property
   /// after the {controller} token.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public sealed class FromRouteAttribute : ModelBinderAttribute, IFromRouteAttribute {

      string _TokenName;

      /// <summary>
      /// The token name. The default name used is the parameter or property name.
      /// </summary>
      public string TokenName {
         get { return _TokenName; }
         private set {
            if (value != null && value.Length == 0)
               throw new ArgumentException("value cannot be empty.", "value");

            _TokenName = value;
         }
      }

      /// <summary>
      /// A regular expression that specify valid values for the decorated parameter or property.
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
      /// using the specified token name.
      /// </summary>
      /// <param name="tokenName">The token name.</param>
      public FromRouteAttribute(string tokenName) {
         this.TokenName = tokenName;
      }

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
