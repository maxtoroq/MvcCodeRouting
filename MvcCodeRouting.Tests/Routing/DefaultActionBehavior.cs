using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCodeRouting;

namespace MvcCodeRouting.Tests.Routing {

   [TestClass]
   public class DefaultActionBehavior {

      [TestMethod]
      public void CanUseEmptyStringInUrlGeneration() {

         // #32
         // Using an empty string as action for URL generation (e.g. Url.Action("")) does not work

         var routes = new RouteCollection();
         routes.MapCodeRoutes(typeof(DefaultAction1Controller), new CodeRoutingSettings { RootOnly = true });

         Assert.IsTrue(((Route)routes[0]).Url.Contains("{action}"));

         routes.Clear();
         routes.MapCodeRoutes(typeof(DefaultAction2Controller), new CodeRoutingSettings { RootOnly = true });

         Assert.IsTrue(((Route)routes[0]).Url.Contains("{action}"));
      }

      [TestMethod]
      public void CanHaveOptionalRouteParameters() {

         // #783
         // Default action with optional route parameters does not work

         var routes = new RouteCollection();
         routes.MapCodeRoutes(typeof(DefaultAction3Controller), new CodeRoutingSettings { RootOnly = true });

         Assert.IsTrue(((Route)routes[0]).Url.Contains("{action}"));
      }

      [TestMethod]
      public void CanBeOverloaded() {

         // #535
         // Overloaded default action should not produced a route with hardcoded action

         var routes = new RouteCollection();
         routes.MapCodeRoutes(typeof(DefaultAction4Controller), new CodeRoutingSettings { RootOnly = true });

         Assert.IsTrue(((Route)routes[0]).Url.Contains("{action}"));
      }
   }

   public class DefaultAction1Controller : Controller {

      public void Index() { }
   }

   public class DefaultAction2Controller : Controller {

      public void Index() { }
      public void Foo() { }
   }

   public class DefaultAction3Controller : Controller {

      public void Index([FromRoute]int? id) { }
   }

   public class DefaultAction4Controller : Controller {

      public void Index() { }

      [HttpPost]
      public void Index(string foo) { }
   }
}
