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

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public RouteFormattingBehavior() {

         routes = TestUtil.GetRouteCollection();
         Url = TestUtil.CreateUrlHelper(routes);
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
         Assert.AreEqual("AbcDfg", routeData.GetRequiredString("action"));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/_Index");

         routeData = routes.GetRouteData(httpContextMock.Object);

         Assert.IsNotNull(routeData);
         Assert.AreEqual("Index", routeData.GetRequiredString("action"));
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

         Assert.AreEqual("RouteFormatting2", routeData.GetRequiredString("controller"));
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

         Assert.AreEqual("RouteFormatting3", routeData.GetRequiredString("controller"));
         Assert.AreEqual(typeof(RouteFormatting.SubNamespace.RouteFormatting3Controller).Namespace, ((string[])routeData.DataTokens["Namespaces"])[0]);
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

         Assert.AreEqual("/_AbcDfg", Url.Action("AbcDfg", controller));
         Assert.AreEqual("/", Url.Action("Index", controller));
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

         Assert.AreEqual("/_RouteFormatting2", Url.Action("", "RouteFormatting2"));
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