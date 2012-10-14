using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class CustomRouteGroupingBehavior {

      static RouteCollection routes;
      static UrlHelper Url;

      public CustomRouteGroupingBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void GroupOverloadsWithSameCustomRoute() {

         // #744
         // Create only one route for multiple actions with equal custom routes

         var controller = typeof(CustomRouteGrouping.CustomRouteGrouping1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(1, routes.Count);
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void DisallowMultipleActionsWithSameCustomRoute() {

         var controller = typeof(CustomRouteGrouping.CustomRouteGrouping2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      public void GroupActionsWithSameCustomRouteIfActionTokenIsPresent() {

         // #779
         // Allow multiple actions with same custom route if {action} token is present

         var controller = typeof(CustomRouteGrouping.CustomRouteGrouping3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(1, routes.Count);
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.CustomRouteGrouping {

   public class CustomRouteGrouping1Controller : Controller {

      [HttpGet]
      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id) { }

      [HttpPost]
      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id, string bar) { }
   }

   public class CustomRouteGrouping2Controller : Controller {

      [CustomRoute("{id}")]
      public void Foo([FromRoute]string id) { }

      [CustomRoute("{id}")]
      public void Bar([FromRoute]string id) { }
   }

   public class CustomRouteGrouping3Controller : Controller {

      [CustomRoute("{id}/{action}")]
      public void Foo([FromRoute]string id) { }

      [CustomRoute("{id}/{action}")]
      public void Bar([FromRoute]string id) { }
   }
}