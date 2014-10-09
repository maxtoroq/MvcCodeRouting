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
using System.Web;
using System.Web.Routing;

namespace MvcCodeRouting.Web.Routing {
   
   abstract class TypeAwareRouteConstraint : IRouteConstraint {

      readonly Type _ParameterType;

      public Type ParameterType {
         get { return _ParameterType; }
      }

      protected TypeAwareRouteConstraint(Type parameterType) {
         
         if (parameterType == null) throw new ArgumentNullException("parameterType");

         _ParameterType = parameterType;
      }

      protected abstract bool TryParse(HttpContextBase httpContext, string parameterName, object rawValue, string attemptedValue, CultureInfo culture, out object result);

      public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {

         object rawValue;

         if (!values.TryGetValue(parameterName, out rawValue)
            || rawValue == null) {

            return true;
         }

         if (this.ParameterType.IsInstanceOfType(rawValue)) {
            return true;
         }

         string attemptedValue = Convert.ToString(rawValue, CultureInfo.InvariantCulture);

         if (attemptedValue.Length == 0) {
            return true;
         }

         object parsedVal;

         if (!TryParse(httpContext, parameterName, rawValue, attemptedValue, CultureInfo.InvariantCulture, out parsedVal)) {
            return false;
         }

         if (routeDirection == RouteDirection.IncomingRequest) {
            values[parameterName] = parsedVal;
         }

         return true;
      }
   }
}
