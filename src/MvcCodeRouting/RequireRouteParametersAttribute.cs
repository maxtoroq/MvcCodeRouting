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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting.Web.Routing;

namespace MvcCodeRouting {
   
   /// <summary>
   /// An <see cref="ActionMethodSelectorAttribute"/> for overloaded action methods, used 
   /// to help the ASP.NET MVC runtime disambiguate and choose the appropriate overload.
   /// </summary>
   [Obsolete("Please use MvcCodeRouting.Web.Mvc.RequireRouteParametersAttribute instead.")]
   [EditorBrowsable(EditorBrowsableState.Never)]
   [AttributeUsage(AttributeTargets.Method)]
   public class RequireRouteParametersAttribute : ActionMethodSelectorAttribute {

      static readonly ConcurrentDictionary<MethodInfo, string[]> actionDataCache = new ConcurrentDictionary<MethodInfo, string[]>();

      /// <summary>
      /// Determines whether the action method selection is valid for the specified
      /// controller context.
      /// </summary>
      /// <param name="controllerContext">The controller context.</param>
      /// <param name="methodInfo">Information about the action method.</param>
      /// <returns>
      /// true if the <see cref="ControllerContext.RouteData"/> has values for
      /// all parameters decorated with <see cref="FromRouteAttribute"/>, and if all keys
      /// in <see cref="ControllerContext.RouteData"/> match any of the decorated parameters,
      /// excluding controller, action and other route parameters that do not map to action method parameters.
      /// </returns>
      public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
         
         var routeValues = new RouteValueDictionary(controllerContext.RouteData.Values);
         routeValues.Remove("controller");
         routeValues.Remove("action");

         CodeRoute codeRoute = controllerContext.RouteData.Route as CodeRoute;

         if (codeRoute != null) {

            for (int i = 0; i < codeRoute.NonActionParameterTokens.Count; i++) {
               routeValues.Remove(codeRoute.NonActionParameterTokens[i]);
            }
         }

#pragma warning disable 0618

         string[] parameters = actionDataCache.GetOrAdd(methodInfo, (m) =>
            (from p in m.GetParameters()
             where p.IsDefined(typeof(MvcCodeRouting.FromRouteAttribute), inherit: true)
             select p.Name).ToArray()
         );

#pragma warning restore 0618

         return parameters.All(p => routeValues.Keys.Contains(p, StringComparer.OrdinalIgnoreCase))
            && routeValues.Keys.All(k => parameters.Contains(k, StringComparer.OrdinalIgnoreCase));
      }
   }
}
