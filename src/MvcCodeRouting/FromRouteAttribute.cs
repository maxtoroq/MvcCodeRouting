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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting.Controllers;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add route parameters after the {action} segment 
   /// for each decorated action method parameter, and after the {controller} segment for each 
   /// decorated controller property.
   /// </summary>
   [Obsolete("Please use MvcCodeRouting.Web.Mvc.FromRouteAttribute instead.")]
   [EditorBrowsable(EditorBrowsableState.Never)]
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public class FromRouteAttribute : CustomModelBinderAttribute, IModelBinder, IFromRouteAttribute {

      /// <summary>
      /// Gets or sets the route parameter name. The default name used is the parameter or property name.
      /// </summary>
      public virtual string Name { get; set; }

      /// <summary>
      /// Gets or sets the route parameter name. The default name used is the parameter or property name.
      /// </summary>
      [Obsolete("Please use Name instead.")]
      public string TokenName { get { return Name; } }

      /// <summary>
      /// Gets or sets a regular expression that specify valid values for the decorated parameter or property.
      /// </summary>
      public virtual string Constraint { get; set; }

      /// <summary>
      /// true if the parameter represents a catch-all parameter; otherwise, false.
      /// This setting is ignored when used on controller properties.
      /// </summary>
      [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CatchAll", Justification = "Consistent with naming used in the .NET Framework.")]
      public virtual bool CatchAll { get; set; }

      /// <summary>
      /// Gets or sets the type of the binder.
      /// </summary>
      public virtual Type BinderType { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class.
      /// </summary>
      public FromRouteAttribute() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class 
      /// using the specified name.
      /// </summary>
      /// <param name="tokenName">The name of the route parameter.</param>
      public FromRouteAttribute(string tokenName) {
         this.Name = tokenName;
      }

      /// <summary>
      /// Gets the model binder used to bind the decorated parameter or property.
      /// </summary>
      /// <returns>The model binder.</returns>
      public override IModelBinder GetBinder() {
         return this;
      }

      /// <summary>
      /// Binds the decorated parameter or property to a value by using the specified controller context and
      /// binding context.
      /// </summary>
      /// <param name="controllerContext">The controller context.</param>
      /// <param name="bindingContext">The binding context.</param>
      /// <returns>The bound value.</returns>
      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         if (this.BinderType != null) {

            if (typeof(ParameterBinder).IsAssignableFrom(this.BinderType)) {

               object r1;

               ParamBind(controllerContext, bindingContext, out r1);

               return r1;
            }

            return ModelBind(controllerContext, bindingContext);
         }

         bool isDefaultBinder;
         IModelBinder modelBinder = GetModelBinder(this, bindingContext.ModelType, out isDefaultBinder);

         if (!isDefaultBinder) {
            return ModelBind(controllerContext, bindingContext, modelBinder);
         }

         object r2;

         if (ParamBind(controllerContext, bindingContext, out r2).HasValue) {
            return r2;
         }

         return ModelBind(controllerContext, bindingContext, modelBinder);
      }

      bool? ParamBind(ControllerContext controllerContext, ModelBindingContext bindingContext, out object result) {

         RouteData routeData = controllerContext.RouteData;
         string name = this.Name ?? bindingContext.ModelName;

         object rawValue;

         if (!routeData.Values.TryGetValue(name, out rawValue)) {

            result = null;
            return false;
         }

         if (rawValue == null
            || bindingContext.ModelType.IsInstanceOfType(rawValue)) {

            result = rawValue;
            return true;
         }

         ParameterBinder paramBinder = null;

         Route route = routeData.Route as Route;
         object binders;
         IDictionary<string, ParameterBinder> bindersDictionary;

         if (route != null
            && route.DataTokens.TryGetValue(DataTokenKeys.ParameterBinders, out binders)
            && (bindersDictionary = binders as IDictionary<string, ParameterBinder>) != null) {

            bindersDictionary.TryGetValue(name, out paramBinder);
         }

         if (paramBinder == null) {
            paramBinder = ParameterBinder.GetInstance(bindingContext.ModelType, this.BinderType);
         }

         if (paramBinder == null) {
            result = null;
            return null;
         }

         return paramBinder.TryBind(Convert.ToString(rawValue, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, out result);
      }

      object ModelBind(ControllerContext controllerContext, ModelBindingContext bindingContext, IModelBinder binder = null) {

         if (this.Name != null) {
            bindingContext.ModelName = this.Name;
         }

         // For action parameters, ValueProvider is ValueProviderCollection (composite).
         // For controller properties (BindRouteProperties), it's null.
         // That's why we always need to set it to the appropriate instance here.

         bindingContext.ValueProvider = (ValueProviderFactories.Factories
            .OfType<RouteDataValueProviderFactory>()
            .FirstOrDefault()
            ?? new RouteDataValueProviderFactory())
            .GetValueProvider(controllerContext);

         if (binder == null) {
            bool isDefaultBinder;
            binder = GetModelBinder(this, bindingContext.ModelType, out isDefaultBinder);
         }

         return binder.BindModel(controllerContext, bindingContext);
      }

      static IModelBinder GetModelBinder(IFromRouteAttribute fromRouteAttr, Type modelType, out bool isDefaultBinder) {

         if (fromRouteAttr != null
            && fromRouteAttr.BinderType != null) {

            try {
               isDefaultBinder = false;

               return (IModelBinder)Activator.CreateInstance(fromRouteAttr.BinderType);

            } catch (Exception ex) {
               throw new InvalidOperationException("An error occurred when trying to create the IModelBinder '{0}'. Make sure that the binder has a public parameterless constructor.".FormatInvariant(fromRouteAttr.BinderType.FullName), ex);
            }
         }

         modelType = TypeHelpers.GetNullableUnderlyingType(modelType);

         IModelBinder binder = ModelBinders.Binders.GetBinder(modelType, fallbackToDefault: false);

         if (binder != null) {
            isDefaultBinder = false;
            return binder;
         }

         isDefaultBinder = true;
         
         return ModelBinders.Binders.DefaultBinder;
      }
   }
}
