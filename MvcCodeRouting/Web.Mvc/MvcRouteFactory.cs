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
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace MvcCodeRouting.Web.Mvc {
   
   class MvcRouteFactory : RouteFactory {

      static readonly Regex TokenPattern = new Regex(@"\{(.+?)\}");

      public override object OptionalParameterValue {
         get { return UrlParameter.Optional; }
      }

      public override object CreateRoute(RouteSettings routeSettings, RegisterSettings registerSettings) {

         ActionInfo first = routeSettings.Actions.First();
         string baseRoute = registerSettings.BaseRoute;

         var nonActionParameterTokens = new List<string>();

         if (baseRoute != null)
            nonActionParameterTokens.AddRange(TokenPattern.Matches(baseRoute).Cast<Match>().Select(m => m.Groups[1].Value));

         nonActionParameterTokens.AddRange(first.Controller.RouteProperties.Select(p => p.Name));

         return new CodeRoute(routeSettings.RouteTemplate, new MvcRouteHandler()) {
            Constraints = routeSettings.Constraints,
            DataTokens = routeSettings.DataTokens,
            Defaults = routeSettings.Defaults,
            ActionMapping = routeSettings.ActionMapping,
            ControllerMapping = routeSettings.ControllerMapping,
            NonActionParameterTokens = nonActionParameterTokens
         };
      }
   }
}
