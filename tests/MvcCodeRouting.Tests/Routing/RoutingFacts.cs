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
   
   //[TestClass]
   public class RoutingFacts {

      readonly RouteCollection routes;
      readonly UrlHelper Url;

      public RoutingFacts() {
         
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
         Assert.AreEqual("/b", Url.RouteUrl(new { a = "b" }));

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

         Assert.AreEqual("/", Url.RouteUrl(new { }));

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));
      }

      [TestMethod]
      public void CanUseNullOrEmptyStringForParameterWithDefaultValue() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "b" });

         Assert.AreEqual("/", Url.RouteUrl(new { a = (string)null }));
         Assert.AreEqual("/", Url.RouteUrl(new { a = "" }));
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

         Assert.AreEqual("/a", Url.RouteUrl(new { b = "c" }));
         Assert.AreEqual("/a", Url.RouteUrl(new { }));
      }

      [TestMethod]
      public void ConstraintLimitsTheValueSpaceOfParameter() {

         routes.Clear();
         routes.MapRoute(null, "{a}", new { }, new { a = "b|c" });

         Assert.AreEqual("/b", Url.RouteUrl(new { a = "b" }));
         Assert.AreEqual("/c", Url.RouteUrl(new { a = "c" }));
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

         Assert.AreEqual("/b", Url.RouteUrl(new { a = "b" }));
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

         Assert.AreEqual("/", Url.RouteUrl(new { a = "" }));

         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

         Assert.IsNotNull(routes.GetRouteData(httpContextMock.Object));

         //--------------------------------------

         routes.Clear();
         routes.MapRoute(null, "{a}", new { a = "b" }, new { a = "b" });

         Assert.AreEqual("/", Url.RouteUrl(new { a = "" }));

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
         Assert.AreEqual("/a", Url.Action(null));
         Assert.AreEqual("/a", Url.Action(null, (string)null));
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

         Assert.AreEqual("/x", Url.RouteUrl(new { }));
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
      public void RoutesAreEvaluatedInOrder() {

         routes.Clear();
         routes.MapRoute(null, "{a}/{b}", new { b = "c" });
         routes.MapRoute(null, "{a}");

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/x");

         Assert.IsTrue(
            Object.ReferenceEquals(
               routes.GetVirtualPath(Url.RequestContext, new RouteValueDictionary(new { a = "x" })).Route, 
               routes.First()
            )
         );

         Assert.IsTrue(
            Object.ReferenceEquals(
               routes.GetRouteData(httpContextMock.Object).Route, 
               routes.First()
            )
         );

         var lastRoute = routes.Last();
         routes.Remove(lastRoute);
         routes.Insert(0, lastRoute);

         Assert.IsTrue(
            Object.ReferenceEquals(
               routes.GetVirtualPath(Url.RequestContext, new RouteValueDictionary(new { a = "x" })).Route,
               routes.First()
            )
         );

         Assert.IsTrue(
            Object.ReferenceEquals(
               routes.GetRouteData(httpContextMock.Object).Route,
               routes.First()
            )
         );
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
