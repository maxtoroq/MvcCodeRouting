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
using System.Runtime.Serialization;
using System.Security;

namespace MvcCodeRouting {
   
   [DebuggerDisplay("{ControllerUrl}")]
   class ControllerInfo {

      static readonly Type baseType = typeof(Controller);
      static readonly Func<Controller, IActionInvoker> createActionInvoker;
      static readonly Func<ControllerActionInvoker, ControllerContext, ControllerDescriptor> getControllerDescriptor;

      const string RootController = "Home";
      const string DefaultAction = "Index";

      readonly ControllerDescriptor controllerDescr;

      ReadOnlyCollection<string> _NamespaceRouteParts;
      ReadOnlyCollection<string> _ControllerBaseRouteSegments;
      TokenInfoCollection _RouteProperties;
      string _Name;

      public Type Type { get; private set; }
      public RegisterInfo Register { get; private set; }
      public string DefaultActionName { get; private set; }

      public string Name {
         get {
            if (_Name == null) {
               string controllerName = (controllerDescr != null) ? controllerDescr.ControllerName 
                  : Type.Name.Substring(0, Type.Name.Length - 10);
                  
               _Name = Register.Settings.RouteFormatter(controllerName, RouteSegmentType.Controller);
               CodeRoutingSettings.CheckCaseFormattingOnly(controllerName, _Name, RouteSegmentType.Controller);
            }
            return _Name;
         }
      }

      public bool IsInRootNamespace {
         get {
            return Type.Namespace == Register.RootNamespace
               || IsInSubNamespace;
         }
      }

      public bool IsInSubNamespace {
         get {
            return Type.Namespace.Length > Register.RootNamespace.Length
               && Type.Namespace.StartsWith(Register.RootNamespace + ".", StringComparison.Ordinal);
         }
      }

      public bool IsRootController {
         get {
            return !String.IsNullOrWhiteSpace(RootController)
               && NamespaceRouteParts.Count == 0
               && NameEquals(Name, RootController);
         }
      }

      public bool IsIgnored {
         get {
            return Register.Settings.IgnoredControllers.Contains(Type);
         }
      }

      public ReadOnlyCollection<string> NamespaceRouteParts {
         get {
            if (_NamespaceRouteParts == null) {
               var namespaceParts = new List<string>();

               if (IsInSubNamespace) {
                  namespaceParts.AddRange(
                     Type.Namespace.Remove(0, Register.RootNamespace.Length + 1).Split('.')
                  );

                  if (namespaceParts.Count > 0 && NameEquals(namespaceParts.Last(), Name))
                     namespaceParts.RemoveAt(namespaceParts.Count - 1);
               }

               _NamespaceRouteParts = new ReadOnlyCollection<string>(namespaceParts);
            }
            return _NamespaceRouteParts;
         }
      }

      public ReadOnlyCollection<string> ControllerBaseRouteSegments {
         get {
            if (_ControllerBaseRouteSegments == null) {
               string[] nsSegments = NamespaceRouteParts.Select(s => Register.Settings.RouteFormatter(s, RouteSegmentType.Namespace)).ToArray();

               for (int i = 0; i < nsSegments.Length; i++)
                  CodeRoutingSettings.CheckCaseFormattingOnly(NamespaceRouteParts[i], nsSegments[i], RouteSegmentType.Namespace);

               if (String.IsNullOrEmpty(Register.BaseRoute)) {
                  _ControllerBaseRouteSegments = new ReadOnlyCollection<string>(nsSegments);
               } else {
                  var segments = new List<string>();
                  segments.AddRange(Register.BaseRoute.Split('/'));
                  segments.AddRange(nsSegments);

                  _ControllerBaseRouteSegments = new ReadOnlyCollection<string>(segments);
               }
            }
            return _ControllerBaseRouteSegments;
         }
      }

      public TokenInfoCollection RouteProperties {
         get {
            if (_RouteProperties == null) {

               var types = new List<Type>();

               for (Type t = this.Type; t != null; t = t.BaseType) 
                  types.Add(t);

               types.Reverse();

               var list = new List<TokenInfo>();

               foreach (var type in types) {
                  list.AddRange(
                     from p in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                     where p.IsDefined(typeof(FromRouteAttribute), inherit: false /* [1] */)
                     let rp = CreateTokenInfo(p)
                     where !list.Any(item => TokenInfo.NameEquals(item.Name, rp.Name))
                     select rp
                  );
               }

               _RouteProperties = new TokenInfoCollection(list);
            }
            return _RouteProperties;

            // [1] Procesing each type of the hierarchy one by one, hence inherit: false.
         }
      }

      public string UrlTemplate {
         get {
            return String.Join("/", ControllerBaseRouteSegments
               .Concat((!IsRootController) ? new[] { "{controller}" } : new string[0])
               .Concat(RouteProperties.Select(p => p.RouteSegment))
            );
         }
      }

      public string ControllerUrl {
         get {
            return String.Join("/", ControllerBaseRouteSegments
               .Concat((!IsRootController) ? new[] { Name } : new string[0])
               .Concat(RouteProperties.Select(p => p.RouteSegment))
            );
         }
      }

      static ControllerInfo() {

         try {
            createActionInvoker =
               (Func<Controller, IActionInvoker>)
                  Delegate.CreateDelegate(typeof(Func<Controller, IActionInvoker>), baseType.GetMethod("CreateActionInvoker", BindingFlags.NonPublic | BindingFlags.Instance));

            getControllerDescriptor =
               (Func<ControllerActionInvoker, ControllerContext, ControllerDescriptor>)
                  Delegate.CreateDelegate(typeof(Func<ControllerActionInvoker, ControllerContext, ControllerDescriptor>), typeof(ControllerActionInvoker).GetMethod("GetControllerDescriptor", BindingFlags.NonPublic | BindingFlags.Instance));
         
         } catch (MethodAccessException) { }
      }

      public static IEnumerable<ControllerInfo> GetControllers(RegisterInfo registerInfo) {

         return
            from t in registerInfo.Assembly.GetTypes()
            where t.IsPublic
              && !t.IsAbstract
              && baseType.IsAssignableFrom(t)
              && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            let controllerInfo = new ControllerInfo(t, registerInfo)
            where controllerInfo.IsInRootNamespace
               && !controllerInfo.IsIgnored
            select controllerInfo;
      }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      static void CheckOverloads(IEnumerable<ActionInfo> actions) {

         var overloadedActions =
            (from a in actions
             where a.RouteParameters.Count > 0
             group a by new { a.Controller, a.Name } into g
             where g.Count() > 1
             select g).ToList();

         var withoutRequiredAttr =
            (from g in overloadedActions
             let distinctParamCount = g.Select(a => a.RouteParameters.Count).Distinct()
             where distinctParamCount.Count() > 1
             let bad = g.Where(a => !a.HasRequireRouteParametersAttribute)
             where bad.Count() > 0
             select bad).ToList();

         if (withoutRequiredAttr.Count > 0) {
            var first = withoutRequiredAttr.First();

            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture,
                  "The following action methods must be decorated with the {0} for disambiguation: {1}.",
                  typeof(RequireRouteParametersAttribute).FullName,
                  String.Join(", ", first.Select(a => String.Concat(a.DeclaringType.FullName, ".", a.MethodName, "(", String.Join(", ", a.Parameters.Select(p => p.Type.Name)), ")")))
               )
            );
         }

         var overloadsComparer = new ActionSignatureComparer();

         var overloadsWithDifferentParameters =
            (from g in overloadedActions
             let ordered = g.OrderByDescending(a => a.RouteParameters.Count).ToArray()
             let first = ordered.First()
             where !ordered.Skip(1).All(a => overloadsComparer.Equals(first, a))
             select g).ToList();

         if (overloadsWithDifferentParameters.Count > 0) {
            var first = overloadsWithDifferentParameters.First();

            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture,
                  "Overloaded action methods must have parameters that are equal in name, position and constraint ({0}).",
                  String.Concat(first.Key.Controller.Type.FullName, ".", first.First().MethodName)
               )
            );
         }
      }

      public ControllerInfo(Type type, RegisterInfo registerInfo) {
         
         this.Type = type;
         this.Register = registerInfo;
         this.DefaultActionName = DefaultAction;

         if (createActionInvoker != null) {
            
            Controller instance = null;

            try {
               instance = (Controller)FormatterServices.GetUninitializedObject(this.Type);
            } catch (SecurityException) { }

            if (instance != null) {

               ControllerActionInvoker actionInvoker = createActionInvoker(instance) as ControllerActionInvoker;

               if (actionInvoker != null)
                  this.controllerDescr = getControllerDescriptor(actionInvoker, new ControllerContext { Controller = instance });
            } 
         }
      }

      public IEnumerable<ActionInfo> GetActions() {

         IEnumerable<ActionInfo> actions;

         if (this.controllerDescr != null)
            actions = GetActions(this.controllerDescr);
         else
            actions = GetActions(this.Type);

         CheckOverloads(actions);

         return actions;
      }

      IEnumerable<ActionInfo> GetActions(ControllerDescriptor controllerDescr) {

         var actions =
            from a in controllerDescr.GetCanonicalActions()
            where !a.IsDefined(typeof(NonActionAttribute), inherit: true)
            select new DescriptedActionInfo(a, this);
         
         return actions;
      }

      IEnumerable<ActionInfo> GetActions(Type controllerType) {

         bool controllerIsDisposable = typeof(IDisposable).IsAssignableFrom(controllerType);

         var actions =
             from m in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
             where !m.IsSpecialName
                && baseType.IsAssignableFrom(m.DeclaringType)
                && !m.IsDefined(typeof(NonActionAttribute), inherit: true)
                && !(controllerIsDisposable && m.Name == "Dispose" && m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
             select new ReflectedActionInfo(m, this);

         return actions;
      }

      TokenInfo CreateTokenInfo(PropertyInfo property) {

         Type propertyType = property.PropertyType;
         bool isNullableValueType = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

         string name = property.Name;

         var routeAttr = property.GetCustomAttributes(typeof(FromRouteAttribute), inherit: true)
            .Cast<FromRouteAttribute>()
            .Single();

         string constraint = routeAttr.Constraint;

         if (constraint == null) {
            Type t = (isNullableValueType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;
            this.Register.Settings.DefaultConstraints.TryGetValue(t, out constraint);
         }

         return new TokenInfo(name, constraint);
      }
   }
}
