using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MvcCodeRouting.Tests.ModelBinding {
   
   [TestClass]
   public class CustomNameBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public CustomNameBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void BindCustomName() {

         var controller = typeof(CustomName.BindCustomNameController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/Foo/hello");

         var httpResponseMock = new Mock<HttpResponseBase>();
         httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

         var routeData = routes.GetRouteData(httpContextMock.Object);

         var controllerInstance = (ControllerBase)Activator.CreateInstance(controller);
         controllerInstance.ValidateRequest = false;

         var requestContext = new RequestContext(httpContextMock.Object, routeData);
         var controllerContext = new ControllerContext(requestContext, controllerInstance);

         controllerInstance.ValueProvider = new ValueProviderCollection(new IValueProvider[] { new RouteDataValueProvider(controllerContext) });

         ((IController)controllerInstance).Execute(requestContext);

         httpResponseMock.Verify(c => c.Write(It.Is<string>(s => s == "hello")), Times.AtLeastOnce());
      }

      [TestMethod]
      public void BindCustomName_Property() {

         var controller = typeof(CustomName.BindCustomNamePropertyController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/hello");

         var httpResponseMock = new Mock<HttpResponseBase>();
         httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

         var routeData = routes.GetRouteData(httpContextMock.Object);

         var controllerInstance = (ControllerBase)Activator.CreateInstance(controller);
         controllerInstance.ValidateRequest = false;

         var requestContext = new RequestContext(httpContextMock.Object, routeData);
         var controllerContext = new ControllerContext(requestContext, controllerInstance);

         controllerInstance.ValueProvider = new ValueProviderCollection(new IValueProvider[] { new RouteDataValueProvider(controllerContext) });

         ((IController)controllerInstance).Execute(requestContext);

         httpResponseMock.Verify(c => c.Write(It.Is<string>(s => s == "hello")), Times.AtLeastOnce());
      }
   }
}

namespace MvcCodeRouting.Tests.ModelBinding.CustomName {
   using FromRouteAttribute = MvcCodeRouting.Web.Mvc.FromRouteAttribute;

   public class BindCustomNameController : Controller {

      public string Foo([FromRoute("b")]string a) {
         return a;
      }
   }

   public class BindCustomNamePropertyController : Controller {

      [FromRoute("b")]
      public string a { get; set; }

      protected override void Initialize(RequestContext requestContext) {
         base.Initialize(requestContext);
         MvcCodeRouting.Web.Mvc.MvcExtensions.BindRouteProperties(this);
      }

      public string Index() {
         return this.a;
      }
   }
}