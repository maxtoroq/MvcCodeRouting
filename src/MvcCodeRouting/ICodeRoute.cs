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
using System.Text;

namespace MvcCodeRouting {
   
   interface ICodeRoute {

      IDictionary<string, object> DataTokens { get; }
      IDictionary<string, string> ControllerMapping { get; }
      IDictionary<string, string> ActionMapping { get; }
   }

   static class CodeRouteExtensions {

      public static void IncomingMapping(this ICodeRoute route, IDictionary<string, object> values) {

         if (route.ControllerMapping != null) {

            string controller = values["controller"] as string;

            if (controller != null) {

               values["controller"] = route.ControllerMapping
                  .Single(p => String.Equals(p.Value, controller, StringComparison.OrdinalIgnoreCase))
                  .Key;
            }
         }

         if (route.ActionMapping != null) {

            string action = values["action"] as string;

            if (action != null) {

               values["action"] = route.ActionMapping
                  .Single(p => String.Equals(p.Value, action, StringComparison.OrdinalIgnoreCase))
                  .Key;
            }
         }
      }

      public static TVirtualPathData DoGetVirtualPath<TVirtualPathData>(this ICodeRoute route, IDictionary<string, object> values, IDictionary<string, object> requestRouteValues, IDictionary<string, object> requestRouteDataTokens, Func<TVirtualPathData> getVirtualPath) where TVirtualPathData : class {

         if (values == null) {
            return getVirtualPath();
         }

         string controllerRef;
         string valuesRouteContext = GetRouteContext(values, requestRouteValues, requestRouteDataTokens, out controllerRef);

         if (valuesRouteContext == null) {
            return null;
         }
         
         string thisRouteContext = (string)route.DataTokens[DataTokenKeys.RouteContext];

         if (!String.Equals(valuesRouteContext, thisRouteContext, StringComparison.OrdinalIgnoreCase)) {

            if (controllerRef != null) {
               values["controller"] = controllerRef;
            }

            return null;
         }

         string unmappedController = null;
         string unmappedAction = null;

         MapValue("controller", values, route.ControllerMapping, out unmappedController);
         bool actionWasMapped = MapValue("action", values, route.ActionMapping, out unmappedAction);

         TVirtualPathData virtualPath = getVirtualPath();

         // See issue #291
         // When the route matches don't change the controller back to its original value
         // because it's used by ASP.NET MVC on child requests (e.g. Html.Action())
         // to locate the controller (DefaultControllerFactory).
         
         if (virtualPath == null 
            && controllerRef != null) {

            values["controller"] = controllerRef;
         }

         if (actionWasMapped) {
            values["action"] = unmappedAction;
         }

         return virtualPath;
      }

      static string GetRouteContext(IDictionary<string, object> values, IDictionary<string, object> requestRouteValues, IDictionary<string, object> requestRouteDataTokens, out string originalController) {

         originalController = null;

         if (values.ContainsKey("controller")) {
            originalController = (string)values["controller"];
         }

         const string routeContextKey = "__routecontext";

         if (values.ContainsKey(routeContextKey)) {
            return (string)values[routeContextKey];
         }
         
         string routeContext = null;

         if (requestRouteDataTokens.ContainsKey(DataTokenKeys.RouteContext)) { 
            routeContext = (string)requestRouteDataTokens[DataTokenKeys.RouteContext];
         }

         if (routeContext == null) {
            return null;
         }

         if (String.IsNullOrEmpty(originalController)) {
            return routeContext;
         }

         var theController = new StringBuilder(originalController);

         List<string> routeContextSegments = (routeContext.Length > 0) ?
            routeContext.Split('/').ToList()
            : new List<string>();

         if (theController[0] == '~') {

            routeContextSegments.Clear();

            if (theController.Length > 1
               && theController[1] == '~') {

               theController.Remove(0, 2);

            } else {

               string baseRoute = requestRouteDataTokens[DataTokenKeys.BaseRoute] as string;

               if (!String.IsNullOrEmpty(baseRoute)) {
                  routeContextSegments.AddRange(baseRoute.Split('/'));
               }

               theController.Remove(0, 1);
            }

         } else if (theController[0] == '+') {

            string currentController = (string)requestRouteValues["controller"];
            routeContextSegments.Add(currentController);

            theController.Remove(0, 1);

         } else if (theController[0] == '.'
            && theController.Length > 1
            && theController[1] == '.') {

            if (routeContextSegments.Count == 0) {
               return null;
            }

            routeContextSegments.RemoveAt(routeContextSegments.Count - 1);
            theController.Remove(0, 2);
         }

         if (theController.Length > 1) {

            string[] controllerSegments = theController.ToString().Split('.');

            if (controllerSegments.Length > 1) {

               routeContextSegments.AddRange(controllerSegments.Take(controllerSegments.Length - 1));

               theController.Clear();
               theController.Append(controllerSegments.Last());
            }
         }

         routeContext = String.Join("/", routeContextSegments);

         values["controller"] = theController.ToString();

         return routeContext;
      }

      static bool MapValue(string name, IDictionary<string, object> values, IDictionary<string, string> mapping, out string originalValue) {

         object originalObject;

         if (!values.TryGetValue(name, out originalObject)) {
            originalValue = null;
            return false;
         }

         originalValue = originalObject as string;

         if (originalValue == null
            || mapping == null
            || !mapping.ContainsKey(originalValue)) {

            return false;
         }

         values[name] = mapping[originalValue];

         return true;
      }
   }
}
