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
   public class RouteFormattingBehavior {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {

         this.routes = new RouteCollection();
         this.Url = TestUtil.CreateUrlHelper(routes);
      }

      [TestMethod]
      public void IncomingRequestAction() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args => {
               if (args.SegmentType == RouteSegmentType.Action)
                  return "_" + args.OriginalSegment;
               return args.OriginalSegment;
            }
         });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/_AbcDfg");

         RouteData routeData = routes.GetRouteData(httpContextMock.Object);

         Assert.IsNotNull(routeData);
         Assert.AreEqual(routeData.GetRequiredString("action"), "AbcDfg");

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/_Index");

         routeData = routes.GetRouteData(httpContextMock.Object);

         Assert.IsNotNull(routeData);
         Assert.AreEqual(routeData.GetRequiredString("action"), "Index");
      }

      [TestMethod]
      public void IncomingRequestController() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args => {
               if (args.SegmentType == RouteSegmentType.Controller)
                  return "_" + args.OriginalSegment;
               return args.OriginalSegment;
            }
         });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/_RouteFormatting2");

         RouteData routeData = routes.GetRouteData(httpContextMock.Object);

         Assert.AreEqual(routeData.GetRequiredString("controller"), "RouteFormatting2");
      }

      [TestMethod]
      public void IncomingRequestNamespace() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args => {
               if (args.SegmentType == RouteSegmentType.Namespace)
                  return "_" + args.OriginalSegment;
               return args.OriginalSegment;
            }
         });

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/_SubNamespace/RouteFormatting3");

         RouteData routeData = routes.GetRouteData(httpContextMock.Object);

         Assert.AreEqual(routeData.GetRequiredString("controller"), "RouteFormatting3");
         Assert.AreEqual(((string[])routeData.DataTokens["Namespaces"])[0], typeof(RouteFormatting.SubNamespace.RouteFormatting3Controller).Namespace);
      }

      [TestMethod]
      public void UrlGenerationAction() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args => {
               if (args.SegmentType == RouteSegmentType.Action)
                  return "_" + args.OriginalSegment;
               return args.OriginalSegment;
            }
         });

         Assert.AreEqual(Url.Action("AbcDfg", controller), "/_AbcDfg");
         Assert.AreEqual(Url.Action("Index", controller), "/");
      }

      [TestMethod]
      public void UrlGenerationController() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args => {
               if (args.SegmentType == RouteSegmentType.Controller)
                  return "_" + args.OriginalSegment;
               return args.OriginalSegment;
            }
         });

         Assert.AreEqual(Url.Action("", "RouteFormatting2"), "/_RouteFormatting2");
      }

      [TestMethod]
      public void UrlGenerationNamespace() {

         var controller = typeof(RouteFormatting.RouteFormatting1Controller);

         routes.Clear();
         routes.MapCodeRoutes(controller, new CodeRoutingSettings {
            RouteFormatter = args => {
               if (args.SegmentType == RouteSegmentType.Namespace)
                  return "_" + args.OriginalSegment;
               return args.OriginalSegment;
            }
         });

         Assert.IsTrue(Url.Action("", "SubNamespace.RouteFormatting3").StartsWith("/_SubNamespace/"));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.RouteFormatting {

   public class RouteFormatting1Controller : Controller {
      public void Index() { }
      public void AbcDfg() { }
   }

   public class RouteFormatting2Controller : Controller {
      public void Index() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.RouteFormatting.SubNamespace {

   public class RouteFormatting3Controller : Controller {
      public void Index() { }
   }
}