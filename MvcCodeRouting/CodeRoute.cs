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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {
   
   [DebuggerDisplay("{Url}")]
   class CodeRoute : Route {

      static readonly Regex TokenPattern = new Regex(@"\{(.+?)\}");

      readonly IDictionary<string, string> controllerMapping;
      readonly IDictionary<string, string> actionMapping;

      public Collection<string> NonActionParameterTokens { get; private set; }

      internal static CodeRoute Create(IEnumerable<ActionInfo> actions) {

         if (actions == null) throw new ArgumentNullException("actions");

         ActionInfo first = actions.First();

         var controllerMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         foreach (string name in actions.Select(a => a.Controller.Name).Distinct(controllerMapping.Comparer)) 
            controllerMapping.Add(name, actions.First(a => ControllerInfo.NameEquals(a.Controller.Name, name)).Controller.ControllerSegment);

         bool hardcodeController = controllerMapping.Count == 1;
         bool controllerFormat = controllerMapping.Any(p => !String.Equals(p.Key, p.Value, StringComparison.Ordinal));
         bool requiresControllerMapping = controllerFormat && !hardcodeController;

         var segments = new List<string> { 
            (hardcodeController ? 
               first.Controller.ControllerUrl 
               : first.Controller.UrlTemplate)
         };

         var actionMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         foreach (string name in actions.Select(a => a.Name).Distinct(actionMapping.Comparer))
            actionMapping.Add(name, actions.First(a => ActionInfo.NameEquals(a.Name, name)).ActionSegment);

         bool includeActionToken = (first.CustomRoute != null) ? 
            first.CustomRouteHasActionToken
            : actionMapping.Count > 1 || first.IsDefaultAction;

         bool actionFormat = actionMapping.Any(p => !String.Equals(p.Key, p.Value, StringComparison.Ordinal));
         bool requiresActionMapping = actionFormat && includeActionToken;

         if (first.CustomRoute != null) {
            segments.Add(first.CustomRoute);
         } else {
            segments.Add(!includeActionToken ? first.ActionSegment : String.Concat("{action}"));
            segments.AddRange(first.RouteParameters.Select(r => r.RouteSegment));
         }

         string url = String.Join("/", segments.Where(s => !String.IsNullOrEmpty(s)));

         var defaults = new RouteValueDictionary();

         if (controllerMapping.Count == 1)
            defaults.Add("controller", controllerMapping.Keys.First());

         ActionInfo defaultAction = (includeActionToken) ?
            actions.FirstOrDefault(a => a.IsDefaultAction)
            : first;

         string actionDefault = null;

         if (defaultAction != null) { 
            actionDefault = (requiresActionMapping) ?
               defaultAction.ActionSegment
               : defaultAction.Name;
         }

         if (actionDefault != null)
            defaults.Add("action", actionDefault);

         TokenInfoCollection parameters = first.RouteParameters;

         foreach (var param in parameters.Where(p => p.IsOptional))
            defaults.Add(param.Name, UrlParameter.Optional);

         var constraints = new RouteValueDictionary();

         if (!hardcodeController)
            constraints.Add("controller", String.Join("|", controllerMapping.Values));

         if (includeActionToken)
            constraints.Add("action", String.Join("|", actionMapping.Values));

         foreach (var param in first.Controller.RouteProperties.Concat(parameters).Where(p => p.Constraint != null)) {

            string regex = param.Constraint;

            if (param.IsOptional)
               regex = String.Concat("(", regex, ")?");

            constraints.Add(param.Name, regex);
         }

         constraints.Add(CodeRoutingConstraint.Key, new CodeRoutingConstraint());

         var dataTokens = new RouteValueDictionary { 
            { DataTokenKeys.Namespaces, new string[1] { first.Controller.Namespace } },
            { DataTokenKeys.BaseRoute, first.Controller.Register.BaseRoute },
            { DataTokenKeys.RouteContext, String.Join("/", first.Controller.CodeRoutingContext) },
            { DataTokenKeys.ViewsLocation, String.Join("/", first.Controller.CodeRoutingContext.Where(s => !s.Contains('{'))) }
         };

         var nonActionParameterTokens = new List<string>();

         if (!String.IsNullOrEmpty(first.Controller.Register.BaseRoute)) 
            nonActionParameterTokens.AddRange(TokenPattern.Matches(first.Controller.Register.BaseRoute).Cast<Match>().Select(m => m.Groups[1].Value));

         nonActionParameterTokens.AddRange(first.Controller.RouteProperties.Select(p => p.Name));

         return new CodeRoute(
            url: url,
            controllerMapping: (requiresControllerMapping) ? controllerMapping : null,
            actionMapping: (requiresActionMapping) ? actionMapping : null,
            nonActionParameterTokens: nonActionParameterTokens.ToArray()) { 
            Constraints = constraints,
            DataTokens = dataTokens,
            Defaults = defaults
         };
      }

      private CodeRoute(string url, IDictionary<string, string> controllerMapping, IDictionary<string, string> actionMapping, string[] nonActionParameterTokens)
         : base(url, new MvcRouteHandler()) {

         this.controllerMapping = controllerMapping;
         this.actionMapping = actionMapping;
         this.NonActionParameterTokens = new Collection<string>(nonActionParameterTokens);
      }

      public override RouteData GetRouteData(HttpContextBase httpContext) {

         RouteData data = base.GetRouteData(httpContext);

         if (data != null) {
            
            RouteValueDictionary values = data.Values;

            if (this.controllerMapping != null) {
               string controller = values["controller"] as string;

               if (controller != null) {

                  values["controller"] = this.controllerMapping
                     .Single(p => String.Equals(p.Value, controller, StringComparison.OrdinalIgnoreCase))
                     .Key;
               }
            }

            if (this.actionMapping != null) {
               string action = values["action"] as string;

               if (action != null) {

                  values["action"] = this.actionMapping
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

         bool cleanupAction = this.actionMapping != null
            && SetAction(values, out originalAction);

         string originalController;
         bool abort;

         bool cleanupRouteContext = SetRouteContext(values, requestContext.RouteData, out originalController, out abort);

         string controller = null;

         bool cleanupController = this.controllerMapping != null
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

         values["action"] = this.actionMapping.ContainsKey(originalAction) ?
            this.actionMapping[originalAction]
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

         values["controller"] = this.controllerMapping.ContainsKey(originalController) ?
            this.controllerMapping[originalController]
            : originalController;

         return true;
      }

      static void CleanupController(RouteValueDictionary values, string originalController) {
         values["controller"] = originalController;
      }
   }
}
