using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding {
   
   [TestClass]
   public class BinderPrecedenceBehavior {

      readonly HttpConfiguration config;
      readonly HttpRouteCollection routes;

      public BinderPrecedenceBehavior() {

         config = new HttpConfiguration();
         routes = config.Routes;
      }

      void Run(Type controller, BinderPrecedence.BinderPrecedence expected, ParameterBinder globalBinder = null) {

         var settings = new CodeRoutingSettings();

         if (globalBinder != null) {
            settings.ParameterBinders.Add(globalBinder);
         }

         config.MapCodeRoutes(controller, settings);

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

         var controller = typeof(BinderPrecedence.TypeVsParameter.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter);
      }

      [TestMethod]
      public void TypeVsGlobal_GlobalWins() {

         var controller = typeof(BinderPrecedence.TypeVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Global, new BinderPrecedence.TypeVsGlobal.BinderPrecedenceGlobalBinder());
      }

      [TestMethod]
      public void ParameterVsGlobal_ParameterWins() {

         var controller = typeof(BinderPrecedence.ParameterVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter, new BinderPrecedence.TypeVsGlobal.BinderPrecedenceGlobalBinder());
      }

      [TestMethod]
      public void ModelParameterVsGlobal_ModelWins() {

         var controller = typeof(BinderPrecedence.ModelParameterVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Model, new BinderPrecedence.ModelParameterVsGlobal.BinderPrecedenceGlobalBinder());
      }

      [TestMethod]
      public void ModelParameterVsType_ModelWins() {

         var controller = typeof(BinderPrecedence.ModelParameterVsType.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Model);
      }

      [TestMethod]
      public void ModelTypeVsParameter_ParameterWins() {

         var controller = typeof(BinderPrecedence.ModelTypeVsParameter.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter);
      }

      [TestMethod]
      public void ModelTypeVsGlobal_GlobalWins() {

         var controller = typeof(BinderPrecedence.ModelTypeVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Global, new BinderPrecedence.ModelTypeVsGlobal.BinderPrecedenceGlobalBinder());
      }

      [TestMethod]
      public void ModelTypeVsType_TypeWins() {

         var controller = typeof(BinderPrecedence.ModelTypeVsType.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Type);
      }

      [TestMethod]
      public void ModelGlobalVsParameter_ParameterWins() {

         var controller = typeof(BinderPrecedence.ModelGlobalVsParameter.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter);
      }

      [TestMethod]
      public void ModelGlobalVsGlobal_GlobalWins() {

         var controller = typeof(BinderPrecedence.ModelGlobalVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Global, new BinderPrecedence.ModelGlobalVsGlobal.BinderPrecedenceGlobalBinder());
      }

      [TestMethod]
      public void ModelGlobalVsType_TypeWins() {

         var controller = typeof(BinderPrecedence.ModelGlobalVsType.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Type);
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence {

   public enum BinderPrecedence {
      None = 0,
      Parameter,
      Type,
      Global,
      Model
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.TypeVsParameter {

   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceParameterBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Parameter
         };

         return true;
      }
   }

   class BinderPrecedenceTypeBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
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

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.TypeVsGlobal {

   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceTypeBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Type
         };

         return true;
      }
   }

   class BinderPrecedenceGlobalBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Global
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      public string Get([FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ParameterVsGlobal {

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceParameterBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Parameter
         };

         return true;
      }
   }

   class BinderPrecedenceGlobalBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Global
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

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelParameterVsGlobal {

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceGlobalBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Global
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      public string Get([FromRoute(BinderType = typeof(BinderPrecedenceModelBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelParameterVsType {

   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceTypeBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Type
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      public string Get([FromRoute(BinderType = typeof(BinderPrecedenceModelBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelTypeVsParameter {

   [ModelBinder(typeof(BinderPrecedenceModelBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceParameterBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Parameter
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

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelTypeVsGlobal {

   [ModelBinder(typeof(BinderPrecedenceModelBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceGlobalBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Global
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      public string Get([FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelTypeVsType {

   [ModelBinder(typeof(BinderPrecedenceModelBinder))]
   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceTypeBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Type
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      public string Get([FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelGlobalVsParameter {

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceParameterBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Parameter
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      protected override void Initialize(HttpControllerContext controllerContext) {

         base.Initialize(controllerContext);

         this.Configuration.BindParameter(typeof(BinderPrecedenceModel), new BinderPrecedenceModelBinder());
      }

      public string Get([FromRoute(BinderType = typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelGlobalVsGlobal {

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceGlobalBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Global
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      protected override void Initialize(HttpControllerContext controllerContext) {

         base.Initialize(controllerContext);

         this.Configuration.BindParameter(typeof(BinderPrecedenceModel), new BinderPrecedenceModelBinder());
      }

      public string Get([FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.BinderPrecedence.ModelGlobalVsType {

   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {

         bindingContext.Model = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };

         return true;
      }
   }

   class BinderPrecedenceTypeBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(BinderPrecedenceModel); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Type
         };

         return true;
      }
   }

   public class BinderPrecedenceController : ApiController {

      protected override void Initialize(HttpControllerContext controllerContext) {

         base.Initialize(controllerContext);

         this.Configuration.BindParameter(typeof(BinderPrecedenceModel), new BinderPrecedenceModelBinder());
      }

      public string Get([FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}