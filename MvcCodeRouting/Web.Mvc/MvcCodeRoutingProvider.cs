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
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {
   
   class MvcCodeRoutingProvider : CodeRoutingProvider {

      readonly RouteFactory _RouteFactory = new MvcRouteFactory();
      readonly Type _FromRouteAttributeType = typeof(MvcCodeRouting.FromRouteAttribute);
      readonly Type _CustomRouteAttributeType = typeof(MvcCodeRouting.CustomRouteAttribute);
      readonly Type _ActionOverloadDisambiguationAttributeType = typeof(MvcCodeRouting.RequireRouteParametersAttribute);

      public override RouteFactory RouteFactory {
         get { return _RouteFactory; }
      }

      public override bool CanDisambiguateActionOverloads {
         get { return false; }
      }

      public override Type FromRouteAttributeType {
         get { return _FromRouteAttributeType; }
      }

      public override Type CustomRouteAttributeType {
         get { return _CustomRouteAttributeType; }
      }

      public override Type ActionOverloadDisambiguationAttributeType {
         get { return _ActionOverloadDisambiguationAttributeType; }
      }

      protected override bool SupportsControllerType(Type controllerType) {
         return MvcControllerInfo.IsMvcController(controllerType);
      }

      protected override ControllerInfo CreateControllerInfo(Type controllerType, RegisterSettings registerSettings) {
         return MvcControllerInfo.Create(controllerType, registerSettings, this);
      }
   }
}
