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
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace MvcCodeRouting.Web.Http {
   
   public static class CodeRoutingHttpExtensions {

      public static HttpRouteMapper EnableCodeRouting(this HttpConfiguration configuration) {
         
         configuration.Services.Replace(typeof(IHttpControllerSelector), new CustomControllerSelector(configuration));

         return new HttpRouteMapper(configuration);
      }
   }
}
