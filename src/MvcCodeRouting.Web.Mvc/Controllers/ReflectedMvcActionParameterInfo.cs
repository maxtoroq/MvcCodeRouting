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
using System.ComponentModel;
using System.Reflection;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {

   class ReflectedMvcActionParameterInfo : ActionParameterInfo {

      readonly ParameterInfo paramInfo;

      public override string Name {
         get { return paramInfo.Name; }
      }

      public override Type Type {
         get { return paramInfo.ParameterType; }
      }

      public override bool IsOptional {
         get {
            return paramInfo.IsOptional
               || IsDefined(typeof(DefaultValueAttribute), inherit: false)
               || IsNullableValueType;
         }
      }

      public ReflectedMvcActionParameterInfo(ParameterInfo paramInfo, ActionInfo action)
         : base(action) {

         this.paramInfo = paramInfo;
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.paramInfo.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.paramInfo.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.paramInfo.IsDefined(attributeType, inherit);
      }
   }
}
