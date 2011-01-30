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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {

   public static class CodeRoutingExtensions {

      static readonly IDictionary<Type, string> _defaultConstraints;

      static CodeRoutingExtensions() {
         
         _defaultConstraints = new Dictionary<Type, string>();

         _defaultConstraints.Add(typeof(Boolean), "true|false");
         _defaultConstraints.Add(typeof(Guid), @"\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Za-z0-9]{12}\b");

         foreach (var type in new[] { typeof(Decimal), typeof(Double), typeof(Single) })
            _defaultConstraints.Add(type, @"-?(0|[1-9]\d*)(\.\d+)?");

         foreach (var type in new[] { typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64) })
            _defaultConstraints.Add(type, @"0|-?[1-9]\d*");

         foreach (var type in new[] { typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64) })
            _defaultConstraints.Add(type, @"0|[1-9]\d*");
      }

      public static void MapCodeRoutes(
         this RouteCollection routes,
         Assembly assembly = null,
         string baseNamespace = null,
         string rootController = "Home",
         string defaultAction = "Index",
         IDictionary<Type, string> defaultConstraints = null,
         Func<string, string> routeFormatter = null,
         Func<MethodInfo, string> actionNameExtractor = null
         ) {

         if (assembly == null)
            assembly = Assembly.GetCallingAssembly();

         if (baseNamespace == null)
            baseNamespace = assembly.GetName().Name;

         if (defaultConstraints == null) {
            defaultConstraints = _defaultConstraints;
         } else {
            var def = new Dictionary<Type, string>(_defaultConstraints);

            foreach (var item in defaultConstraints) 
               def[item.Key] = item.Value;

            defaultConstraints = def;
         }

         if (routeFormatter == null)
            routeFormatter = s => s;

         var actions = ControllerInfo.GetControllers(assembly, baseNamespace, rootController, defaultAction, routeFormatter)
            .Select(c => c.GetActions(actionNameExtractor, defaultConstraints))
            .SelectMany(a => a);

         var groupedActions = GroupActions(actions);

         foreach (var route in groupedActions.Select(g => CreateRoute(g)))
            routes.Add(route);
      }

      private static IEnumerable<IEnumerable<ActionInfo>> GroupActions(IEnumerable<ActionInfo> actions) {

         var groupedActions =
            (from a in actions
             orderby a.Controller.IsRootController descending
                , (a.Controller.IsRootController && a.IsDefaultAction) descending
                , a.Controller.BaseRouteSegments.Count
                , a.Controller.Type.Namespace
                , a.Controller.Name
                , a.Name
                , a.RouteParameters.Count descending
             let declaringType1 = a.Method.DeclaringType
             let declaringType = (declaringType1.IsGenericType) ?
                declaringType1.GetGenericTypeDefinition()
                : declaringType1
             group a by new {
                a.Controller.Type.Namespace,
                DeclaringType = declaringType,
                //a.RouteParameters
                HasRouteParameters = (a.RouteParameters.Count > 0)
             }).ToList();

         var overloadsComparer = new RouteEqualityComparer();
         var finalGrouping = new List<IEnumerable<ActionInfo>>();

         for (int i = 0; i < groupedActions.Count; i++) {

            var set = groupedActions[i];

            if (set.Key.HasRouteParameters) {

               var ordered = set.OrderByDescending(a => a.RouteParameters.Count).ToList();

               while (ordered.Count > 0) {
                  var first = ordered.First();
                  var overloads = ordered.Skip(1).Where(a => overloadsComparer.Equals(first, a)).ToList();

                  if (overloads.Count > 0) {
                     var last = overloads.Last();

                     foreach (var param in first.RouteParameters.Skip(last.RouteParameters.Count))
                        param.IsOptional = true;

                     finalGrouping.Add(new ActionInfo[] { first }.Concat(overloads));

                     foreach (var item in overloads)
                        ordered.Remove(item);

                  } else {
                     finalGrouping.Add(new ActionInfo[] { first });
                  }

                  ordered.Remove(first);
               }
            } else {

               finalGrouping.Add(set);
            }
         }

         return finalGrouping;
      }

      private static Route CreateRoute(IEnumerable<ActionInfo> actions) {

         ActionInfo first = actions.First();
         int count = actions.Count();
         var controllerNames = actions.Select(a => a.Controller.Name).Distinct().ToList();
         var actionNames = actions.Select(a => a.Name).Distinct().ToList();

         List<string> segments = new List<string>();
         segments.Add(first.Controller.UrlTemplate);

         if (controllerNames.Count == 1)
            segments[0] = first.Controller.ControllerUrl;

         segments.Add("{action}");

         bool hardcodeAction = actionNames.Count == 1
            && !(count == 1 && first.IsDefaultAction);

         if (hardcodeAction) 
            segments[1] = first.Name;

         segments.AddRange(first.RouteParameters.Select(r => r.RouteSegment));

         string url = String.Join("/", segments.Where(s => !String.IsNullOrEmpty(s)));

         var defaults = new RouteValueDictionary();

         if (controllerNames.Count == 1)
            defaults.Add("controller", controllerNames.First());

         string defaultAction = null;

         if (actionNames.Count == 1) {
            defaultAction = first.Name;
         } else {
            var defAction = actions.FirstOrDefault(a => a.IsDefaultAction);

            if (defAction != null)
               defaultAction = defAction.Name;
         }

         if (defaultAction != null)
            defaults.Add("action", defaultAction);

         var parameters = first.RouteParameters;

         foreach (var param in parameters.Where(p => p.IsOptional))
            defaults.Add(param.Name, UrlParameter.Optional);

         var constraints = new RouteValueDictionary();

         if (controllerNames.Count > 1)
            constraints.Add("controller", String.Join("|", controllerNames));

         if (!hardcodeAction || actionNames.Count > 1)
            constraints.Add("action", String.Join("|", actionNames));

         foreach (var param in parameters.Where(p => p.Constraint != null)) {

            string regex = param.Constraint;

            if (param.IsOptional)
               regex = String.Concat("(", regex, ")?");

            constraints.Add(param.Name, regex);
         }

         var dataTokens = new RouteValueDictionary();
         dataTokens.Add("Namespaces", new string[1] { first.Controller.Type.Namespace });
         dataTokens.Add("BaseRoute", String.Join("/", first.Controller.BaseRouteSegments));

         Route route = new Route(url, defaults, constraints, dataTokens, new MvcRouteHandler());

         return route;
      }

      public static string ToCSharpMapRouteCalls(this RouteCollection routes) {

         if (routes == null) throw new ArgumentNullException("routes");

         StringBuilder sb = new StringBuilder();

         foreach (Route item in routes.OfType<Route>()) {

            string mapRoute = item.ToCSharpMapRouteCall();

            if (!String.IsNullOrEmpty(mapRoute)) {
               sb.Append(mapRoute)
                  .AppendLine()
                  .AppendLine(); 
            }
         }

         return sb.ToString();
      }

      public static string ToCSharpMapRouteCall(this Route route) {

         if (route == null) throw new ArgumentNullException("route");

         StringBuilder sb = new StringBuilder();

         Type handlerType = route.RouteHandler.GetType();

         if (typeof(StopRoutingHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.IgnoreRoute(\"{0}\");", route.Url);

         } else if (typeof(MvcRouteHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.MapRoute(null, \"{0}\"", route.Url);

            int i = 0;

            if (route.Defaults != null && route.Defaults.Count > 0) {

               sb.Append(", ")
                  .AppendLine()
                  .Append("   new { ");

               foreach (var item in route.Defaults) {

                  if (i > 0)
                     sb.Append(", ");

                  sb.AppendFormat("{0} = {1}", item.Key, ValueToCSharpString(item.Value));

                  i++;
               }

               sb.Append(" }");

               if (route.Constraints != null && route.Constraints.Count > 0) {

                  sb.Append(", ")
                        .AppendLine()
                        .Append("   new { ");

                  int j = 0;

                  foreach (var item in route.Constraints) {

                     if (j > 0)
                        sb.Append(", ");

                     sb.AppendFormat("{0} = {1}", item.Key, ValueToCSharpString(item.Value));

                     j++;
                  }

                  sb.Append(" }");
               }
            }

            string[] namespaces;

            if (route.DataTokens != null && (namespaces = route.DataTokens["Namespaces"] as string[]) != null) {

               sb.Append(", ")
                  .AppendLine()
                  .Append("   new[] { \"").Append(namespaces[0]).Append("\" }");
            }

            sb.Append(");");
         }

         return sb.ToString();
      }

      private static string ValueToCSharpString(object val) {

         string stringVal;

         if (val == null)
            stringVal = "null";

         else if (val.GetType() == typeof(string))
            stringVal = String.Concat("@\"", val, "\"");

         else if (val.GetType() == typeof(UrlParameter))
            stringVal = "UrlParameter.Optional";

         else
            stringVal = val.ToString();

         return stringVal;
      }

      public static void EnableCodeRouting(this ViewEngineCollection engines) {

         if (engines == null) throw new ArgumentNullException("engines");

         Type virtualPathProvType = typeof(VirtualPathProviderViewEngine);

         for (int i = 0; i < engines.Count; i++) {
            IViewEngine engine = engines[i];

            if (virtualPathProvType.IsAssignableFrom(engine.GetType()))
               engines[i] = new CodeRoutingViewEngineWrapper(engine);
         }
      }
   }

   [DebuggerDisplay("{ControllerUrl}")]
   class ControllerInfo {

      static readonly Type baseType = typeof(ControllerBase);

      string baseNamespace;
      string rootControllerName;
      Func<string, string> routeFormatter;

      ReadOnlyCollection<string> _BaseRouteSegments;

      public Type Type { get; private set; }
      public string DefaultActionName { get; private set; }

      public string Name {
         get {
            return routeFormatter(Type.Name.Substring(0, Type.Name.Length - 10));
         }
      }

      public bool IsInBaseNamespace {
         get {
            return Type.Namespace == baseNamespace 
               || IsInSubNamespace;
         }
      }

      public bool IsInSubNamespace {
         get {
            return Type.Namespace.Length > baseNamespace.Length
               && Type.Namespace.StartsWith(baseNamespace + ".");
         }
      }

      public bool IsRootController {
         get {
            return !String.IsNullOrWhiteSpace(rootControllerName)
               && BaseRouteSegments.Count == 0
               && NameEquals(Name, rootControllerName);
         }
      }

      public ReadOnlyCollection<string> BaseRouteSegments {
         get {
            if (_BaseRouteSegments == null) {
               var namespaceParts = new List<string>();
               
               if (IsInSubNamespace) {
                  namespaceParts.AddRange(
                     Type.Namespace.Remove(0, baseNamespace.Length + 1).Split('.')
                     .Select(s => routeFormatter(s))
                  );

                  if (namespaceParts.Count > 0 && NameEquals(namespaceParts.Last(), Name))
                     namespaceParts.RemoveAt(namespaceParts.Count - 1);
               }

               _BaseRouteSegments = new ReadOnlyCollection<string>(namespaceParts);
            }
            return _BaseRouteSegments;
         }
      }

      public string UrlTemplate {
         get { 
            return String.Join("/", BaseRouteSegments
               .Concat((!IsRootController) ? new[] { "{controller}" } : new string[0])
            ); 
         }
      }

      public string ControllerUrl {
         get {
            return String.Join("/", BaseRouteSegments
               .Concat((!IsRootController) ? new[] { Name } : new string[0])
            ); 
         }
      }

      public static IEnumerable<ControllerInfo> GetControllers(Assembly assembly, string baseNamespace, string rootController, string defaultAction, Func<string, string> routeFormatter) {

         return
            from t in assembly.GetTypes()
            where t.IsPublic
              && !t.IsAbstract
              && baseType.IsAssignableFrom(t)
              && t.Name.EndsWith("Controller")
            let controllerInfo =
              new ControllerInfo(t, baseNamespace, rootController, defaultAction, routeFormatter)
            where controllerInfo.IsInBaseNamespace
            select controllerInfo;
      }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      public ControllerInfo(Type type, string baseNamespace, string rootController, string defaultAction, Func<string, string> routeFormatter) {

         this.Type = type;
         this.baseNamespace = baseNamespace;
         this.rootControllerName = rootController;
         this.DefaultActionName = defaultAction;
         this.routeFormatter = routeFormatter;
      }

      public IEnumerable<ActionInfo> GetActions(Func<MethodInfo, string> actionNameExtractor, IDictionary<Type, string> defaultConstraints) { 
         
         bool controllerIsDisposable = typeof(IDisposable).IsAssignableFrom(this.Type);

         var actions =
             from m in this.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
             where !m.IsSpecialName
                && baseType.IsAssignableFrom(m.DeclaringType)
                && !Attribute.IsDefined(m, typeof(NonActionAttribute))
                && !(controllerIsDisposable && m.Name == "Dispose" && m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
             select new ActionInfo(m, this, routeFormatter, actionNameExtractor, defaultConstraints);

         CheckSingleDefaultRootController(actions);
         CheckNoAmbiguousUrls(actions);
         CheckOverloads(actions);

         return actions;
      }

      private void CheckSingleDefaultRootController(IEnumerable<ActionInfo> actions) {

         var rootControllers = actions
            .Select(a => a.Controller)
            .Where(c => c.IsRootController)
            .Distinct()
            .ToList();

         if (rootControllers.Count > 1) {

            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture,
                  "The default root controller is ambiguous between {0}.",
                  String.Join(" and ", rootControllers.Select(c => c.Type.FullName))
               )
            );
         }
      }

      private void CheckNoAmbiguousUrls(IEnumerable<ActionInfo> actions) {

         var ambiguousController =
            (from a in actions
             group a by a.ActionUrl into g
             where g.Count() > 1
             let distinctControllers = g.Select(a => a.Controller).Distinct().ToArray()
             where distinctControllers.Length > 1
             select new {
                ActionUrl = g.Key,
                DistinctControllers = distinctControllers
             }).ToList();

         if (ambiguousController.Count > 0) {
            var first = ambiguousController.First();

            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture,
                  "The URL '{0}' cannot be bound to more than one controller ({1}).",
                  first.ActionUrl,
                  String.Join(", ", first.DistinctControllers.Select(c => c.Type.FullName))
               )
            );
         }
      }

      private void CheckOverloads(IEnumerable<ActionInfo> actions) {
         
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
                  String.Join(", ", first.Select(a => String.Concat(a.Controller.Type.FullName, ".", a.Method.Name, "(", String.Join(", ", a.Method.GetParameters().Select(p => p.ParameterType.Name)), ")")))
               )
            );
         }

         var overloadsComparer = new RouteEqualityComparer();

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
                  String.Concat(first.Key.Controller.Type.FullName, ".", first.First().Method.Name)
               )
            );
         }
      }
   }

   [DebuggerDisplay("{ActionUrl}")]
   class ActionInfo {

      public string Name { get; private set; }
      public MethodInfo Method { get; private set; }
      public ControllerInfo Controller { get; private set; }
      public RouteParameterInfoCollection RouteParameters { get; private set; }

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

      public ActionInfo(MethodInfo method, ControllerInfo controller, Func<string, string> routeFormatter, Func<MethodInfo, string> actionNameExtractor, IDictionary<Type, string> defaultConstraints) {

         this.Method = method;
         this.Controller = controller;

         ActionNameAttribute nameAttr = Attribute.GetCustomAttribute(method, typeof(ActionNameAttribute)) as ActionNameAttribute;

         string actionName = (nameAttr != null) ? nameAttr.Name
            : (actionNameExtractor != null) ? actionNameExtractor(method)
            : method.Name;

         this.Name = routeFormatter(actionName);
         this.RouteParameters = new RouteParameterInfoCollection(
            method.GetParameters()
               .Where(p => Attribute.IsDefined(p, typeof(FromRouteAttribute)))
               .Select(p => new RouteParameterInfo(p, defaultConstraints))
               .ToList()
         );
      }
   }

   [DebuggerDisplay("{RouteSegment}")]
   class RouteParameterInfo : IEquatable<RouteParameterInfo> {

      string _Name, _Constraint;

      public string Name { get { return _Name; } }
      public string Constraint { get { return _Constraint; } }
      public bool IsOptional { get; internal set; }
      
      public string RouteSegment { get { return String.Concat("{", Name, "}"); } }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      public RouteParameterInfo(ParameterInfo param, IDictionary<Type, string> defaultConstraints) {
         
         Type paramType = param.ParameterType;
         bool isNullableValueType = paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(Nullable<>);

         this._Name = param.Name;
         
         this.IsOptional = param.IsOptional
            || Attribute.IsDefined(param, typeof(DefaultValueAttribute))
            || isNullableValueType;

         var routeAttr = (FromRouteAttribute)Attribute.GetCustomAttribute(param, typeof(FromRouteAttribute));

         string constr = routeAttr.Constraint;

         if (constr == null) {
            Type t = (isNullableValueType) ? Nullable.GetUnderlyingType(paramType) : paramType;
            defaultConstraints.TryGetValue(t, out constr);
         }

         this._Constraint = constr;
      }

      public bool Equals(RouteParameterInfo other) {

         if (other == null)
            return false;

         return NameEquals(this.Name, other.Name)
            && this.IsOptional == other.IsOptional
            && this.Constraint == other.Constraint;
      }

      public override bool Equals(object obj) {
         return Equals(obj as RouteParameterInfo);
      }

      public override int GetHashCode() {

         unchecked {
            int hash = 17;

            hash = hash * 23 + this.Name.GetHashCode();
            hash = hash * 23 + this.IsOptional.GetHashCode();
            hash = hash * 23 + ((this.Constraint != null) ? this.Constraint.GetHashCode() : 0);

            return hash;
         }
      }
   }

   class RouteParameterInfoCollection : ReadOnlyCollection<RouteParameterInfo>, IEquatable<RouteParameterInfoCollection> {

      public RouteParameterInfoCollection(IList<RouteParameterInfo> list)
         : base(list) { }

      public bool Equals(RouteParameterInfoCollection other) {

         if (other == null)
            return false;

         if (other.Count != this.Count)
            return false;

         for (int i = 0; i < this.Count; i++) {
            if (!this[i].Equals(other[i]))
               return false;
         }

         return true;
      }

      public override bool Equals(object obj) {
         return Equals(obj as RouteParameterInfoCollection);
      }

      public override int GetHashCode() {

         unchecked {
            int hash = 1;

            foreach (var item in this)
               hash = 31 * hash + (item == null ? 0 : item.GetHashCode());

            return hash;
         }
      }
   }

   class RouteEqualityComparer : IEqualityComparer<ActionInfo> {

      public bool Equals(ActionInfo x, ActionInfo y) {

         if (x == null)
            return y == null;

         if (y == null)
            return x == null;

         return CheckRouteParameters(x, y)
            && CheckRouteParameters(y, x);
      }

      private bool CheckRouteParameters(ActionInfo x, ActionInfo y) {

         for (int i = 0; i < x.RouteParameters.Count; i++) {
            var p = x.RouteParameters[i];

            if (y.RouteParameters.Count - 1 >= i) {
               var p2 = y.RouteParameters[i];

               if (!RouteParameterInfo.NameEquals(p.Name, p2.Name)
                  || p.Constraint != p2.Constraint)
                  return false;
            }
         }

         return true;
      }

      public int GetHashCode(ActionInfo obj) {
         throw new NotImplementedException();
      }
   }
}
