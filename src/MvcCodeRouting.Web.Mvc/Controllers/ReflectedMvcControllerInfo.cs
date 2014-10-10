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
using System.Web.Mvc;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {
   
   class ReflectedMvcControllerInfo : MvcControllerInfo {

      public ReflectedMvcControllerInfo(Type type, RegisterSettings registerSettings, CodeRoutingProvider provider) 
         : base(type, registerSettings, provider) { }

      protected internal override ActionInfo[] GetActions() {

         bool controllerIsAsync = typeof(AsyncController).IsAssignableFrom(this.Type);

         List<MethodInfo> methods = GetCanonicalActionMethods().ToList();

         if (controllerIsAsync) { 

            MethodInfo[] asyncMethods = methods.Where(m => ReflectedMvcAsyncActionInfo.IsAsyncMethod(m)).ToArray();

            MethodInfo[] completedMethods =
               (from method in asyncMethods
                let actionName = ReflectedMvcAsyncActionInfo.AsyncMethodActionName(method)
                select methods.Where(m => m.Name.Equals(actionName + "Completed", StringComparison.OrdinalIgnoreCase)))
                .SelectMany(p => p)
                .ToArray();

            foreach (MethodInfo item in completedMethods) {
               methods.Remove(item);
            }
         }

         return
            (from m in methods
             let info = (controllerIsAsync && ReflectedMvcAsyncActionInfo.IsAsyncMethod(m)) ?
                new ReflectedMvcAsyncActionInfo(m, this)
                : new ReflectedMvcActionInfo(m, this)
             select info)
            .ToArray();
      }

      IEnumerable<MethodInfo> GetCanonicalActionMethods() {

         bool controllerIsDisposable = typeof(IDisposable).IsAssignableFrom(this.Type);

         return
             from m in this.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
             where MvcControllerInfo.IsMvcController(m.DeclaringType)
                && ActionInfo.IsCallableActionMethod(m)
                && !IsNonAction(m)
                && !(controllerIsDisposable && m.Name == "Dispose" && m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
             select m;
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.Type.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.Type.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.Type.IsDefined(attributeType, inherit);
      }
   }
}
