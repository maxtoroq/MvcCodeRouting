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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace MvcCodeRouting {
   
   abstract class ActionParameterInfo : ICustomAttributeProvider {

      public ActionInfo Action { get; private set; }
      public abstract string Name { get; }
      public abstract Type Type { get; }
      public abstract bool IsOptional { get; }

      public bool IsNullableValueType { 
         get {
            return Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>);
         }
      }

      public ActionParameterInfo(ActionInfo action) {
         this.Action = action;
      }

      public abstract object[] GetCustomAttributes(bool inherit);
      public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
      public abstract bool IsDefined(Type attributeType, bool inherit);
   }
}
