using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class OverloadedActionBehavior {

      [TestMethod]
      public void DontRequireActionDisambiguator() {

         var config = new HttpConfiguration();

         config.MapCodeRoutes(typeof(OverloadedAction.OverloadedActionController), new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.OverloadedAction {

   public class OverloadedActionController : System.Web.Http.ApiController {

      public void Foo([FromRoute]int a) { }
      public void Foo([FromRoute]int a, [FromRoute]int b) { }
   }
}