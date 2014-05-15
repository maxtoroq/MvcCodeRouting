using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace MvcCodeRouting.Tests {
   
   static class TestUtil {

      public static string GetControllerName(Type controllerType) {

         string typeName = controllerType.Name;

         return typeName.Substring(0, typeName.Length - 10);
      }

      public static void SetupHttpContextForUrlHelper(Mock<HttpContextBase> httpContextMock) {
         
         httpContextMock.Setup(c => c.Request.ApplicationPath).Returns("");
         httpContextMock.Setup(c => c.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
      }

      public static RouteCollection GetRouteCollection() {
         return new RouteCollection();
      }

      public static UrlHelper CreateUrlHelper(RouteCollection routes, string currentRouteContext = "") {

         var httpContextMock = new Mock<HttpContextBase>();
         SetupHttpContextForUrlHelper(httpContextMock);

         var routeData = new RouteData();

         if (currentRouteContext != null) {
            routeData.DataTokens["MvcCodeRouting.RouteContext"] = currentRouteContext;
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
