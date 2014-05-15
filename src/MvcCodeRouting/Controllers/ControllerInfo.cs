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
using MvcCodeRouting.ParameterBinding.Binders;

namespace MvcCodeRouting.Controllers {
   
   [DebuggerDisplay("{ControllerUrl}")]
   abstract class ControllerInfo : ICustomAttributeProvider {

      ReadOnlyCollection<string> _CodeRoutingNamespace;
      ReadOnlyCollection<string> _CodeRoutingContext;
      ReadOnlyCollection<string> _NamespaceSegments;
      ReadOnlyCollection<string> _BaseRouteAndNamespaceSegments;
      RouteParameterCollection _RouteProperties;
      Collection<ActionInfo> _Actions;
      string _Name;
      string _ControllerSegment;
      string _CustomRoute;
      bool _CustomRouteInit;
      bool? _CustomRouteHasControllerToken;
      string _UrlTemplate;
      string _ControllerUrl;

      public Type Type { get; private set; }
      public RegisterSettings Register { get; private set; }
      public CodeRoutingProvider Provider { get; private set; }

      public virtual string Name {
         get {
            return _Name
               ?? (_Name = Type.Name.Substring(0, Type.Name.Length - "Controller".Length));
         }
      }

      public string Namespace {
         get {
            return Type.Namespace 
               ?? "";
         }
      }

      public bool IsInRootNamespace {
         get {
            return Namespace == Register.RootNamespace
               || IsInSubNamespace;
         }
      }

      public bool IsInSubNamespace {
         get {
            return Namespace.Length > Register.RootNamespace.Length
               && Namespace.StartsWith(Register.RootNamespace + ".", StringComparison.Ordinal);
         }
      }

      public bool IsRootController {
         get {
            return Type == Register.RootController;
         }
      }

      public string ControllerSegment {
         get {
            return _ControllerSegment
               ?? (_ControllerSegment = Register.Settings.FormatRouteSegment(new RouteFormatterArgs(Name, RouteSegmentType.Controller, Type)));
         }
      }

      public ReadOnlyCollection<string> CodeRoutingNamespace {
         get {
            if (_CodeRoutingNamespace == null) {

               var segments = new List<string>();

               if (IsInSubNamespace) {
                  
                  segments.AddRange(Namespace.Remove(0, Register.RootNamespace.Length + 1).Split('.'));

                  if (segments.Count > 0 && NameEquals(segments.Last(), Name))
                     segments.RemoveAt(segments.Count - 1);
               }

               _CodeRoutingNamespace = new ReadOnlyCollection<string>(segments);
            }

            return _CodeRoutingNamespace;
         }
      }

      public ReadOnlyCollection<string> CodeRoutingContext {
         get {
            if (_CodeRoutingContext == null) {

               if (Register.BaseRoute == null) {
                  _CodeRoutingContext = new ReadOnlyCollection<string>(CodeRoutingNamespace);
               
               } else {

                  var segments = new List<string>();
                  segments.AddRange(Register.BaseRoute.Split('/'));
                  segments.AddRange(CodeRoutingNamespace);

                  _CodeRoutingContext = new ReadOnlyCollection<string>(segments);
               }
            }

            return _CodeRoutingContext;
         }
      }

      public ReadOnlyCollection<string> NamespaceSegments {
         get {
            if (_NamespaceSegments == null) {

               var namespaceSegments = new List<string>();

               namespaceSegments.AddRange(
                  CodeRoutingNamespace.Select(s => Register.Settings.FormatRouteSegment(new RouteFormatterArgs(s, RouteSegmentType.Namespace, Type)))
               );

               _NamespaceSegments = new ReadOnlyCollection<string>(namespaceSegments);
            }

            return _NamespaceSegments;
         }
      }

      public ReadOnlyCollection<string> BaseRouteAndNamespaceSegments {
         get {
            if (_BaseRouteAndNamespaceSegments == null) {

               if (Register.BaseRoute == null) {
                  _BaseRouteAndNamespaceSegments = new ReadOnlyCollection<string>(NamespaceSegments);
               
               } else {

                  var segments = new List<string>();
                  segments.AddRange(Register.BaseRoute.Split('/'));
                  segments.AddRange(NamespaceSegments);

                  _BaseRouteAndNamespaceSegments = new ReadOnlyCollection<string>(segments);
               }
            }

            return _BaseRouteAndNamespaceSegments;
         }
      }

      public RouteParameterCollection RouteProperties {
         get {
            if (_RouteProperties == null) {

               var types = new List<Type>();

               for (Type t = this.Type; t != null; t = t.BaseType) {
                  types.Add(t);
               }

               types.Reverse();

               var list = new List<RouteParameter>();

               foreach (Type t in types) {

                  list.AddRange(
                     from p in t.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                     
                     let attr = Provider.GetCorrectAttribute<IFromRouteAttribute>(
                        p, 
                        prov => prov.FromRouteAttributeType, 
                        inherit: false /* [1] */,
                        errorMessage: (attrType, mistakenAttrType) =>
                           String.Format(CultureInfo.InvariantCulture,
                              "Must use {0} instead of {1} (property {2} on {3}).",
                              attrType.FullName,
                              mistakenAttrType.FullName,
                              p.Name,
                              t.FullName
                           )
                        )

                     where attr != null
                     let rp = CreateRouteParameter(p, attr)
                     where !list.Any(item => RouteParameter.NameEquals(item.Name, rp.Name))
                     select rp
                  );
               }

               _RouteProperties = new RouteParameterCollection(list);
            }

            return _RouteProperties;

            // [1] Procesing each type of the hierarchy one by one, hence inherit: false.
         }
      }

      public Collection<ActionInfo> Actions {
         get {
            if (_Actions == null) {

               var actions =
                  (from a in GetActions()
                   where !IsNonAction(a)
                   select a).ToArray();

               CheckDefaultActions(actions);
               CheckOverloads(actions);
               CheckCustomRoutes(actions);

               _Actions = new Collection<ActionInfo>(actions);
            }

            return _Actions;
         }
      }

      public string UrlTemplate {
         get {
            return _UrlTemplate
               ?? (_UrlTemplate = BuildUrl(template: true));
         }
      }

      public string ControllerUrl {
         get {
            return _ControllerUrl
               ?? (_ControllerUrl = BuildUrl(template: false));
         }
      }

      public string CustomRoute {
         get {
            if (!_CustomRouteInit) {

               ICustomRouteAttribute attr = Provider
                  .GetCorrectAttribute<ICustomRouteAttribute>(
                     this,
                     prov => prov.CustomRouteAttributeType,
                     inherit: true,
                     errorMessage: (attrType, mistakenAttrType) =>
                        String.Format(CultureInfo.InvariantCulture,
                           "Must use {0} instead of {1} on {2}.",
                           attrType.FullName,
                           mistakenAttrType.FullName,
                           Type.FullName
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

      public bool CustomRouteHasControllerToken {
         get {
            if (CustomRoute == null) {
               return false;
            }

            if (_CustomRouteHasControllerToken == null) {
               _CustomRouteHasControllerToken =
                  CustomRoute.IndexOf("{controller}", StringComparison.OrdinalIgnoreCase) != -1;
            }

            return _CustomRouteHasControllerToken.Value;
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

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      string BuildUrl(bool template) {

         string custRoute = this.CustomRoute;

         var segments = new List<string>();
         segments.AddRange(this.BaseRouteAndNamespaceSegments);

         if (custRoute != null) {

            if (this.CustomRouteIsAbsolute) {

               segments.Clear();

               if (this.Register.BaseRoute != null) {
                  segments.AddRange(this.Register.BaseRoute.Split('/'));
               }

               custRoute = custRoute.Substring(2);
            }

            segments.AddRange(custRoute.Split('/'));

         } else {

            if (!this.IsRootController) {
               segments.Add(template ? "{controller}" : this.ControllerSegment);
            }

            segments.AddRange(this.RouteProperties.Select(p => p.RouteSegment));
         }

         string url = String.Join("/", segments);

         return url;
      }

      void CheckOverloads(IEnumerable<ActionInfo> actions) {

         var overloadedActions =
            (from a in actions
             where a.RouteParameters.Count > 0
             group a by new { a.Controller, Name = a.ActionSegment } into g
             where g.Count() > 1
             select g).ToList();

         if (!Provider.CanDisambiguateActionOverloads) {

            var withoutRequiredAttr =
               (from g in overloadedActions
                let distinctParamCount = g.Select(a => a.RouteParameters.Count).Distinct()
                where distinctParamCount.Count() > 1
                let bad = g.Where(a => !a.HasActionOverloadDisambiguationAttribute)
                where bad.Count() > 0
                select bad).ToList();

            if (withoutRequiredAttr.Count > 0) {

               var first = withoutRequiredAttr.First();

               throw new InvalidOperationException(
                  String.Format(CultureInfo.InvariantCulture,
                     "The following action methods must be decorated with {0} for disambiguation: {1}.",
                     Provider.ActionOverloadDisambiguationAttributeType.FullName,
                     String.Join(", ", first.Select(a => String.Concat(a.DeclaringType.FullName, ".", a.MethodName, "(", String.Join(", ", a.Parameters.Select(p => p.Type.Name)), ")")))
                  )
               );
            }
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
                  "Overloaded action methods must have route parameters that are equal in name, position and constraint ({0}).",
                  String.Concat(first.Key.Controller.Type.FullName, ".", first.First().MethodName)
               )
            );
         }
      }

      void CheckCustomRoutes(IEnumerable<ActionInfo> actions) { 

         var sameCustomRouteDifferentNames = 
            (from a in actions
             where a.CustomRoute != null
               && !a.CustomRouteHasActionToken
             group a by a.CustomRoute into grp
             let distinctNameCount = grp.Select(a => a.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count()
             where distinctNameCount > 1
             select grp).ToList();

         if (sameCustomRouteDifferentNames.Count > 0) {

            var first = sameCustomRouteDifferentNames.First();
            
            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture,
                  "Action methods decorated with {0} must have the same name: {1}.",
                  Provider.CustomRouteAttributeType.FullName,
                  String.Join(", ", first.Select(a => String.Concat(a.DeclaringType.FullName, ".", a.MethodName, "(", String.Join(", ", a.Parameters.Select(p => p.Type.Name)), ")")))
               )
            );
         }
      }

      void CheckDefaultActions(IEnumerable<ActionInfo> actions) { 
         
         // - Index is the default action by convention
         // - You can use [DefaultAction] to override the convention
         //   - Can only be applied to one action per controller type
         //   - Can be inherited from base controller
         //   - Derived controllers can override the inherited [DefaultAction] by applying it to a different action
         // - Default action cannot have required route parameters (either no parameters or all optional)

         Func<ActionInfo, bool> correctRouteParameterSetup = a =>
            a.RouteParameters.Count == 0 
               || a.RouteParameters.All(p => p.IsOptional);

         ActionInfo defaultAction = null;

         Type attrType = this.Provider.DefaultActionAttributeType;

         if (attrType != null) {

            var defaultActions =
               (from a in actions
                where a.GetCustomAttributes(attrType, inherit: false).Any()
                select a).ToArray();

            if (defaultActions.Any()) {

               var byDeclaringType =
                  from a in defaultActions
                  group a by a.DeclaringType;

               if (defaultActions.Length > byDeclaringType.Count()) {
                  throw new InvalidOperationException(
                     "{0} can only be used once per declaring type: {1}.".FormatInvariant(attrType.FullName, byDeclaringType.First(g => g.Count() > 1).Key.FullName)
                  );
               }

               for (Type t = this.Type; t != null; t = t.BaseType) {

                  defaultAction = defaultActions.SingleOrDefault(a => a.DeclaringType == t);

                  if (defaultAction != null) {

                     if (!correctRouteParameterSetup(defaultAction)) {
                        throw new InvalidOperationException(
                           "Default actions cannot have required route parameters: {0}.".FormatInvariant(
                              String.Concat(defaultAction.DeclaringType.FullName, ".", defaultAction.MethodName, "(", String.Join(", ", defaultAction.Parameters.Select(p => p.Type.Name)), ")")
                           )
                        );
                     }

                     break;
                  }
               }
            } 
         }

         if (defaultAction == null) {
            defaultAction = actions.FirstOrDefault(a => ActionInfo.NameEquals(a.Name, "Index") && correctRouteParameterSetup(a));
         }

         if (defaultAction != null) {
            defaultAction.IsDefaultAction = true;
         }
      }

      protected ControllerInfo(Type type, RegisterSettings registerSettings, CodeRoutingProvider provider) {
         
         this.Type = type;
         this.Register = registerSettings;
         this.Provider = provider;
      }

      protected internal abstract ActionInfo[] GetActions();
      public abstract object[] GetCustomAttributes(bool inherit);
      public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
      public abstract bool IsDefined(Type attributeType, bool inherit);
      protected abstract bool IsNonAction(ICustomAttributeProvider action);

      RouteParameter CreateRouteParameter(PropertyInfo property, IFromRouteAttribute routeAttr) {
         return CreateRouteParameter(property.Name, property.PropertyType, routeAttr, isOptional: false, isCatchAll: false);
      }

      internal RouteParameter CreateRouteParameter(string name, Type type, IFromRouteAttribute routeAttr, bool isOptional, bool isCatchAll) {

         if (routeAttr != null
            && routeAttr.Name.HasValue()) {

            name = routeAttr.Name;
         }

         type = TypeHelpers.GetNullableUnderlyingType(type);

         string constraint = GetConstraintForType(type, routeAttr);

         BinderSource binderSource;
         ParameterBinder binder = GetBinderForType(type, routeAttr, out binderSource);

         if (binder == null
            && type.IsEnum) {

            binder = (ParameterBinder)Activator.CreateInstance(typeof(EnumParameterBinder<>).MakeGenericType(type));
         }

         if (constraint.HasValue()
            && binderSource != BinderSource.Parameter) {
            
            binder = null;
         }

         return new RouteParameter(name, type, constraint, isOptional, isCatchAll, binder);
      }

      ParameterBinder GetBinderForType(Type type, IFromRouteAttribute routeAttr, out BinderSource source) {

         ParameterBinder paramBinder = null;
         source = BinderSource.None;

         if (routeAttr != null
            && routeAttr.BinderType != null) {

            paramBinder = ParameterBinder.GetInstance(null, routeAttr.BinderType);

            if (paramBinder != null) {
               source = BinderSource.Parameter;
               return paramBinder;
            }
         }

         type = TypeHelpers.GetNullableUnderlyingType(type);

         if (this.Register.Settings.ParameterBinders.TryGetItem(type, out paramBinder)) {
            source = BinderSource.Settings;
            return paramBinder;
         }

         paramBinder = ParameterBinder.GetInstance(type, null);

         if (paramBinder != null) {
            source = BinderSource.Type;
         }

         return paramBinder;
      }

      string GetConstraintForType(Type type, IFromRouteAttribute routeAttr) {

         string constraint = null;

         if (routeAttr != null) {
            constraint = routeAttr.Constraint;
         }

         if (constraint == null) {

            type = TypeHelpers.GetNullableUnderlyingType(type);

            this.Register.Settings.DefaultConstraints.TryGetValue(type, out constraint);
         }

         return constraint;
      }

      enum BinderSource { 
         None = 0,
         Parameter,
         Settings,
         Type
      }
   }
}
