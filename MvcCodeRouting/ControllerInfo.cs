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
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Globalization;

namespace MvcCodeRouting {
   
   [DebuggerDisplay("{ControllerUrl}")]
   class ControllerInfo {

      static readonly Type baseType = typeof(ControllerBase);

      readonly CodeRoutingSettings settings;
      readonly string baseNamespace;

      ReadOnlyCollection<string> _BaseRouteSegments;

      public Type Type { get; private set; }
      public string DefaultActionName { get; private set; }

      public string Name {
         get {
            return settings.RouteFormatter(Type.Name.Substring(0, Type.Name.Length - 10));
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
            return !String.IsNullOrWhiteSpace(settings.RootController)
               && BaseRouteSegments.Count == 0
               && NameEquals(Name, settings.RootController);
         }
      }

      public ReadOnlyCollection<string> BaseRouteSegments {
         get {
            if (_BaseRouteSegments == null) {
               var namespaceParts = new List<string>();

               if (IsInSubNamespace) {
                  namespaceParts.AddRange(
                     Type.Namespace.Remove(0, baseNamespace.Length + 1).Split('.')
                     .Select(s => settings.RouteFormatter(s))
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

      public static IEnumerable<ControllerInfo> GetControllers(Assembly assembly, string baseNamespace, CodeRoutingSettings settings) {

         return
            from t in assembly.GetTypes()
            where t.IsPublic
              && !t.IsAbstract
              && baseType.IsAssignableFrom(t)
              && t.Name.EndsWith("Controller")
              && !settings.IgnoredControllers.Contains(t)
            let controllerInfo = new ControllerInfo(t, baseNamespace, settings)
            where controllerInfo.IsInBaseNamespace
            select controllerInfo;
      }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      public ControllerInfo(Type type, string baseNamespace, CodeRoutingSettings settings) {

         this.Type = type;
         this.baseNamespace = baseNamespace;
         this.settings = settings;
         this.DefaultActionName = settings.DefaultAction;
      }

      public IEnumerable<ActionInfo> GetActions() {

         bool controllerIsDisposable = typeof(IDisposable).IsAssignableFrom(this.Type);

         var actions =
             from m in this.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
             where !m.IsSpecialName
                && baseType.IsAssignableFrom(m.DeclaringType)
                && !Attribute.IsDefined(m, typeof(NonActionAttribute))
                && !(controllerIsDisposable && m.Name == "Dispose" && m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
             select new ActionInfo(m, this, this.settings);

         CheckOverloads(actions);

         return actions;
      }

      void CheckOverloads(IEnumerable<ActionInfo> actions) {

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
                  String.Join(", ", first.Select(a => String.Concat(a.Method.DeclaringType.FullName, ".", a.Method.Name, "(", String.Join(", ", a.Method.GetParameters().Select(p => p.ParameterType.Name)), ")")))
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
}
