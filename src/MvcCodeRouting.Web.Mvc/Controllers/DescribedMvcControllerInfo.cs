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
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {
   
   class DescribedMvcControllerInfo : MvcControllerInfo {

      readonly ControllerDescriptor controllerDescr;
      string _Name;

      public override string Name {
         get {
            return _Name
               ?? (_Name = controllerDescr.ControllerName);
         }
      }

      public DescribedMvcControllerInfo(ControllerDescriptor controllerDescr, Type type, RegisterSettings registerSettings, CodeRoutingProvider provider) 
         : base(type, registerSettings, provider) {

         this.controllerDescr = controllerDescr;
      }

      protected internal override ActionInfo[] GetActions() {

         ActionInfo[] actions = 
            (from a in this.controllerDescr.GetCanonicalActions()
             let asyncActionDescr = a as ReflectedAsyncActionDescriptor
             select (asyncActionDescr != null) ?
               new DescribedMvcAsyncActionInfo(asyncActionDescr, this)
               : new DescribedMvcActionInfo(a, this))
            .ToArray();

         if (actions.Length == 0) {
            
            // ReflectedAsyncControllerDescriptor.GetCanonicalActions always returns
            // an empty array

            return new ReflectedMvcControllerInfo(this.Type, this.Register, this.Provider).GetActions();
         }

         return actions;
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.controllerDescr.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.controllerDescr.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.controllerDescr.IsDefined(attributeType, inherit);
      }
   }
}
