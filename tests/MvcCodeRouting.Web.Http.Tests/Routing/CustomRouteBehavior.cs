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

      public CustomRouteBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute() {

         var controller = typeof(CustomRoute.CustomRoute1Controller);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute2() {

         var controller = typeof(CustomRoute.CustomRoute2Controller);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute3() {

         var controller = typeof(CustomRoute.CustomRoute3Controller);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute4() {

         var controller = typeof(CustomRoute.CustomRoute4Controller);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.CustomRoute {

#pragma warning disable 0618

   public class CustomRoute1Controller : ApiController {

      [MvcCodeRouting.CustomRoute("foo")]
      public void Get() { }
   }

   public class CustomRoute2Controller : ApiController {

      [MvcCodeRouting.Web.Mvc.CustomRoute("foo")]
      public void Get() { }
   }

   [MvcCodeRouting.CustomRoute("foo")]
   public class CustomRoute3Controller : ApiController {

      public void Get() { }
   }

   [MvcCodeRouting.Web.Mvc.CustomRoute("foo")]
   public class CustomRoute4Controller : ApiController {

      public void Get() { }
   }

#pragma warning restore 0618
}
