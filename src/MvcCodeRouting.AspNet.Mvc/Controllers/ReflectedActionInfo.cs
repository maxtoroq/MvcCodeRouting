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
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ReflectedModelBuilder;

namespace MvcCodeRouting.Controllers {
   
   class ReflectedActionInfo : ActionInfo {

      readonly ReflectedActionModel actionModel;

      public override string MethodName {
         get {
            return actionModel.ActionMethod.Name;
         }
      }

      public override Type DeclaringType {
         get {
            return actionModel.ActionMethod.DeclaringType;
         }
      }

      public ReflectedActionInfo(ReflectedActionModel actionModel, ControllerInfo controller) 
         : base(controller) {

         this.actionModel = actionModel;
      }

      protected override ActionParameterInfo[] GetParameters() {
         
         return this.actionModel.Parameters
            .Select(p => new ReflectedActionParameterInfo(p, this))
            .ToArray();
      }

      protected override string GetName() {

         if (GetCustomAttributes(typeof(IActionHttpMethodProvider), inherit: true).Length > 0) {
            return this.actionModel.ActionName;
         }

         return "";
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.actionModel.ActionMethod.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.actionModel.ActionMethod.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.actionModel.ActionMethod.IsDefined(attributeType, inherit);
      }
   }
}
