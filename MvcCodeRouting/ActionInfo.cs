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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MvcCodeRouting {
   
   [DebuggerDisplay("{ActionUrl}")]
   class ActionInfo {

      readonly CodeRoutingSettings settings;
      string _Name;
      RouteParameterInfoCollection _RouteParameters;

      public MethodInfo Method { get; private set; }
      public ControllerInfo Controller { get; private set; }

      public string Name { 
         get {
            if (_Name == null) {
               ActionNameAttribute nameAttr = Attribute.GetCustomAttribute(Method, typeof(ActionNameAttribute)) as ActionNameAttribute;

               string actionName = (nameAttr != null) ? nameAttr.Name
                  : (settings.ActionNameExtractor != null) ? settings.ActionNameExtractor(Method)
                  : Method.Name;

               _Name = settings.RouteFormatter(actionName, RouteSegmentType.Action);
               settings.CheckCaseFormattingOnly(actionName, _Name, RouteSegmentType.Action);
            }
            return _Name;
         } 
      }

      public RouteParameterInfoCollection RouteParameters {
         get {
            if (_RouteParameters == null) {
               _RouteParameters = new RouteParameterInfoCollection(
                  Method.GetParameters()
                     .Where(p => Attribute.IsDefined(p, typeof(FromRouteAttribute)))
                     .Select(p => new RouteParameterInfo(p, settings.DefaultConstraints))
                     .ToList()
               );
            }
            return _RouteParameters;
         }
      }

      public bool HasRequireRouteParametersAttribute {
         get { return Attribute.IsDefined(Method, typeof(RequireRouteParametersAttribute)); }
      }

      public bool IsDefaultAction {
         get {
            return RouteParameters.Count == 0
               && Controller.DefaultActionName != null
               && NameEquals(Name, Controller.DefaultActionName);
         }
      }

      public string UrlTemplate {
         get {
            return String.Join("/",
               (new[] { Controller.UrlTemplate, "{action}" }).Where(s => !String.IsNullOrEmpty(s))
               .Concat(RouteParameters.Select(r => r.RouteSegment))
            );
         }
      }

      public string ActionUrl {
         get {
            return String.Join("/",
               (new[] { Controller.ControllerUrl, (!IsDefaultAction) ? Name : null })
               .Where(s => !String.IsNullOrEmpty(s))
            );
         }
      }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      public ActionInfo(MethodInfo method, ControllerInfo controller, CodeRoutingSettings settings) {

         this.Method = method;
         this.Controller = controller;
         this.settings = settings;

         CheckCatchAllParamIsLast(this.RouteParameters, this.Method);
      }

      void CheckCatchAllParamIsLast(IList<RouteParameterInfo> parameters, MethodInfo method) {

         for (int i = 0; i < parameters.Count; i++) {

            var param = parameters[i];

            if (param.IsCatchAll && i < parameters.Count - 1)
               throw new InvalidOperationException(
                  String.Format(CultureInfo.InvariantCulture,
                     "A catch-all parameter must be the last route parameter of the action method (parameter {0} on {1}).",
                     param.Name,
                     String.Concat(method.DeclaringType.FullName, ".", method.Name, "(", String.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name)), ")")
                  )
               );
         }
      }
   }
}
