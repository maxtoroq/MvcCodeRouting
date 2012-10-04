﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class ActionNameBehavior {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {

         this.routes = TestUtil.GetRouteCollection();
         this.Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void EmptyUnlessUsingVerbAttributeInWebApi() {

         var controller = typeof(ActionName.ActionName4Controller);

         routes.Clear();
         routes.MapCodeRoutes(typeof(ActionName.ActionName4Controller), new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.HttpRouteUrl(null, controller));
         Assert.IsNotNull(Url.HttpRouteUrl(null, controller, new { action = "Foo" }));
      }

      [TestMethod]
      public void UseActionAliasMvc() {

         var controller = typeof(ActionName.ActionName1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.Action("Bar", controller));

         controller = typeof(ActionName.ActionName3Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.Action("Bar", controller));
      }

      [TestMethod]
      public void UseActionAliasWebApi() {

         var controller = typeof(ActionName.ActionName2Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         Assert.IsNotNull(Url.HttpRouteUrl(null, controller, new { action = "Bar" }));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.ActionName {

   public class ActionName1Controller : Controller {

      [ActionName("Bar")]
      public void Foo() { }
   }

   public class ActionName2Controller : System.Web.Http.ApiController {

      [System.Web.Http.HttpGet]
      [System.Web.Http.ActionName("Bar")]
      public void Foo() { }
   }

   public class ActionName3Controller : AsyncController {

      [ActionName("Bar")]
      public void FooAsync() { }

      public void FooCompleted() { }
   }

   public class ActionName4Controller : System.Web.Http.ApiController {

      public void Get() { }

      [System.Web.Http.HttpGet]
      public void Foo() { }
   }
}