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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MvcCodeRouting {

   [DebuggerDisplay("{ActionUrl}")]
   abstract class ActionInfo : ICustomAttributeProvider {

      string _Name;
      Collection<ActionParameterInfo> _Parameters;
      RouteParameterInfoCollection _RouteParameters;

      public ControllerInfo Controller { get; private set; }
      public abstract string OriginalName { get; }
      public virtual string MethodName { get { return OriginalName; } }
      public virtual Type DeclaringType { get { return Controller.Type; } }

      public string Name {
         get {
            if (_Name == null) {
               _Name = Controller.RegisterInfo.Settings.RouteFormatter(OriginalName, RouteSegmentType.Action);
               CodeRoutingSettings.CheckCaseFormattingOnly(OriginalName, _Name, RouteSegmentType.Action);
            }
            return _Name;
         }
      }

      public Collection<ActionParameterInfo> Parameters {
         get {
            if (_Parameters == null) 
               _Parameters = new Collection<ActionParameterInfo>(GetParameters());
            return _Parameters;
         }
      }

      public RouteParameterInfoCollection RouteParameters {
         get {
            if (_RouteParameters == null) {
               _RouteParameters = new RouteParameterInfoCollection(
                  (from p in Parameters
                   where p.IsDefined(typeof(FromRouteAttribute), inherit: true)
                   select new RouteParameterInfo(p))
                  .ToList()
               );

               CheckCatchAllParamIsLast(_RouteParameters);
            }
            return _RouteParameters;
         }
      }

      public bool IsDefaultAction {
         get {
            return RouteParameters.Count == 0
               && Controller.DefaultActionName != null
               && NameEquals(Name, Controller.DefaultActionName);
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

      public bool HasRequireRouteParametersAttribute {
         get { return IsDefined(typeof(RequireRouteParametersAttribute), inherit: true); }
      }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      public ActionInfo(ControllerInfo controller) {
         this.Controller = controller;
      }

      void CheckCatchAllParamIsLast(IList<RouteParameterInfo> parameters) {

         for (int i = 0; i < parameters.Count; i++) {

            var param = parameters[i];

            if (param.IsCatchAll && i < parameters.Count - 1)
               throw new InvalidOperationException(
                  String.Format(CultureInfo.InvariantCulture,
                     "A catch-all parameter must be the last route parameter of the action method (parameter {0} on {1}).",
                     param.Name,
                     String.Concat(this.DeclaringType.FullName, ".", this.MethodName, "(", String.Join(", ", this.Parameters.Select(p => p.Type.Name)), ")")
                  )
               );
         }
      }

      protected abstract ActionParameterInfo[] GetParameters();
      public abstract object[] GetCustomAttributes(bool inherit);
      public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
      public abstract bool IsDefined(Type attributeType, bool inherit);
   }
}
