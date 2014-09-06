using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace MvcCodeRouting.Web.Mvc.Tests {
   
   static class TestUtil {

      public static string GetControllerName(Type controllerType) {

         string typeName = controllerType.Name;

         return typeName.Substring(0, typeName.Length - 10);
      }

      public static RouteCollection GetRouteCollection() {
         return new RouteCollection();
      }

      public static UrlHelper CreateUrlHelper(RouteCollection routes, string currentAppRelativePath = "~/", bool createRouteData = false) {

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.ApplicationPath).Returns("");
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns(currentAppRelativePath);
         httpContextMock.Setup(c => c.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

         RouteData routeData = routes.GetRouteData(httpContextMock.Object);

         if (routeData == null) {
            if (createRouteData) {
               routeData = new RouteData { DataTokens = { { "MvcCodeRouting.RouteContext", "" } } };
            } else {
               throw new InvalidOperationException();
            }
         }

         var requestContext = new RequestContext(httpContextMock.Object, routeData);
         var urlHelper = new UrlHelper(requestContext, routes);

         return urlHelper;
      }

      public static Route At(this RouteCollection routes, int index) {
         return routes.Cast<Route>().Skip(index).First();
      }
   }

   class UrlHelper : System.Web.Mvc.UrlHelper {

      public UrlHelper(RequestContext requestContext, RouteCollection routeCollection) 
         : base(requestContext, routeCollection) { }

      public string Action(string action, Type controller) {
         return base.Action(action, TestUtil.GetControllerName(controller));
      }

      public string Action(string action, Type controller, object routeValues) {
         return base.Action(action, TestUtil.GetControllerName(controller), routeValues);
      }
   }
}
