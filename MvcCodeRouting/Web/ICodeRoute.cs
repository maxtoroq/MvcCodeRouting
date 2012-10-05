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

namespace MvcCodeRouting.Web {
   
   interface ICodeRoute {

      IDictionary<string, string> ControllerMapping { get; set; }
      IDictionary<string, string> ActionMapping { get; set; }
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

         if (values == null)
            return getVirtualPath();

         string originalAction = null;

         bool cleanupAction = route.ActionMapping != null
            && SetAction(route, values, out originalAction);

         string originalController;
         bool abort;

         bool cleanupRouteContext = SetRouteContext(values, requestRouteValues, requestRouteDataTokens, out originalController, out abort);

         string controller = null;

         bool cleanupController = route.ControllerMapping != null
            && SetController(route, values, out controller);

         TVirtualPathData virtualPath = (!abort) ?
            getVirtualPath()
            : null;

         if (cleanupAction)
            CleanupAction(values, originalAction);

         if (cleanupRouteContext)
            CleanupRouteContext(virtualPath, values, originalController);

         if (cleanupController && !cleanupRouteContext)
            CleanupController(values, controller);

         return virtualPath;
      }

      static bool SetAction(this ICodeRoute route, IDictionary<string, object> values, out string originalAction) {

         originalAction = values["action"] as string;

         if (originalAction == null)
            return false;

         values["action"] = route.ActionMapping.ContainsKey(originalAction) ?
            route.ActionMapping[originalAction]
            : originalAction;

         return true;
      }

      static bool SetRouteContext(IDictionary<string, object> values, IDictionary<string, object> requestRouteValues, IDictionary<string, object> requestRouteDataTokens, out string originalController, out bool abort) {

         string currentRouteContext;
         abort = false;

         if ((originalController = values["controller"] as string) == null
            || (currentRouteContext = requestRouteDataTokens[DataTokenKeys.RouteContext] as string) == null
            || values.ContainsKey(CodeRoutingConstraint.Key))
            return false;

         string routeContext = currentRouteContext;

         if (originalController.Length > 0) {

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

                  if (!String.IsNullOrEmpty(baseRoute))
                     routeContextSegments.AddRange(baseRoute.Split('/'));

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
                  abort = true;
                  return false;
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
         }

         values[CodeRoutingConstraint.Key] = routeContext;

         return true;
      }

      static bool SetController(this ICodeRoute route, IDictionary<string, object> values, out string originalController) {

         originalController = values["controller"] as string;

         if (originalController == null)
            return false;

         values["controller"] = route.ControllerMapping.ContainsKey(originalController) ?
            route.ControllerMapping[originalController]
            : originalController;

         return true;
      }

      static void CleanupAction(IDictionary<string, object> values, string originalAction) {
         values["action"] = originalAction;
      }

      static void CleanupRouteContext(object virtualPath, IDictionary<string, object> values, string originalController) {

         // See issue #291
         // When the route matches don't change the controller back to it's original value
         // because it's used by ASP.NET MVC on child requests (e.g. Html.Action())
         // to locate the controller (DefaultControllerFactory).
         if (virtualPath == null)
            values["controller"] = originalController;

         values.Remove(CodeRoutingConstraint.Key);
      }

      static void CleanupController(IDictionary<string, object> values, string originalController) {
         values["controller"] = originalController;
      }

      public static bool RouteContextConstraint(bool urlGeneration, IDictionary<string, object> values, IDictionary<string, object> routeDataTokens) {

         if (!urlGeneration)
            return true;

         string valuesContext = values[CodeRoutingConstraint.Key] as string ?? "";
         string routeContext = routeDataTokens[DataTokenKeys.RouteContext] as string ?? "";

         return String.Equals(valuesContext, routeContext, StringComparison.OrdinalIgnoreCase);
      }
   }
}
