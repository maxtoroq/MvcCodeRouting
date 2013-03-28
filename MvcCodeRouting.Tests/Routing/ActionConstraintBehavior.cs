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
   public class ActionConstraintBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public ActionConstraintBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void OnlyMatchesExistingActions() {

         var controller = typeof(ActionConstraint.ActionConstraint1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var httpContextMock = new Mock<HttpContextBase>();

         foreach (var item in new[] { "Foo", "Bar" }) {

            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/" + item);

            Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
            Assert.AreEqual("/" + item, Url.Action(item, controller));
         }

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/a");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
         Assert.AreEqual("/a", Url.Action("Custom", controller));

         foreach (var item in new[] { "Custom2", "Custom3" }) {
            
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/b/" + item);

            Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
            Assert.AreEqual("/b/" + item, Url.Action(item, controller));
         }

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/b/XYZ");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
         Assert.IsNull(Url.Action("XYZ", controller));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/Bar2");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
         Assert.IsNull(Url.Action("Bar2", controller));
      }

      [TestMethod]
      public void NotNeededForSingleNonDefaultAction() {

         var controller = typeof(ActionConstraint.ActionConstraint2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNull(routes.At(0).Constraints["action"]);
      }

      [TestMethod]
      public void NotNeededForCustomRouteWithoutActionToken() {

         var controller = typeof(ActionConstraint.ActionConstraint3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNull(routes.At(0).Constraints["action"]);
         Assert.IsNull(routes.At(1).Constraints["action"]);
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.ActionConstraint {
   using CustomRouteAttribute = MvcCodeRouting.Web.Mvc.CustomRouteAttribute;

   public class ActionConstraint1Controller : Controller {
      public void Foo() { }
      public void Bar() { }

      [CustomRoute("a")]
      public void Custom() { }

      [CustomRoute("b/{action}")]
      public void Custom2() { }

      [CustomRoute("b/{action}")]
      public void Custom3() { }
   }

   public class ActionConstraint2Controller : Controller {
      public void Foo() { }
   }

   public class ActionConstraint3Controller : Controller {

      [CustomRoute("a")]
      public void Index() { }

      [CustomRoute("b")]
      public void Custom() { }
   }
}
