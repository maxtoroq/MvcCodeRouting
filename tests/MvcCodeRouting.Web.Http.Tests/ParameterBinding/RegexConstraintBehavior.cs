using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCodeRouting.ParameterBinding;

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding {
   
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

         var config = new HttpConfiguration();
         var routes = config.Routes;

         config.MapCodeRoutes(controller, settings);

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/02");
         var routeData = routes.GetRouteData(request);

         var controllerInstance = (ApiController)Activator.CreateInstance(controller);

         var controllerContext = new HttpControllerContext(config, routeData, request) {
            ControllerDescriptor = new HttpControllerDescriptor(config, (string)routeData.Values["controller"], controller),
            Controller = controllerInstance
         };

         object value;
         
         controllerInstance.ExecuteAsync(controllerContext, CancellationToken.None)
            .Result
            .TryGetContentValue(out value);

         string expected = (useBinder) ? "02" : "00";

         Assert.AreEqual(expected, value.ToString());
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.RegexConstraint {

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

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.RegexConstraint.RegexParameterAndBinderParameter {

   public class RegexConstraintController : ApiController {

      public Month Get([FromRoute(Constraint = "[0-9]{2}", BinderType = typeof(MonthParameterBinder))]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.RegexConstraint.RegexGlobalAndBinderParameter {

   public class RegexConstraintController : ApiController {

      public Month Get([FromRoute(BinderType = typeof(MonthParameterBinder))]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.RegexConstraint.RegexParameterAndBinderGlobal {

   public class RegexConstraintController : ApiController {

      public Month Get([FromRoute(Constraint = "[0-9]{2}")]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.RegexConstraint.RegexGlobalAndBinderGlobal {

   public class RegexConstraintController : ApiController {

      public Month Get([FromRoute]Month? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.RegexConstraint.RegexParameterAndBinderType {

   public class RegexConstraintController : ApiController {

      public MonthWithAssociatedBinder Get([FromRoute(Constraint = "[0-9]{2}")]MonthWithAssociatedBinder? month) {
         return month.GetValueOrDefault();
      }
   }
}

namespace MvcCodeRouting.Web.Http.Tests.ParameterBinding.RegexConstraint.RegexGlobalAndBinderType {

   public class RegexConstraintController : ApiController {

      public MonthWithAssociatedBinder Get([FromRoute]MonthWithAssociatedBinder? month) {
         return month.GetValueOrDefault();
      }
   }
}