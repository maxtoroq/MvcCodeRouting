using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class OverloadedActionBehavior {

      static RouteCollection routes;
      static UrlHelper Url;

      public OverloadedActionBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void RequireActionDisambiguatorInMvc() {

         var controller = typeof(OverloadedAction.OverloadedAction1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      public void DontRequireActionDisambiguatorInWebApi() {

         var controller = typeof(OverloadedAction.OverloadedAction2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.OverloadedAction {

   public class OverloadedAction1Controller : Controller {

      public void Foo([FromRoute]int a) { }
      public void Foo([FromRoute]int a, [FromRoute]int b) { }
   }

   public class OverloadedAction2Controller : System.Web.Http.ApiController {

      public void Foo([Web.Http.FromRoute]int a) { }
      public void Foo([Web.Http.FromRoute]int a, [Web.Http.FromRoute]int b) { }
   }
}