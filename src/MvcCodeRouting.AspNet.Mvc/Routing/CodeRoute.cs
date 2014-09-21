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
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;

namespace MvcCodeRouting.AspNet.Routing {
   
   class CodeRoute : TemplateRoute, ICodeRoute {

      public IDictionary<string, object> DataTokens {
         get { throw new NotImplementedException(); }
      }

      public IDictionary<string, string> ControllerMapping {
         get { throw new NotImplementedException(); }
      }

      public IDictionary<string, string> ActionMapping {
         get { throw new NotImplementedException(); }
      }

      public CodeRoute(IRouter target, string routeTemplate, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
         : base(default(IRouter), routeTemplate, defaults, constraints, default(IInlineConstraintResolver)) { }

      public string GetVirtualPath(VirtualPathContext context) {

         var actionContext = (ActionContext)context.Context.RequestServices.GetService(typeof(ActionContext));

         RouteData currentRouteData = actionContext.RouteData;
         TemplateRoute currentRoute = (TemplateRoute)currentRouteData.Routers.First();

         return this.DoGetVirtualPath(context.ProvidedValues, context.AmbientValues, /* TODO: currentRoute.DataTokens */null, () => base.GetVirtualPath(context));
      }

      public Task RouteAsync(RouteContext context) {
         throw new NotImplementedException();
      }
   }
}
