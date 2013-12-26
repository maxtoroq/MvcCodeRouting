using System;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding {
   
   [TestClass]
   public class FromRouteBinderPrecedenceBehavior {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;

      public FromRouteBinderPrecedenceBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;
      }

      void Run(Type controller, FromRouteBinderPrecedence.BinderPrecedence expected) {

         config.MapCodeRoutes(controller);

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/foo");
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

         var controller = typeof(FromRouteBinderPrecedence.TypeVsParameter.BinderPrecedenceController);

         Run(controller, FromRouteBinderPrecedence.BinderPrecedence.Parameter);
      }

      [TestMethod]
      public void TypeVsGlobal_TypeWins() {

         var controller = typeof(FromRouteBinderPrecedence.TypeVsGlobal.BinderPrecedenceController);

         Run(controller, FromRouteBinderPrecedence.BinderPrecedence.Type);
      }

      [TestMethod]
      public void ParameterVsGlobal_ParameterWins() {

         var controller = typeof(FromRouteBinderPrecedence.ParameterVsGlobal.BinderPrecedenceController);

         Run(controller, FromRouteBinderPrecedence.BinderPrecedence.Parameter);
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.FromRouteBinderPrecedence {

   public enum BinderPrecedence {
      None = 0,
      Parameter,
      Type,
      Global
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.FromRouteBinderPrecedence.TypeVsParameter {

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

      public string Get([FromRoute(BinderType = typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.FromRouteBinderPrecedence.TypeVsGlobal {

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

      public string Get([FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ModelBinding.FromRouteBinderPrecedence.ParameterVsGlobal {

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

      public string Get([FromRoute(BinderType = typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}