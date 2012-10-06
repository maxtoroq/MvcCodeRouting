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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Routing;

namespace MvcCodeRouting.Web.Http {
   
   public static class CodeRoutingHttpExtensions {

      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpRouteCollection routes, Type rootController) {
         return MapCodeRoutes(routes, rootController, null);
      }

      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpRouteCollection routes, Type rootController, CodeRoutingSettings settings) {
         return MapCodeRoutes(routes, null, rootController, settings);
      }

      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpRouteCollection routes, string baseRoute, Type rootController) {
         return MapCodeRoutes(routes, baseRoute, rootController, null);
      }

      public static ICollection<IHttpRoute> MapCodeRoutes(this HttpRouteCollection routes, string baseRoute, Type rootController, CodeRoutingSettings settings) {

         if (routes == null) throw new ArgumentNullException("routes");
         if (rootController == null) throw new ArgumentNullException("rootController");

         var registerSettings = new RegisterSettings(null, rootController, IsSupportedControllerSelfHost) {
            BaseRoute = baseRoute,
            Settings = settings
         };

         List<IHttpRoute> newRoutes = RouteFactory.CreateRoutes(registerSettings)
            .Cast<IHttpRoute>()
            .ToList();

         foreach (IHttpRoute route in newRoutes) {
            // TODO: in Web API v1 name cannot be null
            routes.Add((routes.Count + 1).ToString(CultureInfo.InvariantCulture), route);
         }

         return newRoutes;
      }

      static bool IsSupportedControllerSelfHost(Type type) {
         return typeof(ApiController).IsAssignableFrom(type);
      }
   }
}
