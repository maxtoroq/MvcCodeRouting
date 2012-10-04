using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MvcCodeRouting.Tests.Routing {

   [TestClass]
   public class DefaultActionBehavior {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {

         this.routes = TestUtil.GetRouteCollection();
         this.Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void IsNamedIndex() {

         var controller = typeof(DefaultAction.DefaultAction2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.AreEqual("Index", routes.GetRouteData(httpContextMock.Object).GetRequiredString("action"));
         Assert.AreEqual("/", Url.Action("", controller));

         controller = typeof(DefaultAction.DefaultAction5Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
         Assert.IsNull(Url.Action("", controller));
      }

      [TestMethod]
      public void CanUseEmptyStringInUrlGeneration() {

         // #32
         // Using an empty string as action for URL generation (e.g. Url.Action("")) does not work

         var controller = typeof(DefaultAction.DefaultAction1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual("/", Url.Action("", controller));

         controller = typeof(DefaultAction.DefaultAction2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual("/", Url.Action("", controller));
      }

      [TestMethod]
      public void CanHaveOptionalRouteParameters() {

         // #783
         // Default action with optional route parameters does not work

         var controller = typeof(DefaultAction.DefaultAction3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual("/", Url.Action("", controller));
         Assert.AreEqual("/Index/5", Url.Action("", controller, new { id = 5 }));
      }

      [TestMethod]
      public void CanBeOverloaded() {

         // #535
         // Overloaded default action should not produced a route with hardcoded action

         var controller = typeof(DefaultAction.DefaultAction4Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual("/", Url.Action("", controller));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.DefaultAction {

   public class DefaultAction1Controller : Controller {
      public void Index() { }
   }

   public class DefaultAction2Controller : Controller {
      public void Index() { }
      public void Foo() { }
   }

   public class DefaultAction3Controller : Controller {
      public void Index([FromRoute]int? id) { }
   }

   public class DefaultAction4Controller : Controller {

      public void Index() { }

      [HttpPost]
      public void Index(string foo) { }
   }

   public class DefaultAction5Controller : Controller {

      [CustomRoute("{action}")]
      public void Foo() { }
   }
}