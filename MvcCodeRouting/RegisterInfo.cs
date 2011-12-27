﻿// Copyright 2011 Max Toro Q.
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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace MvcCodeRouting {
   
   class RegisterInfo {

      Assembly _Assembly;
      CodeRoutingSettings _Settings;
      string _RootNamespace;
      string _ViewsLocation;

      public Assembly Assembly {
         get {
            if (_Assembly == null) 
               _Assembly = this.RootController.Assembly;
            return _Assembly;
         }
         private set {
            _Assembly = value;
         }
      }

      public Type RootController { get; private set; }
      public string BaseRoute { get; set; }
      
      public CodeRoutingSettings Settings {
         get {
            if (_Settings == null) 
               _Settings = new CodeRoutingSettings();
            return _Settings;
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
               _ViewsLocation = (!String.IsNullOrEmpty(this.BaseRoute)) ? 
                  String.Join("/", this.BaseRoute.Split('/').Where(s => !s.Contains('{'))) 
                  : "";
            }
            return _ViewsLocation;
         }
      }

      public RegisterInfo(Assembly assembly, Type rootController) {

         if (rootController != null) { 
            
            if (!IsController(rootController))
               throw new InvalidOperationException("The specified root controller is not a valid controller type.");

            if (assembly != null && rootController.Assembly != assembly)
               throw new InvalidOperationException("The specified root controller does not belong to the specified assembly.");
         
         } else if (assembly == null) {
            throw new ArgumentException("Either assembly or rootController must be specified.");
         }

         this.Assembly = assembly;
         this.RootController = rootController;
      }

      public IEnumerable<ControllerInfo> GetControllers() {

         return
            from t in GetControllerTypes()
            let c = new ControllerInfo(t, this)
            where !this.Settings.IgnoredControllers.Contains(c.Type)
              && c.IsInRootNamespace
            select c;
      }

      IEnumerable<Type> GetControllerTypes() {

         return
            from t in this.Assembly.GetTypes()
            where IsController(t)
            select t;
      }

      static bool IsController(Type t) {

         return t.IsPublic
            && !t.IsAbstract
            && ControllerInfo.BaseType.IsAssignableFrom(t)
            && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);
      }
   }
}
