using System;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding {
   
   //[TestClass]
   public class BinderPrecedenceFacts {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;

      public BinderPrecedenceFacts() {

         config = new HttpConfiguration();
         routes = config.Routes;
      }

      [TestMethod]
      public void TypeVsParameter_ParameterWins() {

         var controller = typeof(BinderPrecedence.TypeVsParameter.BinderPrecedenceController);

         config.MapCodeRoutes(controller);

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");
         var routeData = routes.GetRouteData(request);

         var controllerInstance = (ApiController)Activator.CreateInstance(controller);

         var controllerContext = new HttpControllerContext(config, routeData, request) {
            ControllerDescriptor = new HttpControllerDescriptor(config, (string)routeData.Values["controller"], controller),
            Controller = controllerInstance
         };

         string value;

         controllerInstance.ExecuteAsync(controllerContext, CancellationToken.None)
            .Result
            .TryGetContentValue(out value);

         Assert.AreEqual(BinderPrecedence.BinderPrecedence.Parameter.ToString(), value);
      }

      [TestMethod]
      public void TypeVsGlobal_TypeWins() {

         var controller = typeof(BinderPrecedence.TypeVsGlobal.BinderPrecedenceController);

         config.MapCodeRoutes(controller);

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");
         var routeData = routes.GetRouteData(request);

         var controllerInstance = (ApiController)Activator.CreateInstance(controller);

         var controllerContext = new HttpControllerContext(config, routeData, request) {
            ControllerDescriptor = new HttpControllerDescriptor(config, (string)routeData.Values["controller"], controller),
            Controller = controllerInstance
         };

         string value;

         controllerInstance.ExecuteAsync(controllerContext, CancellationToken.None)
            .Result
            .TryGetContentValue(out value);

         Assert.AreEqual(BinderPrecedence.BinderPrecedence.Type.ToString(), value);
      }

      [TestMethod]
      public void ParameterVsGlobal_ParameterWins() {

         var controller = typeof(BinderPrecedence.ParameterVsGlobal.BinderPrecedenceController);

         config.MapCodeRoutes(controller);

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");
         var routeData = routes.GetRouteData(request);

         var controllerInstance = (ApiController)Activator.CreateInstance(controller);

         var controllerContext = new HttpControllerContext(config, routeData, request) {
            ControllerDescriptor = new HttpControllerDescriptor(config, (string)routeData.Values["controller"], controller),
            Controller = controllerInstance
         };

         string value;

         controllerInstance.ExecuteAsync(controllerContext, CancellationToken.None)
            .Result
            .TryGetContentValue(out value);

         Assert.AreEqual(BinderPrecedence.BinderPrecedence.Parameter.ToString(), value);
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.BinderPrecedence {

   public enum BinderPrecedence {
      None = 0,
      Parameter,
      Type,
      Global
   } 
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.BinderPrecedence.TypeVsParameter {

   public class BinderPrecedenceController : ApiController {

      public string Get([ModelBinder(typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }

   [ModelBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceParameterBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
         
         bindingContext.Model = new BinderPrecedenceModel { 
            Precedence = BinderPrecedence.Parameter
         };

         return true;
      }
   }

   class BinderPrecedenceTypeBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
         
         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Type
         };

         return true;
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.BinderPrecedence.TypeVsGlobal {

   public class BinderPrecedenceController : ApiController {

      protected override void Initialize(HttpControllerContext controllerContext) {
         
         base.Initialize(controllerContext);

         var provider = new SimpleModelBinderProvider(typeof(BinderPrecedenceModel), new BinderPrecedenceGlobalBinder());

         this.Configuration.Services.Insert(typeof(ModelBinderProvider), 0, provider);
      }

      public string Get([ModelBinder]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }

   [ModelBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceTypeBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Type
         };

         return true;
      }
   }

   class BinderPrecedenceGlobalBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
         
         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Global
         };

         return true;
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.BinderPrecedence.ParameterVsGlobal {

   public class BinderPrecedenceController : ApiController {

      protected override void Initialize(HttpControllerContext controllerContext) {

         base.Initialize(controllerContext);

         var provider = new SimpleModelBinderProvider(typeof(BinderPrecedenceModel), new BinderPrecedenceGlobalBinder());

         this.Configuration.Services.Insert(typeof(ModelBinderProvider), 0, provider);
      }

      public string Get([ModelBinder(typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceParameterBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Parameter
         };

         return true;
      }
   }

   class BinderPrecedenceGlobalBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
         
         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Global
         };

         return true;
      }
   }
}