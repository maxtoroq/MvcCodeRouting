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
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Http {
   
   abstract class HttpControllerInfo : ControllerInfo {

      public static ControllerInfo Create(Type controllerType, RegisterSettings registerSettings, CodeRoutingProvider provider) {

         HttpConfiguration configuration = registerSettings.HttpConfiguration as HttpConfiguration;

         if (configuration == null) {
            
            if (registerSettings.UseGlobalHttpConfiguration) {
               configuration = GlobalConfiguration.Configuration;
            } else {
               object httpConfig;

               if (registerSettings.Settings.Properties.TryGetValue("HttpConfiguration", out httpConfig))
                  configuration = httpConfig as HttpConfiguration;
            }

            if (configuration == null) {
               throw new ArgumentException(
                  "You must first specify an {0} instance using the HttpConfiguration key on {1}.Properties.".FormatInvariant(
                     typeof(HttpConfiguration).FullName,
                     typeof(CodeRoutingSettings).FullName
                  )
               );
            }

            registerSettings.HttpConfiguration = configuration;
         }
         
         return new DescribedHttpControllerInfo(
            new HttpControllerDescriptor(
               configuration,
               controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length),
               controllerType),
            controllerType,
            registerSettings,
            provider
         );
      }

      public HttpControllerInfo(Type type, RegisterSettings registerSettings, CodeRoutingProvider provider)
         : base(type, registerSettings, provider) { }

      protected override bool IsNonAction(ICustomAttributeProvider action) {
         return action.IsDefined(typeof(NonActionAttribute), inherit: true);
      }
   }
}
