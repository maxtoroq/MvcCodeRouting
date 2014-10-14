using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MvcCodeRouting.AspNet.Mvc.Tests.Routing {
   
   [TestClass]
   public class RoutingFacts {

      RouteBuilder builder;

      public RoutingFacts() {

         var serviceProviderMock = new Mock<IServiceProvider>();

         this.builder = new RouteBuilder { 
            DefaultHandler = new MvcRouteHandler(),
            ServiceProvider = serviceProviderMock.Object
         };
      }

      [TestMethod]
      public void NameIsOptional() {

         builder.MapRoute(null, "");
      }

      [TestMethod]
      public async Task ParametersAreRequiredByDefault() {

         builder.MapRoute(null, "{a}");

         var routes = builder.Build();

         RouteContext context = CreateRouteContext("/");

         await routes.RouteAsync(context);

         Assert.IsFalse(context.IsHandled);
      }

      static RouteContext CreateRouteContext(string requestPath) {

         var request = new Mock<HttpRequest>(MockBehavior.Strict);
         request.SetupGet(r => r.Path).Returns(new PathString(requestPath));

         var context = new Mock<HttpContext>(MockBehavior.Strict);
         context.SetupGet(c => c.Request).Returns(request.Object);

         return new RouteContext(context.Object);
      }
   }
}
