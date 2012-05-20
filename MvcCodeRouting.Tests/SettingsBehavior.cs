using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcCodeRouting.Tests {
   
   [TestClass]
   public class SettingsBehavior {

      [TestMethod]
      public void CollectionPropertiesAreNotWrapped() {

         // #747
         // IgnoredControllers is wrapped by new CodeRoutingSettings instance instead of copying its items

         var settings1 = new CodeRoutingSettings { 
            IgnoredControllers = { typeof(Settings.Settings1Controller) } 
         };

         var settings2 = new CodeRoutingSettings(settings1) {
            IgnoredControllers = { typeof(Settings.Settings2Controller) }
         };

         Assert.IsFalse(Object.ReferenceEquals(settings1.IgnoredControllers, settings2.IgnoredControllers));
         Assert.AreEqual(1, settings1.IgnoredControllers.Count);
         Assert.AreEqual(2, settings2.IgnoredControllers.Count);
      }

      [TestMethod]
      public void NewInstanceHasDefaultValues() {

         CodeRoutingSettings.Defaults.Reset();

         var settings1 = new CodeRoutingSettings();

         Assert.AreEqual(CodeRoutingSettings.Defaults.EnableEmbeddedViews, settings1.EnableEmbeddedViews);

         CodeRoutingSettings.Defaults.EnableEmbeddedViews = !CodeRoutingSettings.Defaults.EnableEmbeddedViews;

         var settings2 = new CodeRoutingSettings();

         Assert.AreEqual(CodeRoutingSettings.Defaults.EnableEmbeddedViews, settings2.EnableEmbeddedViews);
      }

      [TestMethod]
      public void NewInstanceCopiesValuesFromProvidedSettings() {

         var settings1 = new CodeRoutingSettings();
         settings1.EnableEmbeddedViews = !settings1.EnableEmbeddedViews;

         var settings2 = new CodeRoutingSettings(settings1);

         Assert.AreEqual(settings1.EnableEmbeddedViews, settings2.EnableEmbeddedViews);
      }
   }
}

namespace MvcCodeRouting.Tests.Settings {

   public class Settings1Controller : Controller { }
   public class Settings2Controller : Controller { }
}
