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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {
   
   class CodeRoute : Route {

      static readonly Regex TokenPattern = new Regex(@"\{(.+?)\}");

      readonly IDictionary<string, string> actionMapping;

      public Collection<string> NonActionParameterTokens { get; private set; }

      internal static CodeRoute Create(IEnumerable<ActionInfo> actions) {

         if (actions == null) throw new ArgumentNullException("actions");

         ActionInfo first = actions.First();

         var controllerMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         foreach (string name in actions.Select(a => a.Controller.Name).Distinct(controllerMapping.Comparer)) 
            controllerMapping.Add(name, actions.First(a => ControllerInfo.NameEquals(a.Controller.Name, name)).Controller.ControllerSegment);

         ICollection<string> controllerNames = controllerMapping.Keys;
         ICollection<string> controllerSegments = controllerMapping.Values;
         bool hardcodeController = controllerNames.Count == 1;

         var segments = new List<string> { 
            (hardcodeController ? 
               first.Controller.ControllerUrl 
               : first.Controller.UrlTemplate)
         };

         var actionMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         foreach (string name in actions.Select(a => a.Name).Distinct(actionMapping.Comparer))
            actionMapping.Add(name, actions.First(a => ActionInfo.NameEquals(a.Name, name)).ActionSegment);

         bool actionFormat = actionMapping.Any(p => !String.Equals(p.Key, p.Value, StringComparison.Ordinal));
         bool hardcodeAction = actionMapping.Count == 1 && !first.IsDefaultAction;
         bool actionFormatToken = actionFormat && !hardcodeAction;
         string actionToken = (actionFormatToken) ? "__action" : "action";

         if (first.CustomRoute != null) {
            segments.Add(first.CustomRoute);

         } else {

            segments.Add(hardcodeAction ? first.ActionSegment : String.Concat("{", actionToken, "}"));
            segments.AddRange(first.RouteParameters.Select(r => r.RouteSegment));
         }

         string url = String.Join("/", segments.Where(s => !String.IsNullOrEmpty(s)));

         var defaults = new RouteValueDictionary();

         if (controllerNames.Count == 1)
            defaults.Add("controller", controllerNames.First());

         string defaultAction = null;

         if (actionMapping.Count == 1) {
            defaultAction = (actionFormatToken) ?
               first.ActionSegment
               : first.Name;

         } else {
            ActionInfo defAction = actions.FirstOrDefault(a => a.IsDefaultAction);

            if (defAction != null)
               defaultAction = (actionFormatToken) ?
                  defAction.ActionSegment
                  : defAction.Name;
         }

         if (defaultAction != null)
            defaults.Add(actionToken, defaultAction);

         TokenInfoCollection parameters = first.RouteParameters;

         foreach (var param in parameters.Where(p => p.IsOptional))
            defaults.Add(param.Name, UrlParameter.Optional);

         var constraints = new RouteValueDictionary();

         if (!hardcodeController)
            constraints.Add("controller", String.Join("|", controllerSegments));

         if (!hardcodeAction)
            constraints.Add(actionToken, String.Join("|", actionMapping.Values));

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

         return new CodeRoute(url, actionFormatToken ? actionMapping : null, nonActionParameterTokens.ToArray()) { 
            Constraints = constraints,
            DataTokens = dataTokens,
            Defaults = defaults
         };
      }

      private CodeRoute(string url, IDictionary<string, string> actionMapping, string[] nonActionParameterTokens)
         : base(url, new MvcRouteHandler()) {

         this.actionMapping = actionMapping;
         this.NonActionParameterTokens = new Collection<string>(nonActionParameterTokens);
      }

      public override RouteData GetRouteData(HttpContextBase httpContext) {

         RouteData data = base.GetRouteData(httpContext);

         if (data != null) {
            
            RouteValueDictionary values = data.Values;
            string action = values["action"] as string;
            string __action = values["__action"] as string;

            if (action == null
               && __action != null) {

               values["action"] = this.actionMapping
                  .Single(p => String.Equals(p.Value, __action, StringComparison.OrdinalIgnoreCase))
                  .Key;
            } 
         }

         return data;
      }

      public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {

         if (values == null)
            return base.GetVirtualPath(requestContext, values);

         string action = null;

         bool cleanupAction = this.actionMapping != null
            && SetAction(values, out action);

         string controller;
         bool abort;

         bool cleanupRouteContext = SetRouteContext(values, requestContext.RouteData, out controller, out abort);

         VirtualPathData virtualPath = (!abort)? 
            base.GetVirtualPath(requestContext, values)
            : null;

         if (cleanupAction) {
            
            values.Remove("__action");
            values["action"] = action;
         }

         if (cleanupRouteContext) {

            // See issue #291
            if (virtualPath == null)
               values["controller"] = controller;

            values.Remove(CodeRoutingConstraint.Key);
         }

         return virtualPath;
      }

      bool SetAction(RouteValueDictionary values, out string action) { 

         action = values["action"] as string;

         if (action == null)
            return false;

         values["__action"] = this.actionMapping.ContainsKey(action) ?
            this.actionMapping[action]
            : action;

         values.Remove("action");

         return true;
      }

      bool SetRouteContext(RouteValueDictionary values, RouteData routeData, out string controller, out bool abort) {

         string currentRouteContext;
         abort = false;

         if ((controller = values["controller"] as string) == null
            || (currentRouteContext = routeData.DataTokens[DataTokenKeys.RouteContext] as string) == null
            || values.ContainsKey(CodeRoutingConstraint.Key))
            return false;

         string routeContext = currentRouteContext;

         if (controller.Length > 0) {

            StringBuilder theController = new StringBuilder(controller);

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
   }
}
