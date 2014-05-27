using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Tests.ParameterBinding {
   
   [TestClass]
   public class BinderPrecedenceBehavior {

      readonly RouteCollection routes;

      public BinderPrecedenceBehavior() {
         routes = TestUtil.GetRouteCollection();
      }

      void Run(Type controller, BinderPrecedence.BinderPrecedence expected, ParameterBinder globalBinder = null) {

         var settings = new CodeRoutingSettings();

         if (globalBinder != null) {
            settings.ParameterBinders.Add(globalBinder);
         }

         routes.Clear();
         routes.MapCodeRoutes(controller, settings);

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/index/foo");

         var httpResponseMock = new Mock<HttpResponseBase>();
         httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

         var routeData = routes.GetRouteData(httpContextMock.Object);

         var controllerInstance = (Controller)Activator.CreateInstance(controller);
         controllerInstance.ValidateRequest = false;

         var requestContext = new RequestContext(httpContextMock.Object, routeData);
         var controllerContext = new ControllerContext(requestContext, controllerInstance);

         controllerInstance.ValueProvider = new ValueProviderCollection(new IValueProvider[] { new RouteDataValueProvider(controllerContext) });

         ((IController)controllerInstance).Execute(requestContext);

         httpResponseMock.Verify(c => c.Write(It.Is<string>(s => s == expected.ToString())), Times.AtLeastOnce());
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

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter, new BinderPrecedence.ParameterVsGlobal.BinderPrecedenceGlobalBinder());
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

      // TODO: Change these to match Web.Http in v2 ?

      [TestMethod]
      public void ModelTypeVsGlobal_ModelWins() {

         var controller = typeof(BinderPrecedence.ModelTypeVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Model);
      }

      [TestMethod]
      public void ModelTypeVsType_ModelWins() {

         var controller = typeof(BinderPrecedence.ModelTypeVsType.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Model);
      }

      [TestMethod]
      public void ModelGlobalVsParameter_ParameterWins() {

         var controller = typeof(BinderPrecedence.ModelGlobalVsParameter.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Parameter);
      }

      [TestMethod]
      public void ModelGlobalVsGlobal_ModelWins() {

         var controller = typeof(BinderPrecedence.ModelGlobalVsGlobal.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Model, new BinderPrecedence.ModelGlobalVsGlobal.BinderPrecedenceGlobalBinder());
      }

      [TestMethod]
      public void ModelGlobalVsType_ModelWins() {

         var controller = typeof(BinderPrecedence.ModelGlobalVsType.BinderPrecedenceController);

         Run(controller, BinderPrecedence.BinderPrecedence.Model);
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence {

   public enum BinderPrecedence {
      None = 0,
      Parameter,
      Type,
      Global,
      Model
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.TypeVsParameter {

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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute(BinderType = typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.TypeVsGlobal {

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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ParameterVsGlobal {

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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute(BinderType = typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelParameterVsGlobal {

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute(BinderType = typeof(BinderPrecedenceModelBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelParameterVsType {

   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute(BinderType = typeof(BinderPrecedenceModelBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelTypeVsParameter {

   [ModelBinder(typeof(BinderPrecedenceModelBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute(BinderType = typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelTypeVsGlobal {

   [ModelBinder(typeof(BinderPrecedenceModelBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelTypeVsType {

   [ModelBinder(typeof(BinderPrecedenceModelBinder))]
   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      public string Index([Web.Mvc.FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelGlobalVsParameter {

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      static BinderPrecedenceController() {
         ModelBinders.Binders[typeof(BinderPrecedenceModel)] = new BinderPrecedenceModelBinder();
      }

      public string Index([Web.Mvc.FromRoute(BinderType = typeof(BinderPrecedenceParameterBinder))]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelGlobalVsGlobal {

   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      static BinderPrecedenceController() {
         ModelBinders.Binders[typeof(BinderPrecedenceModel)] = new BinderPrecedenceModelBinder();
      }

      public string Index([Web.Mvc.FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.BinderPrecedence.ModelGlobalVsType {

   [ParameterBinder(typeof(BinderPrecedenceTypeBinder))]
   public class BinderPrecedenceModel {
      public BinderPrecedence Precedence { get; set; }
   }

   class BinderPrecedenceModelBinder : IModelBinder {

      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         return new BinderPrecedenceModel {
            Precedence = BinderPrecedence.Model
         };
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

   public class BinderPrecedenceController : Controller {

      static BinderPrecedenceController() {
         ModelBinders.Binders[typeof(BinderPrecedenceModel)] = new BinderPrecedenceModelBinder();
      }

      public string Index([Web.Mvc.FromRoute]BinderPrecedenceModel model) {
         return model.Precedence.ToString();
      }
   }
}
