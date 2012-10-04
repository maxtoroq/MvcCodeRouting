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
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Web.Mvc;

namespace MvcCodeRouting.Web.Mvc {
   
   abstract class MvcControllerInfo : ControllerInfo {

      static readonly Type BaseType = typeof(Controller);
      static readonly Func<Controller, IActionInvoker> createActionInvoker;
      static readonly Func<ControllerActionInvoker, ControllerContext, ControllerDescriptor> getControllerDescriptor;

      readonly RouteFactory _RouteFactory = new MvcRouteFactory();
      readonly Type _FromRouteAttributeType = typeof(FromRouteAttribute);
      readonly Type _CustomRouteAttributeType = typeof(CustomRouteAttribute);

      public override RouteFactory RouteFactory {
         get { return _RouteFactory; }
      }

      public override bool CanDisambiguateActionOverloads {
         get { return false; }
      }

      public override Type FromRouteAttributeType {
         get { return _FromRouteAttributeType; ; }
      }

      public override Type CustomRouteAttributeType {
         get { return _CustomRouteAttributeType; }
      }

      [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Not a big deal.")]
      static MvcControllerInfo() {

         try {
            createActionInvoker =
               (Func<Controller, IActionInvoker>)
                  Delegate.CreateDelegate(typeof(Func<Controller, IActionInvoker>), BaseType.GetMethod("CreateActionInvoker", BindingFlags.NonPublic | BindingFlags.Instance));

            getControllerDescriptor =
               (Func<ControllerActionInvoker, ControllerContext, ControllerDescriptor>)
                  Delegate.CreateDelegate(typeof(Func<ControllerActionInvoker, ControllerContext, ControllerDescriptor>), typeof(ControllerActionInvoker).GetMethod("GetControllerDescriptor", BindingFlags.NonPublic | BindingFlags.Instance));
         
         } catch (MethodAccessException) { }
      }

      public static new ControllerInfo Create(Type controllerType, RegisterSettings registerSettings) {

         ControllerDescriptor controllerDescr = null;

         if (createActionInvoker != null) {

            Controller instance = null;

            try {
               instance = (Controller)FormatterServices.GetUninitializedObject(controllerType);
            } catch (SecurityException) { }

            if (instance != null) {

               ControllerActionInvoker actionInvoker = createActionInvoker(instance) as ControllerActionInvoker;

               if (actionInvoker != null)
                  controllerDescr = getControllerDescriptor(actionInvoker, new ControllerContext { Controller = instance });
            }
         }

         if (controllerDescr != null)
            return new DescribedMvcControllerInfo(controllerDescr, controllerType, registerSettings);

         return new ReflectedMvcControllerInfo(controllerType, registerSettings);
      }

      public static bool IsMvcController(Type type) {
         return BaseType.IsAssignableFrom(type);
      }

      public MvcControllerInfo(Type type, RegisterSettings registerInfo) 
         : base(type, registerInfo) { }

      protected override bool IsNonAction(ICustomAttributeProvider action) {
         return action.IsDefined(typeof(NonActionAttribute), inherit: true);
      }
   }
}
