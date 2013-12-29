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
using MvcCodeRouting.Controllers;
using MvcCodeRouting.ParameterBinding;
using MvcCodeRouting.Web.Routing;

namespace MvcCodeRouting.Web.Mvc {
   
   class MvcCodeRoutingProvider : CodeRoutingProvider {

      readonly RouteFactory _RouteFactory = new MvcRouteFactory();

#pragma warning disable 0618

      readonly Type _FromRouteAttributeType = typeof(MvcCodeRouting.FromRouteAttribute);
      readonly Type _CustomRouteAttributeType = typeof(MvcCodeRouting.CustomRouteAttribute);
      readonly Type _DefaultActionAttributeType = typeof(DefaultActionAttribute);
      readonly Type _ActionOverloadDisambiguationAttributeType = typeof(MvcCodeRouting.RequireRouteParametersAttribute);

#pragma warning restore 0618

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

      public override Type DefaultActionAttributeType {
         get { return _DefaultActionAttributeType; }
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

      public override object CreateParameterBindingRouteConstraint(ParameterBinder binder) {
         return new ParameterBindingRouteConstraint(binder);
      }

      public override object CreateRegexRouteConstraint(string pattern, Type parameterType) {
         return new RegexRouteConstraint(pattern);
      }

      public override object CreateSetRouteConstraint(string[] values) {
         return new SetRouteConstraint(values);
      }
   }
}
