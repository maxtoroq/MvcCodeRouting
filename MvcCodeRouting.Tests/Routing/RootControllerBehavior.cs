using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class RootControllerBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public RootControllerBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void DontIncludeControllerToken() {

         var controller = typeof(RootController.RootController1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual("/Foo", Url.Action("Foo", controller));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/Foo");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.RootController {

   public class RootController1Controller : Controller {
      public void Foo() { }
   }
}