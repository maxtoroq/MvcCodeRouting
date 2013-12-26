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

      void Run(Type controller, BinderPrecedence.BinderPrecedence expected) {

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

         Assert.AreEqual(expected.ToString(), value);
      }

      [TestMethod]
      public void TypeVsParameter_ParameterWins() {

         var controller = typeof(BinderPrecedence.TypeVsParameter.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter);
      }

      [TestMethod]
      public void TypeVsGlobal_TypeWins() {

         var controller = typeof(BinderPrecedence.TypeVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Type);
      }

      [TestMethod]
      public void ParameterVsGlobal_ParameterWins() {

         var controller = typeof(BinderPrecedence.ParameterVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter);
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

   public class BinderPrecedenceController : ApiController {

      public string Get([ModelBinder(typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.BinderPrecedence.TypeVsGlobal {

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

   public class BinderPrecedenceController : ApiController {

      protected override void Initialize(HttpControllerContext controllerContext) {
         
         base.Initialize(controllerContext);

         this.Configuration.BindParameter(typeof(BinderPrecedenceModel), new BinderPrecedenceGlobalBinder());
      }

      public string Get([ModelBinder]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.BinderPrecedence.ParameterVsGlobal {

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

   public class BinderPrecedenceController : ApiController {

      protected override void Initialize(HttpControllerContext controllerContext) {

         base.Initialize(controllerContext);

         this.Configuration.BindParameter(typeof(BinderPrecedenceModel), new BinderPrecedenceGlobalBinder());
      }

      public string Get([ModelBinder(typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}