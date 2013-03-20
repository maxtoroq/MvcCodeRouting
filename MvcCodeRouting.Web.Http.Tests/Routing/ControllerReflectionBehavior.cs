using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {

   [TestClass]
   public class ControllerReflectionBehavior {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;
      readonly CodeRoutingSettings settings;

      public ControllerReflectionBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;

         settings = new CodeRoutingSettings();
         settings.HttpConfiguration(config);
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

   public class ControllerReflection1Controller : ApiController {

      public static void StaticMethod() { }

      public void TypeParameter<T>() { }

      public void OutParameter(out string s) {
         throw new InvalidOperationException();
      }

      public void RefParameter(ref string s) { }

      [NonAction]
      public void NonAction() { }
   }
}