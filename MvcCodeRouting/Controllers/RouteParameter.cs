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
using System.Diagnostics;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Controllers {
   
   [DebuggerDisplay("{RouteSegment}")]
   class RouteParameter : IEquatable<RouteParameter> {

      public string Name { get; private set; }
      public string Constraint { get; private set; }
      public ParameterBinder Binder { get; private set; }
      public bool IsOptional { get; internal set; }
      public bool IsCatchAll { get; private set; }

      internal Type ParameterType { get; private set; }

      public string RouteSegment { get { return String.Concat("{", ((IsCatchAll) ? "*" : ""), Name, "}"); } }

      public static bool NameEquals(string name1, string name2) {
         return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }

      public RouteParameter(string name, Type parameterType, string constraint = null, bool isOptional = false, bool isCatchAll = false, ParameterBinder binder = null) {

         if (name == null) throw new ArgumentNullException("name");
         if (parameterType == null) throw new ArgumentNullException("parameterType");

         this.Name = name;
         this.Constraint = constraint;
         this.IsOptional = isOptional;
         this.IsCatchAll = isCatchAll;
         this.Binder = binder;
         this.ParameterType = parameterType;
      }

      public bool Equals(RouteParameter other) {

         if (other == null)
            return false;

         return NameEquals(this.Name, other.Name)
            && this.IsOptional == other.IsOptional
            && this.Constraint == other.Constraint
            && Object.Equals(this.Binder, other.Binder);
      }

      public override bool Equals(object obj) {
         return Equals(obj as RouteParameter);
      }

      public override int GetHashCode() {

         unchecked {
            int hash = 17;

            hash = hash * 23 + this.Name.GetHashCode();
            hash = hash * 23 + this.IsOptional.GetHashCode();
            hash = hash * 23 + ((this.Constraint != null) ? this.Constraint.GetHashCode() : 0);
            hash = hash * 23 + ((this.Binder != null) ? this.Binder.GetHashCode() : 0);

            return hash;
         }
      }
   }
}
