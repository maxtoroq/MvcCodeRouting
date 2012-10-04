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
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web {
   
   class RouteSettings {

      readonly RouteValueDictionary _Defaults = new RouteValueDictionary();
      readonly RouteValueDictionary _Constraints = new RouteValueDictionary();
      readonly RouteValueDictionary _DataTokens = new RouteValueDictionary();

      public ICollection<ActionInfo> Actions { get; private set; }
      public string RouteTemplate { get; private set; }

      public RouteValueDictionary Defaults { get { return _Defaults; } }
      public RouteValueDictionary Constraints { get { return _Constraints; } }
      public RouteValueDictionary DataTokens { get { return _DataTokens; } }

      public IDictionary<string, string> ControllerMapping { get; set; }
      public IDictionary<string, string> ActionMapping { get; set; }

      public RouteSettings(string routeTemplate, IEnumerable<ActionInfo> actions) {
         
         this.RouteTemplate = routeTemplate;
         this.Actions = actions.ToArray();
      }
   }
}
