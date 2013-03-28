using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.Routing {
   
   [TestClass]
   public class ControllerSelectionBehavior {

      [TestMethod]
      public void CanDisambiguateControllerSelfHost() {

         var config = new HttpConfiguration();
         var routes = config.Routes;

         config.MapCodeRoutes(typeof(ControllerSelection.EntryController));

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/Foo");
         var routeData = routes.GetRouteData(request);

         request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

         var controllerSelector = config.Services.GetHttpControllerSelector();
         var controllerDescriptor = controllerSelector.SelectController(request);

         Assert.IsNotNull(controllerDescriptor);
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.ControllerSelection {

   public class EntryController : ApiController { }

   public class FooController : ApiController {
      public void Get() { }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.Routing.ControllerSelection.SubNamespace {

   public class FooController : ApiController {
      public void Get() { }
   }
}
