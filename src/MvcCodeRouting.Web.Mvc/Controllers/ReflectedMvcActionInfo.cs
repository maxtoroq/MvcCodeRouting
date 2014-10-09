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
using System.Reflection;
using System.Web.Mvc;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {

   class ReflectedMvcActionInfo : MvcActionInfo {

      readonly MethodInfo method;

      public override string MethodName {
         get { return method.Name; }
      }

      public override Type DeclaringType {
         get { return method.DeclaringType; }
      }

      public ReflectedMvcActionInfo(MethodInfo method, ControllerInfo controller)
         : base(controller) {

         this.method = method;
      }

      protected override string GetName() {

         return base.GetName() 
            ?? this.method.Name;
      }

      protected override ActionParameterInfo[] GetParameters() {

         return this.method.GetParameters()
            .Select(p => new ReflectedMvcActionParameterInfo(p, this))
            .ToArray();
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.method.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.method.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.method.IsDefined(attributeType, inherit);
      }
   }
}
