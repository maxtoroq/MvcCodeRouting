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
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add token segments for each
   /// action method parameter after the {action} token, and for each controller property
   /// after the {controller} token.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public sealed class FromRouteAttribute : CustomModelBinderAttribute, IModelBinder {

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
      /// Gets the model binder used to bind the decorated parameter.
      /// </summary>
      /// <returns>The model binder.</returns>
      public override IModelBinder GetBinder() {
         return this;
      }

      /// <summary>
      /// Binds the decorated parameter to a value by using the specified controller context and
      /// binding context.
      /// </summary>
      /// <param name="controllerContext">The controller context.</param>
      /// <param name="bindingContext">The binding context.</param>
      /// <returns>The bound value.</returns>
      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         bindingContext.ValueProvider = new RouteDataValueProvider(controllerContext);

         return ModelBinders.Binders.DefaultBinder.BindModel(controllerContext, bindingContext);
      }
   }
}
