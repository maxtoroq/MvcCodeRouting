using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class OverloadedActionBehavior {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;
      readonly CodeRoutingSettings settings;

      public OverloadedActionBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;

         settings = new CodeRoutingSettings();
         settings.HttpConfiguration(config);
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

      public void Foo([FromRoute]int a) { }
      public void Foo([FromRoute]int a, [FromRoute]int b) { }
   }
}