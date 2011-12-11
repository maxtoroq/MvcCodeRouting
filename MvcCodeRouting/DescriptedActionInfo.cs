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
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcCodeRouting {

   class DescriptedActionInfo : ActionInfo {

      readonly ActionDescriptor actionDescr;
      readonly ReflectedActionDescriptor reflectedActionDescr;
      string _Name;

      public override string Name {
         get {
            if (_Name == null) {
               ActionNameAttribute nameAttr = GetCustomAttributes(typeof(ActionNameAttribute), inherit: true)
                  .Cast<ActionNameAttribute>()
                  .SingleOrDefault();

               _Name = (nameAttr != null) ? 
                  nameAttr.Name : actionDescr.ActionName;
            }
            return _Name;
         }
      }

      public override string MethodName {
         get {
            return (reflectedActionDescr != null) ?
               reflectedActionDescr.MethodInfo.Name
               : base.MethodName;
         }
      }

      public override Type DeclaringType {
         get {
            return (reflectedActionDescr != null) ?
               reflectedActionDescr.MethodInfo.DeclaringType
               : base.DeclaringType;
         }
      }

      public DescriptedActionInfo(ActionDescriptor actionDescr, ControllerInfo controller)
         : base(controller) {

         this.actionDescr = actionDescr;
         this.reflectedActionDescr = actionDescr as ReflectedActionDescriptor;
      }

      protected override ActionParameterInfo[] GetParameters() {
         return this.actionDescr.GetParameters().Select(p => new DescriptedActionParameterInfo(p, this)).ToArray();
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.actionDescr.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.actionDescr.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.actionDescr.IsDefined(attributeType, inherit);
      }
   }
}
