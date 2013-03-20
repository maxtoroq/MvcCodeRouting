using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Hosting;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class FromRouteAttributeBehavior {

      [TestMethod]
      public void UseCustomName() {

         var controller = typeof(FromRouteAttr.FromRouteAttributeController);

         var config = new HttpConfiguration();
         var routes = config.Routes;

         var settings = new CodeRoutingSettings();
         settings.HttpConfiguration(config);

         routes.MapCodeRoutes(controller, settings);

         Assert.IsNotNull(routes.First().RouteTemplate.Contains("{b}"));
      }

      [TestMethod]
      public void BindCustomName() {

         var controller = typeof(FromRouteAttr.FromRouteAttributeController);

         var config = new HttpConfiguration();
         var routes = config.Routes;
         
         var settings = new CodeRoutingSettings();
         settings.HttpConfiguration(config);

         routes.MapCodeRoutes(controller, settings);
         
         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/hello");
         var routeData = routes.GetRouteData(request);

         var controllerInstance = new FromRouteAttr.FromRouteAttributeController();

         var controllerContext = new HttpControllerContext(config, routeData, request) {
            ControllerDescriptor = new HttpControllerDescriptor(config, (string)routeData.Values["controller"], controller),
            Controller = controllerInstance
         };

         var result = controllerInstance.ExecuteAsync(controllerContext, CancellationToken.None).Result;
         string value;

         Assert.IsTrue(result.TryGetContentValue(out value) && value == "hello");
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.FromRouteAttr {

   public class FromRouteAttributeController : ApiController {

      public string Get([FromRoute("b")]string a) {
         return a;
      }
   }
}
