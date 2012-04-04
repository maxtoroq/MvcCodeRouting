using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class RouteFormattingBehavior {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {

         this.routes = new RouteCollection();
         this.Url = TestUtil.CreateUrlHelper(routes);
      }

      //[TestMethod]
      public void IncomingRequestWorks() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();

         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args =>
               "_" + args.OriginalSegment
         });
         
         // TODO
         //routes.GetRouteData(
      }

      [TestMethod]
      public void UrlGenerationWorks() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();

         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args => 
               "_" + args.OriginalSegment
         });

         Assert.AreEqual(Url.Action("AbcDfg", controller), "/_AbcDfg");
         Assert.AreEqual(Url.Action("", "SubNamespace.RouteFormatting2"), "/_SubNamespace/_RouteFormatting2");
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.RouteFormatting {

   public class RouteFormatting1Controller : Controller {

      public void Index() { }
      public void AbcDfg() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.RouteFormatting.SubNamespace {

   public class RouteFormatting2Controller : Controller {
      public void Index() { }
   }
}