using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class AliasedActionBehavior {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {

         this.routes = new RouteCollection();
         this.Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void UseActionAlias() {

         var controller = typeof(AliasedAction.AliasedAction1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.Action("Bar", controller));

         controller = typeof(AliasedAction.AliasedAction2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.Action("Bar", controller));

         controller = typeof(AliasedAction.AliasedAction3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.Action("Bar", controller));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.AliasedAction {

   public class AliasedAction1Controller : Controller {

      [ActionName("Bar")]
      public void Foo() { }
   }

   public class AliasedAction2Controller : System.Web.Http.ApiController {

      [System.Web.Http.ActionName("Bar")]
      public void Foo() { }
   }

   public class AliasedAction3Controller : AsyncController {

      [ActionName("Bar")]
      public void FooAsync() { }

      public void FooCompleted() { }
   }
}