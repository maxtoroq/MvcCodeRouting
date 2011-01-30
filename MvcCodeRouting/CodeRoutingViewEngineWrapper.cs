// Copyright 2011 Max Toro Q.
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
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {
   
   public class CodeRoutingViewEngineWrapper : IViewEngine {

      readonly IViewEngine wrappedEngine;

      public CodeRoutingViewEngineWrapper(IViewEngine wrappedEngine) {

         if (wrappedEngine == null) throw new ArgumentNullException("wrappedEngine");

         this.wrappedEngine = wrappedEngine;
      }

      public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
         return FindView(controllerContext, partialViewName, null, useCache, true);
      }

      public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
         return FindView(controllerContext, viewName, masterName, useCache, false);
      }

      private ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache, bool partial) {

         if (controllerContext == null) throw new ArgumentNullException("controllerContext");

         RouteData routeData = controllerContext.RouteData;

         string baseRoute = routeData.DataTokens["BaseRoute"] as string;

         ViewEngineResult result;

         if (!String.IsNullOrEmpty(baseRoute)) {

            string controller = routeData.GetRequiredString("controller");

            string baseRoutePlusController = String.Concat(baseRoute, "/", controller);

            routeData.Values["controller"] = baseRoutePlusController;

            if (partial) 
               result = this.wrappedEngine.FindPartialView(controllerContext, viewName, useCache);
            else 
               result = this.wrappedEngine.FindView(controllerContext, viewName, masterName, useCache);

            routeData.Values["controller"] = controller;

         } else {
            if (partial) 
               result = this.wrappedEngine.FindPartialView(controllerContext, viewName, useCache);
            else 
               result = this.wrappedEngine.FindView(controllerContext, viewName, masterName, useCache);
         }

         return result;
      }

      public void ReleaseView(ControllerContext controllerContext, IView view) {
         this.wrappedEngine.ReleaseView(controllerContext, view);
      }
   }
}
