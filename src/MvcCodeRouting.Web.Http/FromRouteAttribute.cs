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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.Routing;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;
using MvcCodeRouting.Controllers;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Web.Http {

   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add route parameters after the {action} token 
   /// for each decorated action method parameter, and after the {controller} token for each 
   /// decorated controller property.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public sealed class FromRouteAttribute : ModelBinderAttribute, IModelBinder, IFromRouteAttribute {

      static readonly ConcurrentDictionary<Type, ModelBinderAttribute> modelBinderAttrCache = new ConcurrentDictionary<Type, ModelBinderAttribute>();

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

      public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter) {
         return new ModelBinderParameterBinding(parameter, this, GetValueProviderFactories(parameter.Configuration));
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
               return new ValueProviderFactory[1] { valueProviderFactory };
         }

         return new ValueProviderFactory[1] { new RouteDataValueProviderFactory() };
      }

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         if (this.BinderType != null
            && typeof(ParameterBinder).IsInstanceOfType(this.BinderType)) {

            // For consistency with Web.Mvc, skip model binding only if a ParameterBinder is specified
            // using [FromRoute(BinderType)]

            return BindParameter(actionContext, bindingContext);
         }

         if (this.Name != null)
            bindingContext.ModelName = this.Name;

         // For action parameters, ValueProvider is set by ModelBinderParameterBinding (see GetBinding).
         // For controller properties (BindRouteProperties), and if for some other reason it's null,
         // we set it here.

         if (bindingContext.ValueProvider == null) {

            // Multiple bindings with the same context should be able to reuse this computation

            bindingContext.ValueProvider = GetValueProviderFactories(actionContext.ControllerContext.Configuration)
               .Single()
               .GetValueProvider(actionContext);
         }

         IModelBinder modelBinder = GetRealBinder(this, bindingContext.ModelType, true /* fallbackToDefault */, actionContext.ControllerContext.Configuration);

         return modelBinder.BindModel(actionContext, bindingContext);
      }

      bool BindParameter(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         IHttpRouteData routeData = actionContext.ControllerContext.RouteData;
         string name = this.Name ?? bindingContext.ModelName;
         object rawValue;

         if (!routeData.Values.TryGetValue(name, out rawValue))
            return false;

         if (rawValue == null
            || bindingContext.ModelType.IsInstanceOfType(rawValue)) {

            bindingContext.Model = rawValue;
            return true;
         }

         object binders;
         IDictionary<string, object> bindersDictionary;
         object binder;
         ParameterBinder paramBinder = null;

         if (routeData.Route.DataTokens.TryGetValue(name, out binders)
            && (bindersDictionary = binders as IDictionary<string, object>) != null
            && bindersDictionary.TryGetValue(name, out binder)) {

            paramBinder = binder as ParameterBinder;
         }

         if (paramBinder == null) 
            paramBinder = ParameterBinder.CreateInstance(this.BinderType);

         object parsedValue;
         bool success = paramBinder.TryBind(Convert.ToString(rawValue, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, out parsedValue);

         bindingContext.Model = parsedValue;

         return success;
      }

      static IModelBinder GetRealBinder(IFromRouteAttribute fromRouteAttr, Type modelType, bool fallbackToDefault, HttpConfiguration configuration) {

         modelType = TypeHelpers.GetNullableUnderlyingType(modelType);

         ModelBinderAttribute modelBinderAttr = null;

         if (fromRouteAttr != null)
            modelBinderAttr = (ModelBinderAttribute)fromRouteAttr;

         if (modelBinderAttr == null
            || modelBinderAttr.BinderType == null) {

            ModelBinderAttribute modelTypeAttr = modelBinderAttrCache.GetOrAdd(modelType, t =>
               t.GetCustomAttributes(typeof(ModelBinderAttribute), inherit: true)
                  .Cast<ModelBinderAttribute>()
                  .SingleOrDefault()
            );

            if (modelTypeAttr != null
               && (modelBinderAttr == null
                  || modelTypeAttr.BinderType != null)) {

               modelBinderAttr = modelTypeAttr;
            }
         }

         if (modelBinderAttr == null)
            modelBinderAttr = new ModelBinderAttribute();

         IModelBinder binder = modelBinderAttr.GetModelBinder(configuration, modelType);

         if (binder == null
            || (!fallbackToDefault && binder.GetType() == typeof(CompositeModelBinder))) {

            return null;
         }

         return binder;
      }
   }
}
