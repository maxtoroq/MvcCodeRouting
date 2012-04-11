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
   using UrlHelper = System.Web.Mvc.UrlHelper;
   
   [TestClass]
   public class RoutingFacts {

      RouteCollection routes;
      UrlHelper Url;

      [TestInitialize]
      public void Init() {
         
         this.routes = new RouteCollection();

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.ApplicationPath).Returns("");
         httpContextMock.Setup(c => c.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

         var routeData = new RouteData();
         var requestContext = new RequestContext(httpContextMock.Object, routeData);
         
         this.Url = new UrlHelper(requestContext, routes);
      }

      [TestMethod]
      public void NameIsOptional() { 
         
         routes.Clear();
         routes.MapRoute(null, "");
      }

      [TestMethod]
      public void ParametersAreRequiredByDefault() {

         routes.Clear();
         routes.MapRoute(null, "{a}");

         Assert.IsNull(Url.RouteUrl(new { }));
         Assert.AreEqual(Url.RouteUrl(new { a = "b" }), "/b");

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/b");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void DefaultValueMakesParameterOptional() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "b" });

         Assert.AreEqual(Url.RouteUrl(new { }), "/");

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void CanUseNullOrEmptyStringForParameterWithDefaultValue() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "b" });

         Assert.AreEqual(Url.RouteUrl(new { a = (string)null }), "/");
         Assert.AreEqual(Url.RouteUrl(new { a = "" }), "/");
      }

      [TestMethod]
      public void CannotUseNullOrEmptyStringForDefaultValueWithoutParameter() {

         routes.Clear();
         routes.MapRoute(null, "a", new { b = "c" });

         Assert.IsNull(Url.RouteUrl(new { b = (string)null }));
         Assert.IsNull(Url.RouteUrl(new { b = "" }));
      }

      [TestMethod]
      public void ValueMustMatchDefaultValueWithoutParameterOrBeOmitted() {

         routes.Clear();
         routes.MapRoute(null, "a", new { b = "c" });

         Assert.AreEqual(Url.RouteUrl(new { b = "c" }), "/a");
         Assert.AreEqual(Url.RouteUrl(new { }), "/a");
      }

      [TestMethod]
      public void ConstraintLimitsTheValueSpaceOfParameter() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { }, new { a = "b|c" });

         Assert.AreEqual(Url.RouteUrl(new { a = "b" }), "/b");
         Assert.AreEqual(Url.RouteUrl(new { a = "c" }), "/c");
         Assert.IsNull(Url.RouteUrl(new { a = "d" }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/b");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/c");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/d");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void ConstraintTestsTheWholeValue() {
         
         routes.Clear();
         routes.MapRoute(null, "{a}", new { }, new { a = "b" });

         Assert.AreEqual(Url.RouteUrl(new { a = "b" }), "/b");
         Assert.IsNull(Url.RouteUrl(new { a = "b2" }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/b");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/b2");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void ConstraintForOptionalParameterShouldMatchAnEmptyStringIfDefaultValueIsEmptyString() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "" }, new { a = "b" });

         Assert.IsNull(Url.RouteUrl(new { a = "" }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));

         //--------------------------------------

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "" }, new { a = "(b)?" });

         Assert.AreEqual(Url.RouteUrl(new { a = "" }), "/");

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         //--------------------------------------

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "b" }, new { a = "b" });

         Assert.AreEqual(Url.RouteUrl(new { a = "" }), "/");

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void UrlHelperActionUsesTheCurrentRequestControllerAndActionValuesAsDefaults() {

         var routeData = new RouteData {
            Values = { 
               { "controller", "b" },
               { "action", "c" }
            }
         };

         var requestContext = new RequestContext(this.Url.RequestContext.HttpContext, routeData);
         var Url = new UrlHelper(requestContext, routes);

         routes.Clear();
         routes.MapRoute(null, "a", new { controller = "b", action = "c" });

         Assert.IsNull(Url.RouteUrl(new { controller = (string)null, action = (string)null }));
         Assert.AreEqual(Url.Action(null), "/a");
         Assert.AreEqual(Url.Action(null, (string)null), "/a");
      }

      [TestMethod]
      public void CurrentRequestValuesAreUsedAsDefaults() {

         routes.Clear();
         routes.MapRoute(null, "{a}");

         var routeData = new RouteData {
            Values = { 
               { "a", "x" },
            }
         };

         var requestContext = new RequestContext(this.Url.RequestContext.HttpContext, routeData);
         var Url = new UrlHelper(requestContext, routes);

         Assert.AreEqual(Url.RouteUrl(new { }), "/x");
      }

      [TestMethod]
      public void RoutingIsCaseInsensitive() {

         routes.Clear();
         routes.MapRoute(null, "a/{b}", new { a = "A" }, new { b = "xyz" });

         Assert.IsNotNull(Url.RouteUrl(new { a = "a", b = "XYZ" }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/A/XYZ");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void LiteralSubsegmentBug() {

         // http://stackoverflow.com/questions/4318373

         routes.Clear();
         routes.MapRoute(null, "_{a}");

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/_b");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/__b");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
      }

      //[TestMethod] // Appears to be fixed in .NET 4.5
      public void TwoConsecutiveOptionalParametersBug() {

         // http://haacked.com/archive/2011/02/20/routing-regression-with-two-consecutive-optional-url-parameters.aspx

         routes.Clear();
         routes.MapRoute(null, "a/{b}/{c}", new { b = "", c = "" });

         Assert.IsNotNull(Url.RouteUrl(new { b = "1", c = "2" }));
         Assert.IsNotNull(Url.RouteUrl(new { b = "1" }));
         Assert.IsNull(Url.RouteUrl(new { }));
      }
   }
}
