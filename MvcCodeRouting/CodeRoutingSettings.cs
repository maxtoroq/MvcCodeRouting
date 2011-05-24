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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;

namespace MvcCodeRouting {
   
   public class CodeRoutingSettings {

      Func<string, string> _RouteFormatter;
      readonly IDictionary<Type, string> _DefaultConstraints;
      readonly Collection<Type> _IgnoredControllers;

      public string RootController { get; set; }

      public string DefaultAction { get; set; }

      public IDictionary<Type, string> DefaultConstraints {
         get { return _DefaultConstraints; }
      }

      public Collection<Type> IgnoredControllers { 
         get { return _IgnoredControllers; } 
      }

      public Func<string, string> RouteFormatter {
         get {
            if (_RouteFormatter == null)
               _RouteFormatter = s => s;
            return _RouteFormatter;
         }
         set { _RouteFormatter = value; }
      }

      public Func<MethodInfo, string> ActionNameExtractor { get; set; }

      public CodeRoutingSettings() {

         this.RootController = "Home";
         this.DefaultAction = "Index";

         _IgnoredControllers = new Collection<Type>();

         _DefaultConstraints = new Dictionary<Type, string>();
         _DefaultConstraints.Add(typeof(Boolean), "true|false");
         _DefaultConstraints.Add(typeof(Guid), @"\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Za-z0-9]{12}\b");

         foreach (var type in new[] { typeof(Decimal), typeof(Double), typeof(Single) })
            _DefaultConstraints.Add(type, @"-?(0|[1-9]\d*)(\.\d+)?");

         foreach (var type in new[] { typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64) })
            _DefaultConstraints.Add(type, @"0|-?[1-9]\d*");

         foreach (var type in new[] { typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64) })
            _DefaultConstraints.Add(type, @"0|[1-9]\d*");
      }
   }
}
