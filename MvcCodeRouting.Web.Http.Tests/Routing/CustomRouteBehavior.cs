using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class CustomRouteBehavior {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;
      readonly CodeRoutingSettings settings;

      public CustomRouteBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;

         settings = new CodeRoutingSettings();
         settings.HttpConfiguration(config);
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute() {

         var controller = typeof(CustomRoute.CustomRoute1Controller);

         routes.MapCodeRoutes(controller, new CodeRoutingSettings(settings) { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.CustomRoute {

   public class CustomRoute1Controller : ApiController {

      [MvcCodeRouting.CustomRoute("foo")]
      public void Get() { }
   }
}
