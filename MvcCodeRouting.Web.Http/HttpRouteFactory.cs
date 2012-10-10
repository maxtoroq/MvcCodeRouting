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
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Http {
   
   class HttpRouteFactory : RouteFactory {

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

         if (conversionType != typeof(Route))
            return base.ConvertRoute(route, conversionType, registerSettings);

         HttpConfiguration httpConfig = (HttpConfiguration)registerSettings.HttpConfiguration;
         RouteCollection routes = registerSettings.RouteCollection;

         HttpConfiguration globalHttpConfig = GlobalConfiguration.Configuration;
         RouteCollection globalRoutes = RouteTable.Routes;

         bool httpConfigIsGlobal = Object.ReferenceEquals(httpConfig, globalHttpConfig);
         bool routesIsGlobal = Object.ReferenceEquals(routes, globalRoutes);

         string name = (httpConfigIsGlobal) ?
            null
            : Guid.NewGuid().ToString();

         httpConfig.Routes.Add(name, (IHttpRoute)route);

         var httpRoute = (Web.Http.CodeHttpRoute)route;

         // System.Web.Http.WebHost.Routing.HttpWebRoute
         Route webRoute;

         if (routesIsGlobal) {

            if (httpConfigIsGlobal) {
               webRoute = (Route)routes.Last();

            } else {
               globalHttpConfig.Routes.Add(name, httpRoute);
               webRoute = (Route)routes.Last();
               globalHttpConfig.Routes.Remove(name);
            }

         } else {

            if (httpConfigIsGlobal) {
               webRoute = (Route)globalRoutes.Last();

            } else {
               globalHttpConfig.Routes.Add(name, httpRoute);
               webRoute = (Route)globalRoutes.Last();
               globalHttpConfig.Routes.Remove(name);
            }

            globalRoutes.RemoveAt(globalRoutes.Count - 1);
         }

         routes.RemoveAt(routes.Count - 1);

         var codeRoute = new Web.Http.WebHost.CodeHttpWebRoute(webRoute, httpRoute);

         CodeRoutingHttpExtensions.EnableCodeRouting(httpConfig);

         return codeRoute;
      }
   }
}
