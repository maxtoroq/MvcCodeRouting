using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class RouteGroupingBehavior {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {

         this.routes = new RouteCollection();
         this.Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void GroupOverloadsWithSameCustomRoute() {

         // #744
         // Create only one route for multiple actions with equal custom routes

         var controller = typeof(RouteGrouping.RouteGroupingBehavior1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(routes.Count, 1);
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void DisallowMultipleActionsWithSameCustomRoute() {

         var controller = typeof(RouteGrouping.RouteGroupingBehavior2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      public void GroupActionsWithSameCustomRouteIfActionTokenIsPresent() {

         // #779
         // Allow multiple actions with same custom route if {action} token is present

         var controller = typeof(RouteGrouping.RouteGroupingBehavior3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(routes.Count, 1);
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.RouteGrouping {

   public class RouteGroupingBehavior1Controller : Controller {

      [HttpGet]
      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id) { }

      [HttpPost]
      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id, string bar) { }
   }

   public class RouteGroupingBehavior2Controller : Controller {

      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id) { }

      [CustomRoute("{id}")]
      public void Bar([FromRoute]string id) { }
   }

   public class RouteGroupingBehavior3Controller : Controller {

      [CustomRoute("{id}/{action}")]
      public void Foo([FromRoute]string id) { }

      [CustomRoute("{id}/{action}")]
      public void Bar([FromRoute]string id) { }
   }
}