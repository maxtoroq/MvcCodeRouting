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
using System.Web.Http.WebHost;
using System.Web.Routing;

namespace MvcCodeRouting.Web.Http.WebHost {
   
   class CodeHttpWebRoute : CodeRoute {

      public CodeHttpWebRoute(CodeHttpRoute httpRoute)
         : base(httpRoute.RouteTemplate, new RouteValueDictionary(httpRoute.Defaults), new RouteValueDictionary(httpRoute.Constraints), new RouteValueDictionary(httpRoute.DataTokens), HttpControllerRouteHandler.Instance) {

         this.ActionMapping = httpRoute.ActionMapping;
         this.ControllerMapping = httpRoute.ControllerMapping;
      }
      
      public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {

         if (values == null || !values.ContainsKey("httproute")) 
            return null;

         values = new RouteValueDictionary(values);
         values.Remove("httproute");

         VirtualPathData data = base.GetVirtualPath(requestContext, values);

         return data;
      }
   }
}
