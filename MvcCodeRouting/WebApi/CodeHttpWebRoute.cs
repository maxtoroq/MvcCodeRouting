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
using System.Web.Routing;

namespace MvcCodeRouting.WebApi {
   
   class CodeHttpWebRoute : CodeRoute {

      // originalRoute is System.Web.Http.WebHost.Routing.HttpWebRoute
      // Web API requires RouteData.Route and VirtualPathData.Route to be an instance of that type
      // The problem with HttpWebRoute is that it's not extensible
      // that's why we replace it with CodeHttpWebRoute in MapCodeRoutes

      readonly Route originalRoute;

      public CodeHttpWebRoute(Route originalRoute, CodeHttpRoute httpRoute) 
         : base(originalRoute.Url, originalRoute.RouteHandler) {

         this.originalRoute = originalRoute;
         this.Constraints = originalRoute.Constraints;
         this.DataTokens = originalRoute.DataTokens;
         this.Defaults = originalRoute.Defaults;

         this.ActionMapping = httpRoute.ActionMapping;
         this.ControllerMapping = httpRoute.ControllerMapping;
      }
      
      public override RouteData GetRouteData(System.Web.HttpContextBase httpContext) {
         
         RouteData data = base.GetRouteData(httpContext);

         if (data != null) 
            data.Route = this.originalRoute;

         return data;
      }

      public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {

         if (values == null || !values.ContainsKey("httproute")) 
            return null;

         values = new RouteValueDictionary(values);
         values.Remove("httproute");

         VirtualPathData data = base.GetVirtualPath(requestContext, values);

         if (data != null) 
            data.Route = this.originalRoute;

         return data;
      }
   }
}
