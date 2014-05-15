// Copyright 2012 Max Toro Q.
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
using System.Collections.Concurrent;
using System.Linq;
using System.Web.Http.Routing;
using MvcCodeRouting.Controllers;
using MvcCodeRouting.Web.Http.Routing;

namespace MvcCodeRouting {
   
   class HttpRouteFactory : RouteFactory {

      static readonly ConcurrentDictionary<Type, Func<object, RegisterSettings, object>> _RouteConverters = 
         new ConcurrentDictionary<Type, Func<object, RegisterSettings, object>>();

      internal static ConcurrentDictionary<Type, Func<object, RegisterSettings, object>> RouteConverters {
         get { return _RouteConverters; }
      }

      public override object OptionalParameterValue {
         get { return System.Web.Http.RouteParameter.Optional; }
      }
      
      public override object CreateRoute(RouteSettings routeSettings, RegisterSettings registerSettings) {

         var defaults = new HttpRouteValueDictionary(routeSettings.Defaults);
         var constraints = new HttpRouteValueDictionary(routeSettings.Constraints);
         var dataTokens = new HttpRouteValueDictionary(routeSettings.DataTokens);

         return new CodeHttpRoute(routeSettings.RouteTemplate, defaults, constraints, dataTokens) {
            ActionMapping = routeSettings.ActionMapping,
            ControllerMapping = routeSettings.ControllerMapping,
            ControllerDescriptors = routeSettings.Actions
               .Select(a => a.Controller)
               .DistinctReference()
               .ToDictionary(c => c.Name, c => ((DescribedHttpControllerInfo)c).Descriptor)
         };
      }

      public override object ConvertRoute(object route, Type conversionType, RegisterSettings registerSettings) {

         Func<object, RegisterSettings, object> converterFn;

         if (!RouteConverters.TryGetValue(conversionType, out converterFn)) {
            return base.ConvertRoute(route, conversionType, registerSettings);
         }

         return converterFn(route, registerSettings);
      }
   }
}
