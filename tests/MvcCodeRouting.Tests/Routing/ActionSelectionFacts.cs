using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MvcCodeRouting.Tests.Routing {
   
   //[TestClass]
   public class ActionSelectionFacts {

      [TestMethod]
      public void ActionSelectionIsOrdinalCaseInsensitive() {

         var routes = new RouteCollection();
         
         routes.MapRoute(null, "{action}",
            new { controller = "ActionSelection" });

         foreach (string action in new[] { "Strasse", "Straße" }) {

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/" + action.ToUpperInvariant());

            var httpResponseMock = new Mock<HttpResponseBase>();
            httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

            var routeData = routes.GetRouteData(httpContextMock.Object);

            var controllerInstance = (Controller)Activator.CreateInstance(typeof(ActionSelection.ActionSelectionController));
            controllerInstance.ValidateRequest = false;

            var requestContext = new RequestContext(httpContextMock.Object, routeData);
            var controllerContext = new ControllerContext(requestContext, controllerInstance);

            ((IController)controllerInstance).Execute(requestContext);

            httpResponseMock.Verify(c => c.Write(It.Is<string>(s => String.Equals(s, action, StringComparison.OrdinalIgnoreCase))), Times.AtLeastOnce()); 
         }
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.ActionSelection {

   public class ActionSelectionController : Controller {

      public string Strasse() {
         return "Strasse";
      }

      public string Straße() {
         return "Straße";
      }
   }
}