using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class CatchAllParameterBehavior {

      [TestMethod]
      public void SupportsCatchAllParameter() {

         var routes = new RouteCollection();
         routes.MapCodeRoutes(typeof(CatchAllParameter1Controller), new CodeRoutingSettings { RootOnly = true });

         Assert.IsTrue(routes.At(0).Url.EndsWith("{*a}"));
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void IsLastParameter() {

         var routes = new RouteCollection();
         routes.MapCodeRoutes(typeof(CatchAllParameter2Controller), new CodeRoutingSettings { RootOnly = true });
      }
   }

   public class CatchAllParameter1Controller : Controller {

      public void Foo([FromRoute(CatchAll = true)]string a) { }
   }

   public class CatchAllParameter2Controller : Controller {

      public void Foo([FromRoute(CatchAll = true)]string a, [FromRoute]string b) { }
   }
}
