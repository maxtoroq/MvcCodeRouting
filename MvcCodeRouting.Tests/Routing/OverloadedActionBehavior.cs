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

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public OverloadedActionBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void RequireActionDisambiguatorInMvc() {

         var controller = typeof(OverloadedAction.OverloadedActionController);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.OverloadedAction {
   using FromRouteAttribute = MvcCodeRouting.Web.Mvc.FromRouteAttribute;

   public class OverloadedActionController : Controller {

      public void Foo([FromRoute]int a) { }
      public void Foo([FromRoute]int a, [FromRoute]int b) { }
   }
}