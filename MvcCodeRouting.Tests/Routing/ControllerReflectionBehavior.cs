using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {

   [TestClass]
   public class ControllerReflectionBehavior {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {

         this.routes = TestUtil.GetRouteCollection();
         this.Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void CreateRoutesForCallableActionMethodsOnly() {

         var controller = typeof(ControllerReflection.ControllerReflection1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(0, routes.Count);

         controller = typeof(ControllerReflection.ControllerReflection2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.AreEqual(0, routes.Count);
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReflection {

   public class ControllerReflection1Controller : Controller {

      public static void StaticMethod() { }
      
      public void TypeParameter<T>() { }
      
      public void OutParameter(out string s) {
         throw new InvalidOperationException();
      }

      public void RefParameter(ref string s) { }

      [NonAction]
      public void NonAction() { }
   }

   public class ControllerReflection2Controller : System.Web.Http.ApiController {

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