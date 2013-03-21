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

namespace MvcCodeRouting {
   
   abstract class CodeRoutingProvider {

      static readonly List<CodeRoutingProvider> providers = new List<CodeRoutingProvider>();
      static readonly object staticLock = new object();

      public abstract RouteFactory RouteFactory { get; }
      public abstract bool CanDisambiguateActionOverloads { get; }
      public abstract Type FromRouteAttributeType { get; }
      public abstract Type CustomRouteAttributeType { get; }
      
      public virtual Type ActionOverloadDisambiguationAttributeType {
         get { return null; }
      }

      public static void RegisterProvider(CodeRoutingProvider provider) {

         if (provider == null) throw new ArgumentNullException("provider");

         lock (staticLock) 
            providers.Add(provider);
      }

      public static CodeRoutingProvider GetProviderForControllerType(Type controllerType) {

         foreach (CodeRoutingProvider provider in providers) {

            if (provider.SupportsControllerType(controllerType))
               return provider;
         }

         return null;
      }

      public static ControllerInfo AnalyzeControllerType(Type controllerType, RegisterSettings registerSettings) {

         foreach (CodeRoutingProvider provider in providers) {

            if (provider.SupportsControllerType(controllerType))
               return provider.CreateControllerInfo(controllerType, registerSettings);
         }

         return null;
      }

      protected abstract bool SupportsControllerType(Type controllerType);
      protected abstract ControllerInfo CreateControllerInfo(Type controllerType, RegisterSettings registerSettings);
   }
}
