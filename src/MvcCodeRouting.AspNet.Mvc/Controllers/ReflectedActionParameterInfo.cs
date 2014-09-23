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
using Microsoft.AspNet.Mvc.ReflectedModelBuilder;

namespace MvcCodeRouting.Controllers {
   
   class ReflectedActionParameterInfo : ActionParameterInfo {

      readonly ReflectedParameterModel parameterModel;

      public override string Name {
         get { return parameterModel.ParameterName; }
      }

      public override Type Type {
         get { return parameterModel.ParameterInfo.ParameterType; }
      }

      public override bool IsOptional {
         get {
            return parameterModel.IsOptional
               || IsNullableValueType;
         }
      }

      public ReflectedActionParameterInfo(ReflectedParameterModel parameterModel, ActionInfo action) 
         : base(action) {

         this.parameterModel = parameterModel;
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.parameterModel.ParameterInfo.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.parameterModel.ParameterInfo.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.parameterModel.ParameterInfo.IsDefined(attributeType, inherit);
      }
   }
}
