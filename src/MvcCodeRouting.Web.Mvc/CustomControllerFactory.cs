// Copyright 2014 Max Toro Q.
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
using MvcCodeRouting.Web.Routing;

namespace MvcCodeRouting.Web.Mvc {
   
   class CustomControllerFactory : DefaultControllerFactory {

      protected override Type GetControllerType(RequestContext requestContext, string controllerName) {

         CodeRoute codeRoute = requestContext.RouteData.Route as CodeRoute;

         if (codeRoute == null
            || codeRoute.ControllerTypes == null
            || codeRoute.ControllerTypes.Count == 0) {

            return base.GetControllerType(requestContext, controllerName);
         }

         Type controllerType;

         if (codeRoute.ControllerTypes.TryGetValue(controllerName, out controllerType)) {
            return controllerType;
         }

         return null;
      }
   }
}
