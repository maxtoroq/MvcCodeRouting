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

namespace MvcCodeRouting.Controllers {

   class ActionSignatureComparer : IEqualityComparer<ActionInfo> {

      public bool Equals(ActionInfo x, ActionInfo y) {

         if (x == null)
            return y == null;

         if (y == null)
            return x == null;

         return CheckRouteParameters(x, y)
            && CheckRouteParameters(y, x);
      }

      static bool CheckRouteParameters(ActionInfo x, ActionInfo y) {

         for (int i = 0; i < x.RouteParameters.Count; i++) {
            var p = x.RouteParameters[i];

            if (y.RouteParameters.Count - 1 >= i) {
               var p2 = y.RouteParameters[i];

               if (!RouteParameter.NameEquals(p.Name, p2.Name)
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
