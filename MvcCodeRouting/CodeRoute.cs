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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace MvcCodeRouting {
   
   [DebuggerDisplay("{Url}")]
   class CodeRoute : Route {

      public IDictionary<string, string> ControllerMapping { get; set; }
      public IDictionary<string, string> ActionMapping { get; set; }
      public IList<string> NonActionParameterTokens { get; set; }

      public CodeRoute(string url, IRouteHandler routeHandler)
         : base(url, routeHandler) { }

      public override RouteData GetRouteData(HttpContextBase httpContext) {

         RouteData data = base.GetRouteData(httpContext);

         if (data != null) {
            
            RouteValueDictionary values = data.Values;

            if (this.ControllerMapping != null) {
               string controller = values["controller"] as string;

               if (controller != null) {

                  values["controller"] = this.ControllerMapping
                     .Single(p => String.Equals(p.Value, controller, StringComparison.OrdinalIgnoreCase))
                     .Key;
               }
            }

            if (this.ActionMapping != null) {
               string action = values["action"] as string;

               if (action != null) {

                  values["action"] = this.ActionMapping
                     .Single(p => String.Equals(p.Value, action, StringComparison.OrdinalIgnoreCase))
                     .Key;
               }  
            }
         }

         return data;
      }

      public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {

         if (values == null)
            return base.GetVirtualPath(requestContext, values);

         string originalAction = null;

         bool cleanupAction = this.ActionMapping != null
            && SetAction(values, out originalAction);

         string originalController;
         bool abort;

         bool cleanupRouteContext = SetRouteContext(values, requestContext.RouteData, out originalController, out abort);

         string controller = null;

         bool cleanupController = this.ControllerMapping != null
            && SetController(values, out controller);

         VirtualPathData virtualPath = (!abort)? 
            base.GetVirtualPath(requestContext, values)
            : null;

         if (cleanupAction)
            CleanupAction(values, originalAction);

         if (cleanupRouteContext) 
            CleanupRouteContext(virtualPath, values, originalController);

         if (cleanupController && !cleanupRouteContext)
            CleanupController(values, controller);

         return virtualPath;
      }

      bool SetAction(RouteValueDictionary values, out string originalAction) { 

         originalAction = values["action"] as string;

         if (originalAction == null)
            return false;

         values["action"] = this.ActionMapping.ContainsKey(originalAction) ?
            this.ActionMapping[originalAction]
            : originalAction;

         return true;
      }

      static void CleanupAction(RouteValueDictionary values, string originalAction) {
         values["action"] = originalAction;
      }

      static bool SetRouteContext(RouteValueDictionary values, RouteData routeData, out string originalController, out bool abort) {

         string currentRouteContext;
         abort = false;

         if ((originalController = values["controller"] as string) == null
            || (currentRouteContext = routeData.DataTokens[DataTokenKeys.RouteContext] as string) == null
            || values.ContainsKey(CodeRoutingConstraint.Key))
            return false;

         string routeContext = currentRouteContext;

         if (originalController.Length > 0) {

            StringBuilder theController = new StringBuilder(originalController);

            List<string> routeContextSegments = (routeContext.Length > 0) ?
               routeContext.Split('/').ToList()
               : new List<string>();

            if (theController[0] == '~') {

               routeContextSegments.Clear();

               if (theController.Length > 1
                  && theController[1] == '~') {

                  theController.Remove(0, 2);

               } else {

                  string baseRoute = routeData.DataTokens[DataTokenKeys.BaseRoute] as string;

                  if (!String.IsNullOrEmpty(baseRoute))
                     routeContextSegments.AddRange(baseRoute.Split('/'));

                  theController.Remove(0, 1);
               }

            } else if (theController[0] == '+') {

               string currentController = (string)routeData.Values["controller"];
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

      static void CleanupRouteContext(VirtualPathData virtualPath, RouteValueDictionary values, string originalController) {

         // See issue #291
         // When the route matches don't change the controller back to it's original value
         // because it's used by ASP.NET MVC on child requests (e.g. Html.Action())
         // to locate the controller (DefaultControllerFactory).
         if (virtualPath == null)
            values["controller"] = originalController;

         values.Remove(CodeRoutingConstraint.Key);
      }

      bool SetController(RouteValueDictionary values, out string originalController) {

         originalController = values["controller"] as string;
         
         if (originalController == null)
            return false;

         values["controller"] = this.ControllerMapping.ContainsKey(originalController) ?
            this.ControllerMapping[originalController]
            : originalController;

         return true;
      }

      static void CleanupController(RouteValueDictionary values, string originalController) {
         values["controller"] = originalController;
      }
   }
}
