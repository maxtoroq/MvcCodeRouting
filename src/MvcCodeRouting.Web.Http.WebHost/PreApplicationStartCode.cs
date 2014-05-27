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
using System.ComponentModel;
using System.Web.Http;
using System.Web.Routing;
using MvcCodeRouting.Web.Routing;

namespace MvcCodeRouting.Web.Http.WebHost {
   
   /// <summary>
   /// Executes the initialization code needed to use MvcCodeRouting with ASP.NET Web API on WebHost (System.Web).
   /// </summary>
   [EditorBrowsable(EditorBrowsableState.Never)]
   public static class PreApplicationStartCode {

      /// <summary>
      /// Executes the initialization code.
      /// </summary>
      public static void Start() {

         CodeRoutingExtensions.Initialize();
         CodeRoutingHttpExtensions.Initialize();

         CodeRoutingSettings.Defaults.HttpConfiguration(GlobalConfiguration.Configuration);

         HttpRouteFactory.RouteConverters[typeof(Route)] = CodeHttpWebRoute.ConvertToWebRoute;
      }
   }
}
