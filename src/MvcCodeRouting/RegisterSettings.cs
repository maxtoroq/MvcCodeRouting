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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting {
   
   class RegisterSettings {

      string _BaseRoute;
      Assembly _Assembly;
      CodeRoutingSettings _Settings;
      string _RootNamespace;
      string _ViewsLocation;

      public string BaseRoute {
         get { return _BaseRoute; }
         set {
            if (!String.IsNullOrWhiteSpace(value)) {

               value = value.Trim();

               if (new[] { '~', '/' }.Any(c => value[0] == c)) {
                  throw new ArgumentException(
                     String.Format(CultureInfo.InvariantCulture, "Base route cannot start with '{0}'.", value[0])
                  );
               }

               _BaseRoute = value;
            }
         }
      }

      public Assembly Assembly {
         get {
            return _Assembly
               ?? (_Assembly = this.RootController.Assembly);
         }
         private set {
            _Assembly = value;
         }
      }

      public Type RootController { get; private set; }
      
      public CodeRoutingSettings Settings {
         get {
            return _Settings
               ?? (_Settings = new CodeRoutingSettings());
         }
         set { _Settings = value; }
      }

      public string RootNamespace {
         get {
            if (_RootNamespace == null) {

               if (this.RootController != null) {
                  _RootNamespace = this.RootController.Namespace;
               } else {
                  throw new InvalidOperationException();
               }
            }

            return _RootNamespace;
         }
      }

      public string ViewsLocation {
         get {
            if (_ViewsLocation == null) {
               _ViewsLocation = (this.BaseRoute != null) ? 
                  String.Join("/", this.BaseRoute.Split('/').Where(s => !s.Contains('{'))) 
                  : "";
            }

            return _ViewsLocation;
         }
      }

      public RegisterSettings(Assembly assembly, Type rootController) {

         if (rootController != null) {

            if (CodeRoutingProvider.GetProviderForControllerType(rootController) == null
               || !IsValidControllerType(rootController)) {

               throw new InvalidOperationException("The specified root controller is not a valid controller type.");
            }

            if (assembly != null 
               && rootController.Assembly != assembly) {

               throw new InvalidOperationException("The specified root controller does not belong to the specified assembly.");
            }

         } else if (assembly == null) {
            throw new ArgumentException("Either assembly or rootController must be specified.");
         }

         this.Assembly = assembly;
         this.RootController = rootController;
      }

      public IEnumerable<ControllerInfo> GetControllers() {

         return
            from t in GetControllerTypes()
            where !this.Settings.IgnoredControllers.Contains(t)
            let c = CodeRoutingProvider.AnalyzeControllerType(t, this)
            where c != null 
               && c.IsInRootNamespace
            select c;
      }

      IEnumerable<Type> GetControllerTypes() {

         Type[] types = (this.Settings.RootOnly) ?
            new[] { this.RootController }
            : this.Assembly.GetTypes();

         return types.Where(t => IsValidControllerType(t));
      }

      static bool IsValidControllerType(Type type) {

         return type.IsPublic
            && !type.IsAbstract
            && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);
      }
   }
}
