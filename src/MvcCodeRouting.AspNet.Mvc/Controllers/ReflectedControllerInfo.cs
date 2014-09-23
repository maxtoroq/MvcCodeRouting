// Copyright 2014 Max Toro Q.
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
using System.Reflection;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ReflectedModelBuilder;

namespace MvcCodeRouting.Controllers {
   
   class ReflectedControllerInfo : ControllerInfo {

      readonly ReflectedControllerModel controllerModel;

      public override string Name {
         get {
            return controllerModel.ControllerName;
         }
      }

      public ReflectedControllerInfo(ReflectedControllerModel controllerModel, Type type, RegisterSettings registerSettings, CodeRoutingProvider provider) 
         : base(type, registerSettings, provider) {

         this.controllerModel = controllerModel;
      }

      protected internal override ActionInfo[] GetActions() {

         return this.controllerModel.Actions
            .Select(a => new ReflectedActionInfo(a, this))
            .ToArray();
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.controllerModel.ControllerType.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.controllerModel.ControllerType.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.controllerModel.ControllerType.IsDefined(attributeType, inherit);
      }

      protected override bool IsNonAction(ICustomAttributeProvider action) {
         return action.IsDefined(typeof(NonActionAttribute), inherit: true);
      }
   }
}
