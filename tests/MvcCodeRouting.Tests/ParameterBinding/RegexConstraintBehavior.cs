using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Tests.ParameterBinding {
   
   [TestClass]
   public class RegexConstraintBehavior {

      [TestMethod]
      public void RegexParameterAndBinderParameter_UseBinder() {
         Run(typeof(RegexConstraint.RegexParameterAndBinderParameter.RegexConstraintController), useBinder: true);
      }

      [TestMethod]
      public void RegexGlobalAndBinderParameter_UseBinder() {

         var settings = new CodeRoutingSettings {
            DefaultConstraints = { 
               { typeof(RegexConstraint.Month), "[0-9]{2}" }
            }
         };

         Run(typeof(RegexConstraint.RegexGlobalAndBinderParameter.RegexConstraintController), settings, useBinder: true);
      }

      [TestMethod]
      public void RegexParameterAndBinderGlobal_DontUseBinder() {

         var settings = new CodeRoutingSettings {
            ParameterBinders = { 
               { new RegexConstraint.MonthParameterBinder() }
            }
         };

         Run(typeof(RegexConstraint.RegexParameterAndBinderGlobal.RegexConstraintController), settings);
      }

      [TestMethod]
      public void RegexGlobalAndBinderGlobal_DontUseBinder() {

         var settings = new CodeRoutingSettings {
            DefaultConstraints = { 
               { typeof(RegexConstraint.Month), "[0-9]{2}" }
            },
            ParameterBinders = { 
               { new RegexConstraint.MonthParameterBinder() }
            }
         };

         Run(typeof(RegexConstraint.RegexGlobalAndBinderGlobal.RegexConstraintController), settings);
      }

      [TestMethod]
      public void RegexParameterAndBinderType_DontUseBinder() {
         Run(typeof(RegexConstraint.RegexParameterAndBinderType.RegexConstraintController));
      }

      [TestMethod]
      public void RegexGlobalAndBinderType_DontUseBinder() {

         var settings = new CodeRoutingSettings {
            DefaultConstraints = { 
               { typeof(RegexConstraint.MonthWithAssociatedBinder), "[0-9]{2}" }
            }
         };

         Run(typeof(RegexConstraint.RegexGlobalAndBinderType.RegexConstraintController), settings);
      }

      void Run(Type controller, CodeRoutingSettings settings = null, bool useBinder = false) {

         if (settings == null) {
            settings = new CodeRoutingSettings();
         }

         var routes = new RouteCollection();
         routes.MapCodeRoutes(controller, settings);

         var httpContextMock = new Mock<HttpContextBase>();
         httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/Archive/02");

         var httpResponseMock = new Mock<HttpResponseBase>();
         httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

         var routeData = routes.GetRouteData(httpContextMock.Object);

         var controllerInstance = (Controller)Activator.CreateInstance(controller);
         controllerInstance.ValidateRequest = false;

         var requestContext = new RequestContext(httpContextMock.Object, routeData);
         var controllerContext = new ControllerContext(requestContext, controllerInstance);

         controllerInstance.ValueProvider = new ValueProviderCollection(new IValueProvider[] { new RouteDataValueProvider(controllerContext) });

         ((IController)controllerInstance).Execute(requestContext);

         string expected = (useBinder) ? "02" : "00";

         httpResponseMock.Verify(c => c.Write(It.Is<string>(s => s == expected)), Times.AtLeastOnce());
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.RegexConstraint {

   public struct Month {

      public readonly int Number;

      public static bool TryParse(string value, out Month result) {

         result = default(Month);

         if (String.IsNullOrEmpty(value)
            || value.Length != 2) {

            return false;
         }

         int number;

         if (!Int32.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out number)) {
            return false;
         }

         if (number < 1
            || number > 12) {
            return false;
         }

         result = new Month(number);

         return true;
      }

      private Month(int number) {
         this.Number = number;
      }

      public override string ToString() {
         return this.Number.ToString("00", CultureInfo.InvariantCulture);
      }
   }

   class MonthParameterBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(Month); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = null;

         Month r;

         bool success = Month.TryParse(value, out r);

         if (success) {
            result = r;
         }

         return success;
      }
   }

   [ParameterBinder(typeof(MonthWithAssociatedBinderParameterBinder))]
   public struct MonthWithAssociatedBinder {

      public readonly int Number;

      public static bool TryParse(string value, out MonthWithAssociatedBinder result) {

         result = default(MonthWithAssociatedBinder);

         if (String.IsNullOrEmpty(value)
            || value.Length != 2) {

            return false;
         }

         int number;

         if (!Int32.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out number)) {
            return false;
         }

         if (number < 1
            || number > 12) {
            return false;
         }

         result = new MonthWithAssociatedBinder(number);

         return true;
      }

      private MonthWithAssociatedBinder(int number) {
         this.Number = number;
      }

      public override string ToString() {
         return this.Number.ToString("00", CultureInfo.InvariantCulture);
      }
   }

   class MonthWithAssociatedBinderParameterBinder : ParameterBinder {

      public override Type ParameterType {
         get { return typeof(MonthWithAssociatedBinder); }
      }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = null;

         MonthWithAssociatedBinder r;

         bool success = MonthWithAssociatedBinder.TryParse(value, out r);

         if (success) {
            result = r;
         }

         return success;
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.RegexConstraint.RegexParameterAndBinderParameter {

   public class RegexConstraintController : Controller {

      public Month Archive([Web.Mvc.FromRoute(Constraint = "[0-9]{2}", BinderType = typeof(MonthParameterBinder))]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.RegexConstraint.RegexGlobalAndBinderParameter {

   public class RegexConstraintController : Controller {

      public Month Archive([Web.Mvc.FromRoute(BinderType = typeof(MonthParameterBinder))]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.RegexConstraint.RegexParameterAndBinderGlobal {

   public class RegexConstraintController : Controller {

      public Month Archive([Web.Mvc.FromRoute(Constraint = "[0-9]{2}")]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.RegexConstraint.RegexGlobalAndBinderGlobal {

   public class RegexConstraintController : Controller {

      public Month Archive([Web.Mvc.FromRoute]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.RegexConstraint.RegexParameterAndBinderType {

   public class RegexConstraintController : Controller {

      public MonthWithAssociatedBinder Archive([Web.Mvc.FromRoute(Constraint = "[0-9]{2}")]MonthWithAssociatedBinder? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Tests.ParameterBinding.RegexConstraint.RegexGlobalAndBinderType {

   public class RegexConstraintController : Controller {

      public MonthWithAssociatedBinder Archive([Web.Mvc.FromRoute]MonthWithAssociatedBinder? month) {
         return month.GetValueOrDefault();
      }
   }
}