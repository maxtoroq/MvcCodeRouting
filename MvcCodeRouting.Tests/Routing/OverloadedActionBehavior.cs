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
      public void MustHaveRouteParametersThatAreEqualInNameAndPosition() {
         routes.MapCodeRoutes(typeof(OverloadedAction.OverloadedAction1Controller), new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void MustHaveRouteParametersThatAreEqualInConstraint() {
         routes.MapCodeRoutes(typeof(OverloadedAction.OverloadedAction2Controller), new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void RequireActionDisambiguatorInMvc() {
         routes.MapCodeRoutes(typeof(OverloadedAction.OverloadedAction3Controller), new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.OverloadedAction {
   using FromRouteAttribute = MvcCodeRouting.Web.Mvc.FromRouteAttribute;

   public class OverloadedAction1Controller : Controller {

      public void Foo([FromRoute]string a) { }
      public void Foo([FromRoute]string b, [FromRoute]string a) { }
   }

   public class OverloadedAction2Controller : Controller {

      public void Foo([FromRoute(Constraint = "a")]string a) { }
      public void Foo([FromRoute(Constraint = "x")]string a, [FromRoute]string b) { }
   }

   public class OverloadedAction3Controller : Controller {

      public void Foo([FromRoute]int a) { }
      public void Foo([FromRoute]int a, [FromRoute]int b) { }
   }
}