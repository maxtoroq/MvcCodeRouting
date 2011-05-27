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
using System.Reflection;
using System.Web.Routing;

namespace MvcCodeRouting {
   
   [AttributeUsage(AttributeTargets.Method)]
   public sealed class RequireRouteParametersAttribute : ActionMethodSelectorAttribute {
      
      public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
         
         var routeValues = new RouteValueDictionary(controllerContext.RouteData.Values);
         routeValues.Remove("controller");
         routeValues.Remove("action");

         CodeRoute codeRoute = controllerContext.RouteData.Route as CodeRoute;

         if (codeRoute != null && codeRoute.BaseRouteTokens.Count > 0) {
            for (int i = 0; i < codeRoute.BaseRouteTokens.Count; i++) 
               routeValues.Remove(codeRoute.BaseRouteTokens[i]);
         }

         var parameters =
            (from p in methodInfo.GetParameters()
             where Attribute.IsDefined(p, typeof(FromRouteAttribute))
             select p.Name).ToArray();

         return parameters.All(p => routeValues.Keys.Contains(p, StringComparer.OrdinalIgnoreCase))
            && routeValues.Keys.All(k => parameters.Contains(k, StringComparer.OrdinalIgnoreCase));
      }
   }
}
