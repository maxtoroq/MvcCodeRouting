using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcCodeRouting.Web.Mvc;

namespace MvcCodeRouting.Tests.Routing {

   [TestClass]
   public class DefaultActionBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public DefaultActionBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void IsNamedIndex() {

         var controller = typeof(DefaultAction.DefaultAction2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.AreEqual("Index", routes.GetRouteData(httpContextMock.Object).GetRequiredString("action"));
         Assert.AreEqual("/", Url.Action("Index", controller));

         controller = typeof(DefaultAction.DefaultAction5Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
         Assert.IsNull(Url.Action("", controller));
      }

      //[TestMethod] // breaking change
      public void IsNamedAsControllerIfNoIndex() {

         var controller = typeof(DefaultAction.DefaultAction11Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.AreEqual("DefaultAction11", routes.GetRouteData(httpContextMock.Object).GetRequiredString("action"));
         Assert.AreEqual("/", Url.Action("DefaultAction11", controller));
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

      [TestMethod]
      public void CanUseCustomDefault() {

         var controller = typeof(DefaultAction.DefaultAction6Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.AreEqual("Foo", routes.GetRouteData(httpContextMock.Object).GetRequiredString("action"));
         Assert.AreEqual("/", Url.Action("Foo", controller));
      }

      [TestMethod]
      public void CanInheritCustomDefault() {

         var controller = typeof(DefaultAction.DefaultAction7Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.AreEqual("Foo", routes.GetRouteData(httpContextMock.Object).GetRequiredString("action"));
         Assert.AreEqual("/", Url.Action("Foo", controller));
      }

      [TestMethod]
      public void CanOverrideInheritedDefault() {

         var controller = typeof(DefaultAction.DefaultAction8Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.AreEqual("Bar", routes.GetRouteData(httpContextMock.Object).GetRequiredString("action"));
         Assert.AreEqual("/", Url.Action("Bar", controller));
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void CustomDefaultCannotHaveRequiredRouteParameters() {

         var controller = typeof(DefaultAction.DefaultAction9Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void CustomMustBeSpecifiedOncePerDeclaringType() {

         var controller = typeof(DefaultAction.DefaultAction10Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.DefaultAction {
   using FromRouteAttribute = MvcCodeRouting.Web.Mvc.FromRouteAttribute;
   using CustomRouteAttribute = MvcCodeRouting.Web.Mvc.CustomRouteAttribute;

   public class DefaultAction1Controller : Controller {
      public void Index() { }
   }

   public class DefaultAction2Controller : Controller {
      public void Index() { }
      public void Foo() { }
      public void DefaultAction2() { }
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

   public class DefaultAction6Controller : Controller {

      public void Index() { }
      public void DefaultAction6() { }

      [DefaultAction]
      public void Foo() { }
   }

   public abstract class DefaultAction7BaseController : Controller {

      public void Index() { }

      [DefaultAction]
      public void Foo() { }
   }

   public class DefaultAction7Controller : DefaultAction7BaseController {

      public void Bar() { }
   }

   public abstract class DefaultAction8BaseController : Controller {

      public void Index() { }

      [DefaultAction]
      public void Foo() { }
   }

   public class DefaultAction8Controller : DefaultAction8BaseController {

      [DefaultAction]
      public void Bar() { }
   }

   public class DefaultAction9Controller : Controller {

      [DefaultAction]
      public void Bar([FromRoute]string a) { }
   }

   public class DefaultAction10Controller : Controller {

      [DefaultAction]
      public void Foo() { }

      [DefaultAction]
      public void Bar() { }
   }

   public class DefaultAction11Controller : Controller {

      public void DefaultAction11() { }
   }
}