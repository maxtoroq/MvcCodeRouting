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
   public class FromRouteAttributeBehavior {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public FromRouteAttributeBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void UseCustomName() {

         var controller = typeof(FromRouteAttr.FromRouteAttributeController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         Assert.IsNotNull(routes.At(0).Url.Contains("{b}"));
      }

      [TestMethod]
      public void BindCustomName() {

         var controller = typeof(FromRouteAttr.FromRouteAttributeController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/Foo/hello");

         var httpResponseMock = new Mock<HttpResponseBase>();
         httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);
         
         var routeData = routes.GetRouteData(httpContextMock.Object);
         
         var controllerInstance = new FromRouteAttr.FromRouteAttributeController { 
            ValidateRequest = false
         };
         
         var controllerContext = new ControllerContext(httpContextMock.Object, routeData, controllerInstance);

         controllerInstance.ValueProvider = new ValueProviderCollection(new IValueProvider[] { new RouteDataValueProvider(controllerContext) });

         controllerInstance.ActionInvoker.InvokeAction(controllerContext, routeData.GetRequiredString("action"));

         httpResponseMock.Verify(c => c.Write(It.Is<string>(s => s == "hello")), Times.AtLeastOnce());
      }

      [TestMethod]
      public void UsesSpecifiedBinder() {

         var controller = typeof(FromRouteAttr.FromRouteAttribute2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/Foo/no");

         var httpResponseMock = new Mock<HttpResponseBase>();
         httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

         var routeData = routes.GetRouteData(httpContextMock.Object);

         var controllerInstance = new FromRouteAttr.FromRouteAttribute2Controller {
            ValidateRequest = false
         };

         var controllerContext = new ControllerContext(httpContextMock.Object, routeData, controllerInstance);
         
         controllerInstance.ValueProvider = new ValueProviderCollection(new IValueProvider[] { new RouteDataValueProvider(controllerContext) });

         controllerInstance.ActionInvoker.InvokeAction(controllerContext, routeData.GetRequiredString("action"));

         httpResponseMock.Verify(c => c.Write(It.Is<string>(s => s == "False")), Times.AtLeastOnce());
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.FromRouteAttr {

   public class FromRouteAttributeController : Controller {

      public string Foo([FromRoute("b")]string a) {
         return a;
      }
   }

   public class FromRouteAttribute2Controller : Controller {

      public bool Foo([FromRoute(Constraint = "yes|no", BinderType = typeof(YesNoModelBinder))]bool a) {
         return a;
      }
   }

   class YesNoModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         switch ((string)bindingContext.ValueProvider.GetValue(bindingContext.ModelName).RawValue) {
            case "yes":
               return true;
            case "no":
               return false;
            
            default:
               throw new InvalidOperationException();
         }
      }
   }
}
