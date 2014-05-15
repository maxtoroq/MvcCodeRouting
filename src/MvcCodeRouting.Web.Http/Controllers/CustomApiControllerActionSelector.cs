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
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace MvcCodeRouting.Web.Http.Controllers {
   
   class CustomApiControllerActionSelector : ApiControllerActionSelector {

      public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext) {

         IHttpRouteData routeData = controllerContext.RouteData;
         bool containsAction = routeData.Values.ContainsKey("action");

         if (containsAction) {
            return base.SelectAction(controllerContext);
         }

         try {
            routeData.Values["action"] = controllerContext.Request.Method.Method;

            return base.SelectAction(controllerContext);

         } finally {

            routeData.Values.Remove("action");
         }
      }
   }
}
