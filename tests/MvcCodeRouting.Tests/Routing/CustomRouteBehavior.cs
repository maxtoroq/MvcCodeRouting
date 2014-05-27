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
   public class CustomRouteBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public CustomRouteBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void GroupOverloadsWithSameCustomRoute() {

         // #744
         // Create only one route for multiple actions with equal custom routes

         var controller = typeof(CustomRoute.CustomRoute1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(1, routes.Count);
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void DisallowMultipleActionsWithSameCustomRoute() {

         var controller = typeof(CustomRoute.CustomRoute2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      public void GroupActionsWithSameCustomRouteIfActionTokenIsPresent() {

         // #779
         // Allow multiple actions with same custom route if the {action} parameter is present

         var controller = typeof(CustomRoute.CustomRoute3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(1, routes.Count);
      }

      [TestMethod]
      public void AbsoluteCustomRouteOnAction() {

         var controller = typeof(CustomRoute.CustomRoute4.CustomRoute4Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         Assert.AreEqual("bar", routes.At(0).Url);
      }

      [TestMethod]
      public void AbsoluteCustomRouteOnController() {

         var controller = typeof(CustomRoute.CustomRoute5.CustomRoute5Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/bar");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/bar/foo");
         
         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.CustomRoute {
   using FromRouteAttribute = MvcCodeRouting.Web.Mvc.FromRouteAttribute;
   using CustomRouteAttribute = MvcCodeRouting.Web.Mvc.CustomRouteAttribute;

   public class CustomRoute1Controller : Controller {

      [HttpGet]
      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id) { }

      [HttpPost]
      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id, string bar) { }
   }

   public class CustomRoute2Controller : Controller {

      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id) { }

      [CustomRoute("{id}")]
      public void Bar([FromRoute]string id) { }
   }

   public class CustomRoute3Controller : Controller {

      [CustomRoute("{id}/{action}")]
      public void Foo([FromRoute]string id) { }

      [CustomRoute("{id}/{action}")]
      public void Bar([FromRoute]string id) { }
   }
}

namespace MvcCodeRouting.Tests.Routing.CustomRoute.CustomRoute4 {

   public class CustomRoute4Controller : Controller { }
}

namespace MvcCodeRouting.Tests.Routing.CustomRoute.CustomRoute4.SubNamespace {
   using CustomRouteAttribute = MvcCodeRouting.Web.Mvc.CustomRouteAttribute;

   public class SubNamespace1Controller : Controller {

      [CustomRoute("~/bar")]
      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.CustomRoute.CustomRoute5 {

   public class CustomRoute5Controller : Controller { }
}

namespace MvcCodeRouting.Tests.Routing.CustomRoute.CustomRoute5.SubNamespace {
   using CustomRouteAttribute = MvcCodeRouting.Web.Mvc.CustomRouteAttribute;

   [CustomRoute("~/bar")]
   public class SubNamespace1Controller : Controller {

      public void Index() { }
      public void Foo() { }
   }
}