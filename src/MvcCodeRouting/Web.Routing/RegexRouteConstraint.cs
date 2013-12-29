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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace MvcCodeRouting.Web.Routing {
   
   class RegexRouteConstraint : IRouteConstraint {

      readonly Regex _Regex;
      internal readonly string OriginalPattern;

      public Regex Regex {
         get { return _Regex; }
      }

      public RegexRouteConstraint(string pattern) {

         if (pattern == null) throw new ArgumentNullException("pattern");

         OriginalPattern = pattern;
         _Regex = new Regex("^(" + pattern + ")$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
      }

      internal RegexRouteConstraint(Regex regex) {

         if (regex == null) throw new ArgumentNullException("regex");

         _Regex = regex;
      }

      public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {

         object rawValue;

         if (!values.TryGetValue(parameterName, out rawValue)
            || rawValue == null) {

            return true;
         }

         string attemptedValue = Convert.ToString(rawValue, CultureInfo.InvariantCulture);

         if (attemptedValue.Length == 0) {
            return true;
         }

         return this.Regex.IsMatch(attemptedValue);
      }
   }
}
