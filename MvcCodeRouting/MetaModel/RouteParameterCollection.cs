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
using System.Collections.ObjectModel;

namespace MvcCodeRouting.Controllers {

   class RouteParameterCollection : ReadOnlyCollection<RouteParameter>, IEquatable<RouteParameterCollection> {

      public RouteParameterCollection(IList<RouteParameter> list)
         : base(list) { }

      public bool Equals(RouteParameterCollection other) {

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
         return Equals(obj as RouteParameterCollection);
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
}
