// Copyright 2011 Max Toro Q.
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcCodeRouting.Web.Hosting;

namespace MvcCodeRouting {
   
   public static partial class CodeRoutingExtensions {

      static CodeRoutingExtensions() {
         CodeRoutingProvider.RegisterProvider(new Web.Mvc.MvcCodeRoutingProvider());
      }

      internal static void Initialize() { }

      /// <summary>
      /// Enables namespace-aware views location. Always call after you are done adding view engines.
      /// </summary>
      /// <param name="engines">The view engine collection.</param>
      public static void EnableCodeRouting(this ViewEngineCollection engines) {

         if (engines == null) throw new ArgumentNullException("engines");

         for (int i = 0; i < engines.Count; i++) {

            IViewEngine engine = engines[i];

            if (engine.GetType() == typeof(Web.Mvc.ViewEngineWrapper)) {
               continue;
            }

            engines[i] = new Web.Mvc.ViewEngineWrapper(engine);
         }

         EmbeddedViewsVirtualPathProvider.RegisterIfNecessary();
      }

      /// <summary>
      /// Sets a custom <see cref="DefaultControllerFactory"/> implementation that provides a more
      /// direct access to the controller types for routes created by MvcCodeRouting.
      /// It enables a scenario where routes are created for controllers that are dynamically loaded at runtime.
      /// </summary>
      /// <param name="controllerBuilder">The controller builder.</param>
      public static void EnableCodeRouting(this ControllerBuilder controllerBuilder) {
         controllerBuilder.SetControllerFactory(new Web.Mvc.CustomControllerFactory());
      }

      /// <summary>
      /// Binds controller properties decorated with <see cref="FromRouteAttribute"/>
      /// using the current route values.
      /// </summary>
      /// <param name="controller">The controller to bind.</param>
      /// <remarks>You can call this method from <see cref="ControllerBase.Initialize"/>.</remarks>
      [Obsolete("Please use MvcCodeRouting.Web.Mvc.MvcExtensions.BindRouteProperties(ControllerBase) instead.")]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public static void BindRouteProperties(this ControllerBase controller) {
         Web.Mvc.MvcExtensions.BindRouteProperties(controller);
      }
   }
}
