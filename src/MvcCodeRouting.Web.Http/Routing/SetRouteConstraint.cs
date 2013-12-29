// Copyright 2013 Max Toro Q.
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
using System.Net.Http;
using System.Web.Http.Routing;

namespace MvcCodeRouting.Web.Http.Routing {
   
   class SetRouteConstraint : IHttpRouteConstraint {

      readonly HashSet<string> set;

      public SetRouteConstraint(params string[] values) {

         if (values == null) {
            values = new string[0];
         }

         this.set = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
      }

      public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection) {

         object rawValue;

         if (!values.TryGetValue(parameterName, out rawValue)
            || rawValue == null) {

            return true;
         }

         string attemptedValue = Convert.ToString(rawValue, CultureInfo.InvariantCulture);

         if (attemptedValue.Length == 0) {
            return true;
         }

         return this.set.Contains(attemptedValue);
      }

      public string[] GetValues() {
         return this.set.ToArray();
      }
   }
}
