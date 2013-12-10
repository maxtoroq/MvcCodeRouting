using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class ActionNameBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public ActionNameBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void UseActionAlias() {

         var controller = typeof(ActionName.ActionName1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.Action("Bar", controller));

         controller = typeof(ActionName.ActionName3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.Action("Bar", controller));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.ActionName {

   public class ActionName1Controller : Controller {

      [ActionName("Bar")]
      public void Foo() { }
   }

   public class ActionName3Controller : AsyncController {

      [ActionName("Bar")]
      public void FooAsync() { }

      public void FooCompleted() { }
   }
}