using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class ActionNameBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public ActionNameBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void EmptyUnlessUsingVerbAttributeIn() {

         var controller = typeof(ActionName.ActionName4Controller);

         routes.Clear();
         routes.MapCodeRoutes(typeof(ActionName.ActionName4Controller), new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.HttpRouteUrl(null, controller));
         Assert.IsNotNull(Url.HttpRouteUrl(null, controller, new { action = "Foo" }));
      }

      [TestMethod]
      public void UseActionAlias() {

         var controller = typeof(ActionName.ActionName2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.HttpRouteUrl(null, controller, new { action = "Bar" }));
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.ActionName {

   public class ActionName2Controller : System.Web.Http.ApiController {

      [System.Web.Http.HttpGet]
      [System.Web.Http.ActionName("Bar")]
      public void Foo() { }
   }

   public class ActionName4Controller : System.Web.Http.ApiController {

      public void Get() { }

      [System.Web.Http.HttpGet]
      public void Foo() { }
   }
}