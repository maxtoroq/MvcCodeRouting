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
using MvcCodeRouting.Controllers;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting {
   
   abstract class CodeRoutingProvider {

      static readonly List<CodeRoutingProvider> providers = new List<CodeRoutingProvider>();
      static readonly object staticLock = new object();

      public abstract RouteFactory RouteFactory { get; }
      
      public abstract Type FromRouteAttributeType { get; }
      public abstract Type CustomRouteAttributeType { get; }
      public abstract Type DefaultActionAttributeType { get; }

      public abstract bool CanDisambiguateActionOverloads { get; }

      public virtual Type ActionOverloadDisambiguationAttributeType {
         get { return null; }
      }

      public static void RegisterProvider(CodeRoutingProvider provider) {

         if (provider == null) throw new ArgumentNullException("provider");

         lock (staticLock) {
            providers.Add(provider);
         }
      }

      public static CodeRoutingProvider GetProviderForControllerType(Type controllerType) {

         foreach (CodeRoutingProvider provider in providers) {

            if (provider.SupportsControllerType(controllerType)) {
               return provider;
            }
         }

         return null;
      }

      public static ControllerInfo AnalyzeControllerType(Type controllerType, RegisterSettings registerSettings) {

         foreach (CodeRoutingProvider provider in providers) {

            if (provider.SupportsControllerType(controllerType)) {
               return provider.CreateControllerInfo(controllerType, registerSettings);
            }
         }

         return null;
      }

      protected abstract bool SupportsControllerType(Type controllerType);
      protected abstract ControllerInfo CreateControllerInfo(Type controllerType, RegisterSettings registerSettings);
      public abstract object CreateParameterBindingRouteConstraint(ParameterBinder binder);
      public abstract object CreateRegexRouteConstraint(string pattern, Type parameterType);
      public abstract object CreateSetRouteConstraint(string[] values);

      public TAttr GetCorrectAttribute<TAttr>(ICustomAttributeProvider context, Func<CodeRoutingProvider, Type> attribute, bool inherit, Func<Type, Type, string> errorMessage) where TAttr : class {

         Type attrType = attribute(this);

         TAttr attr = context.GetCustomAttributes(attrType, inherit)
            .Cast<TAttr>()
            .SingleOrDefault();

         if (attr != null) {
            return attr;
         }

         foreach (CodeRoutingProvider otherProvider in providers.Where(p => !Object.ReferenceEquals(p, this))) {

            Type mistakenAttrType = attribute(otherProvider);

            attr = context.GetCustomAttributes(mistakenAttrType, inherit)
               .Cast<TAttr>()
               .SingleOrDefault();

            if (attr != null) {
               throw new InvalidOperationException(errorMessage(attrType, attr.GetType()));
            }
         }

         return null;
      }
   }
}
