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
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace MvcCodeRouting.Web.Http.Routing {

   [DebuggerDisplay("{RouteTemplate}")]
   class CodeHttpRoute : HttpRoute, ICodeRoute {
      
      public IDictionary<string, string> ActionMapping { get; set; }
      public IDictionary<string, string> ControllerMapping { get; set; }
      public IDictionary<string, HttpControllerDescriptor> ControllerDescriptors { get; set; }

      public CodeHttpRoute(string routeTemplate, HttpRouteValueDictionary defaults, HttpRouteValueDictionary constraints, HttpRouteValueDictionary dataTokens)
         : base(routeTemplate, defaults, constraints, dataTokens) { }

      public override IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request) {

         IHttpRouteData data = base.GetRouteData(virtualPathRoot, request);

         if (data != null)
            this.IncomingMapping(data.Values);

         return data;
      }

      public override IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values) {
         
         IHttpRouteData requestRouteData = request.GetRouteData();

         return this.DoGetVirtualPath(values, requestRouteData.Values, requestRouteData.Route.DataTokens, () => base.GetVirtualPath(request, values));
      }
   }
}
