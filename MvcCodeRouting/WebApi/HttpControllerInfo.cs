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
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MvcCodeRouting.Web.Http {
   
   abstract class HttpControllerInfo : ControllerInfo {

      readonly RouteFactory _RouteFactory = new HttpRouteFactory();
      readonly Type _FromRouteAttributeType = typeof(FromRouteAttribute);
      readonly Type _CustomRouteAttributeType = typeof(CustomRouteAttribute);

      public override RouteFactory RouteFactory {
         get { return _RouteFactory; }
      }

      public override bool CanDisambiguateActionOverloads {
         get { return true; }
      }

      public override Type FromRouteAttributeType {
         get { return _FromRouteAttributeType; }
      }

      public override Type CustomRouteAttributeType {
         get { return _CustomRouteAttributeType; }
      }

      public static new ControllerInfo Create(Type controllerType, RegisterSettings registerSettings) {

         // TODO: Remove GlobalConfiguration dependency

         return new DescribedHttpControllerInfo(
            new HttpControllerDescriptor(
               GlobalConfiguration.Configuration,
               controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length),
               controllerType),
            controllerType,
            registerSettings
         );
      }

      public HttpControllerInfo(Type type, RegisterSettings registerSettings) 
         : base(type, registerSettings) { }

      protected override bool IsNonAction(ICustomAttributeProvider action) {
         return action.IsDefined(typeof(NonActionAttribute), inherit: true);
      }
   }
}
