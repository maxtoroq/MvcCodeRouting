using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {

   [TestClass]
   public class ControllerReflectionBehavior {

      static RouteCollection routes;
      static UrlHelper Url;

      public ControllerReflectionBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void CreateRoutesForCallableActionMethodsOnly() {

         var controller = typeof(ControllerReflection.ControllerReflection1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(0, routes.Count);
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.ControllerReflection {

   public class ControllerReflection1Controller : System.Web.Http.ApiController {

      public static void StaticMethod() { }

      public void TypeParameter<T>() { }

      public void OutParameter(out string s) {
         throw new InvalidOperationException();
      }

      public void RefParameter(ref string s) { }

      [System.Web.Http.NonAction]
      public void NonAction() { }
   }
}