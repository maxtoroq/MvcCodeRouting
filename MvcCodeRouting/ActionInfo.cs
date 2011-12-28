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

      string _ActionSegment;
      string _CustomRoute;
      bool _CustomRouteInit;
      Collection<ActionParameterInfo> _Parameters;
      TokenInfoCollection _RouteParameters;

      public ControllerInfo Controller { get; private set; }
      public abstract string Name { get; }
      public virtual string MethodName { get { return Name; } }
      public virtual Type DeclaringType { get { return Controller.Type; } }

      public string ActionSegment {
         get {
            if (_ActionSegment == null) 
               _ActionSegment = Controller.Register.Settings.FormatRouteSegment(new RouteFormatterArgs(Name, RouteSegmentType.Action, Controller.Type), caseOnly: true);
            return _ActionSegment;
         }
      }

      public Collection<ActionParameterInfo> Parameters {
         get {
            if (_Parameters == null) 
               _Parameters = new Collection<ActionParameterInfo>(GetParameters());
            return _Parameters;
         }
      }

      public TokenInfoCollection RouteParameters {
         get {
            if (_RouteParameters == null) {
               _RouteParameters = new TokenInfoCollection(
                  (from p in Parameters
                   where p.FromRouteAttribute != null
                   select CreateTokenInfo(p))
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
               && NameEquals(ActionSegment, Controller.DefaultActionName);
         }
      }

      public string ActionUrl {
         get {
            return String.Join("/",
               (new[] { Controller.ControllerUrl, (!IsDefaultAction) ? ActionSegment : null })
               .Where(s => !String.IsNullOrEmpty(s))
            );
         }
      }

      public string CustomRoute {
         get {
            if (!_CustomRouteInit) {

               var attr = GetCustomAttributes(typeof(CustomRouteAttribute), inherit: true)
                  .Cast<CustomRouteAttribute>()
                  .SingleOrDefault();

               if (attr != null) 
                  _CustomRoute = attr.Url;

               _CustomRouteInit = true;
            }
            return _CustomRoute;
         }
      }

      public bool HasRequireRouteParametersAttribute {
         get { return IsDefined(typeof(RequireRouteParametersAttribute), inherit: true); }
      }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      static TokenInfo CreateTokenInfo(ActionParameterInfo actionParam) {

         string name = actionParam.Name;
         bool isOptional = actionParam.IsOptional;

         var routeAttr = actionParam.FromRouteAttribute;

         string constraint = routeAttr.Constraint;

         if (constraint == null) {
            Type t = (actionParam.IsNullableValueType) ? Nullable.GetUnderlyingType(actionParam.Type) : actionParam.Type;
            actionParam.Action.Controller.Register.Settings.DefaultConstraints.TryGetValue(t, out constraint);
         }

         bool isCatchAll = routeAttr.CatchAll;

         return new TokenInfo(routeAttr.TokenName ?? name, constraint, isOptional, isCatchAll);
      }

      public ActionInfo(ControllerInfo controller) {
         this.Controller = controller;
      }

      protected abstract ActionParameterInfo[] GetParameters();
      public abstract object[] GetCustomAttributes(bool inherit);
      public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
      public abstract bool IsDefined(Type attributeType, bool inherit);

      void CheckCatchAllParamIsLast(IList<TokenInfo> parameters) {

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
   }
}
