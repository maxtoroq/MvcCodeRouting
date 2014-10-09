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
using System.Linq;
using System.Reflection;
using System.Text;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {

   class ReflectedMvcAsyncActionInfo : ReflectedMvcActionInfo {

      readonly MethodInfo method;

      public ReflectedMvcAsyncActionInfo(MethodInfo method, ControllerInfo controller) 
         : base(method, controller) {

         if (!IsAsyncMethod(method)) throw new InvalidOperationException();

         this.method = method;
      }

      protected override string GetName() {

         string name = base.GetName();

         if (name != this.method.Name) {
            return name;
         }

         return AsyncMethodActionName(this.method);
      }

      internal static bool IsAsyncMethod(MethodInfo method) {
         return method.Name.EndsWith("Async", StringComparison.OrdinalIgnoreCase);
      }

      internal static string AsyncMethodActionName(MethodInfo method) {
         return method.Name.Substring(0, method.Name.Length - "Async".Length);
      }
   }
}
