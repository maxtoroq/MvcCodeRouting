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
using System.Web.Mvc;

namespace MvcCodeRouting {
   
   class ReflectedControllerInfo : ControllerInfo {

      public ReflectedControllerInfo(Type type, RegisterInfo registerInfo) 
         : base(type, registerInfo) { }

      protected internal override ActionInfo[] GetActions() {

         bool controllerIsAsync = typeof(AsyncController).IsAssignableFrom(this.Type);

         List<MethodInfo> methods = GetCanonicalActionMethods().ToList();

         if (controllerIsAsync) { 
            MethodInfo[] asyncMethods = methods.Where(m => ReflectedAsyncActionInfo.IsAsyncMethod(m)).ToArray();

            MethodInfo[] completedMethods =
               (from method in asyncMethods
                let actionName = ReflectedAsyncActionInfo.AsyncMethodActionName(method)
                select methods.Where(m => m.Name.Equals(actionName + "Completed", StringComparison.OrdinalIgnoreCase)))
                .SelectMany(p => p)
                .ToArray();

            foreach (MethodInfo item in completedMethods) 
               methods.Remove(item);
         }

         return
            (from m in methods
             let info = (controllerIsAsync && ReflectedAsyncActionInfo.IsAsyncMethod(m)) ?
                new ReflectedAsyncActionInfo(m, this)
                : new ReflectedActionInfo(m, this)
             select info).ToArray();
      }
   }
}
