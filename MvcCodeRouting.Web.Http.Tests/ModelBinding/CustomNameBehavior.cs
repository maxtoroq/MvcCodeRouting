using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding {
   
   [TestClass]
   public class CustomNameBehavior {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;

      public CustomNameBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;
      }

      [TestMethod]
      public void BindCustomName() {

         var controller = typeof(CustomName.BindCustomNameController);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/hello");
         var routeData = routes.GetRouteData(request);

         var controllerInstance = (IHttpController)Activator.CreateInstance(controller);

         var controllerContext = new HttpControllerContext(config, routeData, request) {
            ControllerDescriptor = new HttpControllerDescriptor(config, (string)routeData.Values["controller"], controller),
            Controller = controllerInstance
         };

         var result = controllerInstance.ExecuteAsync(controllerContext, CancellationToken.None).Result;
         string value;

         Assert.IsTrue(result.TryGetContentValue(out value) && value == "hello");
      }

      [TestMethod]
      public void BindCustomName_Property() {

         var controller = typeof(CustomName.BindCustomNamePropertyController);

         config.MapCodeRoutes(controller, new CodeRoutingSettings { RootOnly = true });

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/hello");
         var routeData = routes.GetRouteData(request);

         var controllerInstance = (IHttpController)Activator.CreateInstance(controller);

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

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.CustomName {

   public class BindCustomNameController : ApiController {

      public string Get([FromRoute("b")]string a) {
         return a;
      }
   }

   public class BindCustomNamePropertyController : ApiController {

      [FromRoute("b")]
      public string a { get; set; }

      protected override void Initialize(HttpControllerContext controllerContext) {
         base.Initialize(controllerContext);
         this.BindRouteProperties();
      }

      public string Get() {
         return this.a;
      }
   }
}
