using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class OverloadedActionBehavior {

      readonly HttpConfiguration config = new HttpConfiguration();

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void MustHaveRouteParametersThatAreEqualInNameAndPosition() {
         config.MapCodeRoutes(typeof(OverloadedAction.OverloadedAction1Controller), new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void MustHaveRouteParametersThatAreEqualInConstraint() {
         config.MapCodeRoutes(typeof(OverloadedAction.OverloadedAction2Controller), new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod]
      public void DontRequireActionDisambiguator() {
         config.MapCodeRoutes(typeof(OverloadedAction.Overloaded3ActionController), new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.OverloadedAction {

   public class OverloadedAction1Controller : ApiController {

      public void Foo([FromRoute]string a) { }
      public void Foo([FromRoute]string b, [FromRoute]string a) { }
   }

   public class OverloadedAction2Controller : ApiController {

      public void Foo([FromRoute(Constraint = "a")]string a) { }
      public void Foo([FromRoute(Constraint = "x")]string a, [FromRoute]string b) { }
   }

   public class Overloaded3ActionController : ApiController {

      public void Foo([FromRoute]int a) { }
      public void Foo([FromRoute]int a, [FromRoute]int b) { }
   }
}