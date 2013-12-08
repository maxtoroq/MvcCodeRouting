using System;
using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class FromRouteAttributeBehavior {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;

      public FromRouteAttributeBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;
      }

      [TestMethod]
      public void UseCustomName() {

         var controller = typeof(FromRouteAttr.UseCustomNameController);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(routes.First().RouteTemplate.Contains("{b}"));
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute() {

         var controller = typeof(FromRouteAttr.FailIfUsingWrongAttributeController);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute2() {

         var controller = typeof(FromRouteAttr.FailIfUsingWrongAttribute2Controller);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute3() {

         var controller = typeof(FromRouteAttr.FailIfUsingWrongAttribute3Controller);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void FailIfUsingWrongAttribute4() {

         var controller = typeof(FromRouteAttr.FailIfUsingWrongAttribute4Controller);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.FromRouteAttr {

   public class UseCustomNameController : ApiController {

      public string Get([FromRoute("b")]string a) {
         return a;
      }
   }

#pragma warning disable 0618

   public class FailIfUsingWrongAttributeController : ApiController {

      public void Get([MvcCodeRouting.FromRoute]string foo) { }
   }

   public class FailIfUsingWrongAttribute2Controller : ApiController {

      public void Get([MvcCodeRouting.Web.Mvc.FromRoute]string foo) { }
   }

   public class FailIfUsingWrongAttribute3Controller : ApiController {

      [MvcCodeRouting.FromRoute]
      public string Bar { get; set; }

      public void Get() { }
   }

   public class FailIfUsingWrongAttribute4Controller : ApiController {

      [MvcCodeRouting.Web.Mvc.FromRoute]
      public string Bar { get; set; }

      public void Get() { }
   }

#pragma warning restore 0618
}
