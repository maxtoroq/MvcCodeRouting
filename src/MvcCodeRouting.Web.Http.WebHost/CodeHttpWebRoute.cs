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
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using MvcCodeRouting.Web.Http.Routing;

namespace MvcCodeRouting.Web.Routing {
   
   class CodeHttpWebRoute : CodeRoute {

      // originalRoute is System.Web.Http.WebHost.Routing.HttpWebRoute
      // with HttpRoute property set to CodeHttpRoute
      // Web API requires RouteData.Route and VirtualPathData.Route to be an instance of that type
      // The problem with HttpWebRoute is that it's not extensible
      // that's why we replace it with CodeHttpWebRoute in MapCodeRoutes

      readonly Route originalRoute;

      public static Route ConvertToWebRoute(object route, RegisterSettings registerSettings) {

         HttpConfiguration httpConfig = registerSettings.Settings.HttpConfiguration();

         CodeHttpRoute httpRoute = (CodeHttpRoute)route;

         // httpWebRoute is System.Web.Http.WebHost.Routing.HttpWebRoute
         // with HttpRoute property set to httpRoute

         GlobalConfiguration.Configuration.Routes.Add(null, httpRoute);
         Route httpWebRoute = (Route)RouteTable.Routes.Last();
         RouteTable.Routes.RemoveAt(RouteTable.Routes.Count - 1);

         foreach (var item in httpWebRoute.Constraints.ToArray()) {

            var paramBindConstraint = item.Value as Web.Http.Routing.ParameterBindingRouteConstraint;

            if (paramBindConstraint != null) {
               httpWebRoute.Constraints[item.Key] = new ParameterBindingRouteConstraint(paramBindConstraint.Binder);
               continue;
            }

            var regexConstraint = item.Value as Web.Http.Routing.RegexRouteConstraint;

            if (regexConstraint != null) {
               httpWebRoute.Constraints[item.Key] = new RegexRouteConstraint(regexConstraint.Regex);
               continue;
            }

            var setConstraint = item.Value as Web.Http.Routing.SetRouteConstraint;

            if (setConstraint != null) {
               httpWebRoute.Constraints[item.Key] = new SetRouteConstraint(setConstraint.GetValues());
               continue;
            }
         }

         var codeWebRoute = new CodeHttpWebRoute(httpWebRoute, httpRoute);

         CodeRoutingHttpExtensions.EnableCodeRouting(httpConfig);

         return codeWebRoute;
      }

      public CodeHttpWebRoute(Route originalRoute, CodeHttpRoute httpRoute) 
         : base(originalRoute.Url, originalRoute.Defaults, originalRoute.Constraints, originalRoute.DataTokens, originalRoute.RouteHandler) {

         this.originalRoute = originalRoute;

         this.ActionMapping = httpRoute.ActionMapping;
         this.ControllerMapping = httpRoute.ControllerMapping;
      }
      
      public override RouteData GetRouteData(HttpContextBase httpContext) {
         
         RouteData data = base.GetRouteData(httpContext);

         if (data != null) {
            data.Route = this.originalRoute;
         }

         return data;
      }

      public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {

         if (values == null || !values.ContainsKey("httproute")) {
            return null;
         }

         values = new RouteValueDictionary(values);
         values.Remove("httproute");

         VirtualPathData data = base.GetVirtualPath(requestContext, values);

         if (data != null) {
            data.Route = this.originalRoute;
         }

         return data;
      }
   }
}
