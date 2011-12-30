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
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {
   
   class CodeRoute : Route {

      static readonly Regex TokenPattern = new Regex(@"\{(.+?)\}");

      public Collection<string> NonActionParameterTokens { get; private set; }

      internal static CodeRoute Create(IEnumerable<ActionInfo> actions) {

         if (actions == null) throw new ArgumentNullException("actions");

         ActionInfo first = actions.First();
         var controllerNames = actions.Select(a => a.Controller.Name).Distinct().ToList();
         var actionNames = actions.Select(a => a.ActionSegment).Distinct().ToList();

         bool hardcodeController = controllerNames.Count == 1;
         bool hardcodeAction = actionNames.Count == 1
            && !first.IsDefaultAction;

         List<string> segments = new List<string>();
         segments.Add(first.Controller.UrlTemplate);

         if (hardcodeController)
            segments[0] = first.Controller.ControllerUrl;

         if (first.CustomRoute != null) {
            segments.Add(first.CustomRoute);

         } else {

            segments.Add("{action}");

            if (hardcodeAction)
               segments[1] = first.ActionSegment;

            segments.AddRange(first.RouteParameters.Select(r => r.RouteSegment));
         }

         string url = String.Join("/", segments.Where(s => !String.IsNullOrEmpty(s)));

         var defaults = new RouteValueDictionary();

         if (controllerNames.Count == 1)
            defaults.Add("controller", controllerNames.First());

         string defaultAction = null;

         if (actionNames.Count == 1) {
            defaultAction = first.ActionSegment;
         } else {
            var defAction = actions.FirstOrDefault(a => a.IsDefaultAction);

            if (defAction != null)
               defaultAction = defAction.ActionSegment;
         }

         if (defaultAction != null)
            defaults.Add("action", defaultAction);

         var parameters = first.RouteParameters;

         foreach (var param in parameters.Where(p => p.IsOptional))
            defaults.Add(param.Name, UrlParameter.Optional);

         var constraints = new RouteValueDictionary();

         if (!hardcodeController) 
            constraints.Add("controller", String.Join("|", actions.Select(a => a.Controller.ControllerSegment).Distinct()));

         if (!hardcodeAction || actionNames.Count > 1)
            constraints.Add("action", String.Join("|", actionNames));

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

         return new CodeRoute(url, nonActionParameterTokens.ToArray()) { 
            Constraints = constraints,
            DataTokens = dataTokens,
            Defaults = defaults
         };
      }

      private CodeRoute(string url, string[] nonActionParameterTokens)
         : base(url, new MvcRouteHandler()) {

         this.NonActionParameterTokens = new Collection<string>(nonActionParameterTokens);
      }

      public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {

         string controller;
         string currentRouteContext;
         RouteData routeData = requestContext.RouteData;

         if (values != null
            && (controller = values["controller"] as string) != null
            && (currentRouteContext = routeData.DataTokens[DataTokenKeys.RouteContext] as string) != null) {

            string routeContext = currentRouteContext;
            bool valuesHasRouteContext = values.ContainsKey(CodeRoutingConstraint.Key);

            if (controller.Length > 0 && !valuesHasRouteContext) {

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

                  if (routeContextSegments.Count == 0)
                     return null;

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

            if (!valuesHasRouteContext)
               values[CodeRoutingConstraint.Key] = routeContext;

            VirtualPathData virtualPath = base.GetVirtualPath(requestContext, values);

            // See issue #291
            if (virtualPath == null)
               values["controller"] = controller;

            if (!valuesHasRouteContext)
               values.Remove(CodeRoutingConstraint.Key);

            return virtualPath;
         }

         return base.GetVirtualPath(requestContext, values);
      }
   }
}
