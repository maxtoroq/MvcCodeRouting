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
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Web.Routing {
   
   class ParameterBindingRouteConstraint : TypeAwareRouteConstraint {
      
      readonly ParameterBinder _Binder;

      public ParameterBinder Binder {
         get { return _Binder; }
      }

      public ParameterBindingRouteConstraint(ParameterBinder binder) 
         : base(binder.ParameterType) {

         if (binder == null) throw new ArgumentNullException("binder");

         _Binder = binder;
      }

      protected override bool TryParse(HttpContextBase httpContext, string parameterName, object rawValue, string attemptedValue, CultureInfo culture, out object result) {
         return this.Binder.TryBind(attemptedValue, culture, out result);
      }
   }
}
