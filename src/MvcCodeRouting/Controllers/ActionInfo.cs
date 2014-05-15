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
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Controllers {

   [DebuggerDisplay("{ActionUrl}")]
   abstract class ActionInfo : ICustomAttributeProvider {

      string _Name;
      string _ActionSegment;
      string _CustomRoute;
      bool _CustomRouteInit;
      bool? _CustomRouteHasActionToken;
      Collection<ActionParameterInfo> _Parameters;
      RouteParameterCollection _RouteParameters;

      public ControllerInfo Controller { get; private set; }
      
      public string Name {
         get {
            return _Name
               ?? (_Name = GetName());
         }
      }

      public virtual string MethodName { get { return Name; } }
      public virtual Type DeclaringType { get { return Controller.Type; } }

      public string ActionSegment {
         get {
            return _ActionSegment
               ?? (_ActionSegment = Controller.Register.Settings.FormatRouteSegment(new RouteFormatterArgs(Name, RouteSegmentType.Action, Controller.Type)));
         }
      }

      public Collection<ActionParameterInfo> Parameters {
         get {
            return _Parameters
               ?? (_Parameters = new Collection<ActionParameterInfo>(GetParameters()));
         }
      }

      public RouteParameterCollection RouteParameters {
         get {
            if (_RouteParameters == null) {

               _RouteParameters = new RouteParameterCollection(
                  (from p in Parameters
                   where p.FromRouteAttribute != null
                   select CreateRouteParameter(p))
                  .ToList()
               );

               CheckCatchAllParamIsLast(_RouteParameters);

               if (_RouteParameters.Count == 0
                  && Controller.Register.Settings.UseImplicitIdToken) {

                  ActionParameterInfo id = Parameters.FirstOrDefault(p => p.Name.Equals("id", StringComparison.OrdinalIgnoreCase));

                  if (id != null) {

                     _RouteParameters = new RouteParameterCollection(
                        new[] { CreateRouteParameter(id) }
                     );
                  }
               }
            }

            return _RouteParameters;
         }
      }

      public bool IsDefaultAction { get; internal set; }

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

               ICustomRouteAttribute attr = Controller.Provider
                  .GetCorrectAttribute<ICustomRouteAttribute>(
                     this,
                     prov => prov.CustomRouteAttributeType,
                     inherit: true,
                     errorMessage: (attrType, mistakenAttrType) =>
                        String.Format(CultureInfo.InvariantCulture,
                           "Must use {0} instead of {1} on {3}.",
                           attrType.FullName,
                           mistakenAttrType.FullName,
                           Name,
                           String.Concat(DeclaringType.FullName, ".", MethodName, "(", String.Join(", ", Parameters.Select(p => p.Type.Name)), ")")
                        )
                  );

               if (attr != null) {
                  _CustomRoute = attr.Url;
               }

               _CustomRouteInit = true;
            }

            return _CustomRoute;
         }
      }

      public bool CustomRouteHasActionToken {
         get {
            if (CustomRoute == null) {
               return false;
            }

            if (_CustomRouteHasActionToken == null) {
               _CustomRouteHasActionToken =
                  CustomRoute.IndexOf("{action}", StringComparison.OrdinalIgnoreCase) != -1;
            }

            return _CustomRouteHasActionToken.Value;
         }
      }

      public bool CustomRouteIsAbsolute {
         get {
            if (CustomRoute == null) {
               return false;
            }

            return CustomRoute.StartsWith("~/", StringComparison.OrdinalIgnoreCase);
         }
      }

      public bool HasActionOverloadDisambiguationAttribute {
         get { return IsDefined(Controller.Provider.ActionOverloadDisambiguationAttributeType, inherit: true); }
      }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      public static bool IsCallableActionMethod(MethodInfo method) {
         
         return !method.IsSpecialName
            && !method.ContainsGenericParameters
            && !method.GetParameters().Any(p => p.IsOut || p.ParameterType.IsByRef);
      }

      static RouteParameter CreateRouteParameter(ActionParameterInfo actionParam) {

         IFromRouteAttribute routeAttr = actionParam.FromRouteAttribute;

         bool isCatchAll = (routeAttr != null) ?
            routeAttr.CatchAll
            : false;

         return actionParam.Action.Controller.CreateRouteParameter(actionParam.Name, actionParam.Type, routeAttr, actionParam.IsOptional, isCatchAll);
      }

      protected ActionInfo(ControllerInfo controller) {
         this.Controller = controller;
      }

      protected abstract ActionParameterInfo[] GetParameters();
      protected abstract string GetName();

      public abstract object[] GetCustomAttributes(bool inherit);
      public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
      public abstract bool IsDefined(Type attributeType, bool inherit);

      void CheckCatchAllParamIsLast(IList<RouteParameter> parameters) {

         for (int i = 0; i < parameters.Count; i++) {

            var param = parameters[i];

            if (param.IsCatchAll && i < parameters.Count - 1) {

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
}
