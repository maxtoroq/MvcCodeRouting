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
using MvcCodeRouting.Web;

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
            if (_Name == null) 
               _Name = Type.Name.Substring(0, Type.Name.Length - "Controller".Length);
            return _Name;
         }
      }

      public string Namespace {
         get {
            return Type.Namespace ?? "";
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
            if (_ControllerSegment == null) 
               _ControllerSegment = Register.Settings.FormatRouteSegment(new RouteFormatterArgs(Name, RouteSegmentType.Controller, Type));
            return _ControllerSegment;
         }
      }

      public ReadOnlyCollection<string> CodeRoutingNamespace {
         get {
            if (_CodeRoutingNamespace == null) {

               List<string> segments = new List<string>();

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

               for (Type t = this.Type; t != null; t = t.BaseType) 
                  types.Add(t);

               types.Reverse();

               var list = new List<RouteParameter>();

               foreach (var type in types) {
                  list.AddRange(
                     from p in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                     where p.IsDefined(Provider.FromRouteAttributeType, inherit: false /* [1] */)
                     let rp = CreateRouteParameter(p)
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
               _Actions = new Collection<ActionInfo>(
                  (from a in GetActions()
                   where !IsNonAction(a)
                   select a).ToArray()
               );

               if (!Provider.CanDisambiguateActionOverloads)
                  CheckOverloads(_Actions);

               CheckCustomRoutes(_Actions);
            }
            return _Actions;
         }
      }

      public string UrlTemplate {
         get {
            if (_UrlTemplate == null) 
               _UrlTemplate = BuildUrl(template: true);
            return _UrlTemplate;
         }
      }

      public string ControllerUrl {
         get {
            if (_ControllerUrl == null)
               _ControllerUrl = BuildUrl(template: false);
            return _ControllerUrl;
         }
      }

      public string CustomRoute {
         get {
            if (!_CustomRouteInit) {

               var attr = GetCustomAttributes(Provider.CustomRouteAttributeType, inherit: true)
                  .Cast<ICustomRouteAttribute>()
                  .SingleOrDefault();

               if (attr != null)
                  _CustomRoute = attr.Url;

               _CustomRouteInit = true;
            }
            return _CustomRoute;
         }
      }

      public bool CustomRouteHasControllerToken {
         get {
            if (CustomRoute == null)
               return false;

            if (_CustomRouteHasControllerToken == null) {
               _CustomRouteHasControllerToken =
                  CustomRoute.IndexOf("{controller}", StringComparison.OrdinalIgnoreCase) != -1;
            }

            return _CustomRouteHasControllerToken.Value;
         }
      }

      public bool CustomRouteIsAbsolute {
         get {
            if (CustomRoute == null)
               return false;

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

               if (this.Register.BaseRoute != null)
                  segments.AddRange(this.Register.BaseRoute.Split('/'));

               custRoute = custRoute.Substring(2);
            }

            segments.AddRange(custRoute.Split('/'));

         } else {

            if (!this.IsRootController)
               segments.Add(template ? "{controller}" : this.ControllerSegment);

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

      RouteParameter CreateRouteParameter(PropertyInfo property) {

         Type propertyType = property.PropertyType;

         var routeAttr = property.GetCustomAttributes(Provider.FromRouteAttributeType, inherit: true)
            .Cast<IFromRouteAttribute>()
            .Single();

         string name = routeAttr.TokenName ?? property.Name;
         string constraint = this.Register.Settings.GetConstraintForType(propertyType, routeAttr);

         return new RouteParameter(name, constraint);
      }
   }
}
