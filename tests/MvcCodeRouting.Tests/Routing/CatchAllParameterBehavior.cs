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
   public class CatchAllParameterBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public CatchAllParameterBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void SupportsCatchAllParameter() {

         var controller = typeof(CatchAll.CatchAllParameter1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/Foo/1/2/3");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
         Assert.IsNotNull(Url.Action("Foo", controller, new { a = "1/2/3" }));
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void IsLastParameter() {

         routes.Clear();
         routes.MapCodeRoutes(typeof(CatchAll.CatchAllParameter2Controller), new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.CatchAll {
   using FromRouteAttribute = MvcCodeRouting.Web.Mvc.FromRouteAttribute;

   public class CatchAllParameter1Controller : Controller {
      public void Foo([FromRoute(CatchAll = true)]string a) { }
   }

   public class CatchAllParameter2Controller : Controller {
      public void Foo([FromRoute(CatchAll = true)]string a, [FromRoute]string b) { }
   }
}