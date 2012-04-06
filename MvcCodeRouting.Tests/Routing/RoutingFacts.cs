using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;

namespace MvcCodeRouting.Tests.Routing {
   
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
      public void TokensAreRequiredByDefault() {

         routes.Clear();
         routes.MapRoute(null, "{a}");

         Assert.IsNull(Url.RouteUrl(new { }));
         Assert.AreEqual(Url.RouteUrl(new { a = "foo" }), "/foo");

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/foo");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void DefaultValueMakesTokenOptional() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "foo" });

         Assert.AreEqual(Url.RouteUrl(new { }), "/");

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void CanUseNullOrEmptyStringForTokenWithDefaultValue() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "foo" });

         Assert.AreEqual(Url.RouteUrl(new { a = (string)null }), "/");
         Assert.AreEqual(Url.RouteUrl(new { a = "" }), "/");
      }

      [TestMethod]
      public void CannotUseNullOrEmptyStringForDefaultValueWithoutToken() {

         routes.Clear();
         routes.MapRoute(null, "foo", new { a = "foo" });

         Assert.IsNull(Url.RouteUrl(new { a = (string)null }));
         Assert.IsNull(Url.RouteUrl(new { a = "" }));
      }

      [TestMethod]
      public void ValueMustMatchDefaultValueWithoutTokenOrBeOmitted() {

         routes.Clear();
         routes.MapRoute(null, "foo", new { a = "foo" });

         Assert.AreEqual(Url.RouteUrl(new { a = "foo" }), "/foo");
         Assert.AreEqual(Url.RouteUrl(new { }), "/foo");
      }

      [TestMethod]
      public void ConstraintsLimitTheValueSpaceOfAToken() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { }, new { a = "foo|bar" });

         Assert.AreEqual(Url.RouteUrl(new { a = "foo" }), "/foo");
         Assert.AreEqual(Url.RouteUrl(new { a = "bar" }), "/bar");
         Assert.IsNull(Url.RouteUrl(new { a = "xyz" }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/foo");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/bar");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/xyz");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void ConstraintsTestTheWholeValue() {
         
         routes.Clear();
         routes.MapRoute(null, "{a}", new { }, new { a = "foo" });

         Assert.AreEqual(Url.RouteUrl(new { a = "foo" }), "/foo");
         Assert.IsNull(Url.RouteUrl(new { a = "foo2" }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/foo");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/foo2");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void ConstraintsForOptionalTokensShouldMatchAnEmptyString() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "" }, new { a = "foo" });

         Assert.IsNull(Url.RouteUrl(new { a = "" }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "" }, new { a = "(foo)?" });

         Assert.AreEqual(Url.RouteUrl(new { a = "" }), "/");

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void LiteralSubsegmentBug() {

         // http://stackoverflow.com/questions/4318373

         routes.Clear();
         routes.MapRoute(null, "_{a}");

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/_bar");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/__bar");

         Assert.IsNull(routes.GetRouteData(httpContextMock.Object));
      }

      //[TestMethod] // Appears to be fixed in .NET 4.5
      public void TwoConsecutiveOptionalTokensBug() {

         // http://haacked.com/archive/2011/02/20/routing-regression-with-two-consecutive-optional-url-parameters.aspx

         routes.Clear();
         routes.MapRoute(null, "a/{b}/{c}", new { b = "", c = "" });

         Assert.IsNotNull(Url.RouteUrl(new { b = "1", c = "2" }));
         Assert.IsNotNull(Url.RouteUrl(new { b = "1" }));
         Assert.IsNull(Url.RouteUrl(new { }));
      }
   }
}
