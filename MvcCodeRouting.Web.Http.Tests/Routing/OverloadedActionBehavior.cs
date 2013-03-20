using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class OverloadedActionBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public OverloadedActionBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void DontRequireActionDisambiguator() {

         var controller = typeof(OverloadedAction.OverloadedActionController);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.OverloadedAction {

   public class OverloadedActionController : System.Web.Http.ApiController {

      public void Foo([Web.Http.FromRoute]int a) { }
      public void Foo([Web.Http.FromRoute]int a, [Web.Http.FromRoute]int b) { }
   }
}