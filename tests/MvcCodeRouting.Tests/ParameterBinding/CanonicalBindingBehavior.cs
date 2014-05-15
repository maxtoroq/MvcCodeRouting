using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCodeRouting.ParameterBinding;
using MvcCodeRouting.ParameterBinding.Binders;

namespace MvcCodeRouting.Tests.ParameterBinding {
   
   [TestClass]
   public class CanonicalBindingBehavior {

      [TestMethod]
      public void BooleanCanonicalForm() {

         var binder = new BooleanParameterBinder();

         foreach (var item in new[] { "true", "TRUE" }) {

            object result;

            Assert.IsTrue(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
            Assert.AreEqual(true, (bool)result);
         }

         foreach (var item in new[] { "false", "FALSE" }) {

            object result;

            Assert.IsTrue(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
            Assert.AreEqual(false, (bool)result);
         }

         foreach (var item in new[] { "", " ", null, " true", "true " }) {

            object result;

            Assert.IsFalse(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
            Assert.IsNull(result);
         }
      }

      [TestMethod]
      public void IntegerCanonicalForm() {

         var signedIntegers = new ParameterBinder[] { 
            new Int16ParameterBinder(),
            new Int32ParameterBinder(),
            new Int64ParameterBinder(),
            new SByteParameterBinder(),
         };

         var unsignedIntegers = new ParameterBinder[] { 
            new ByteParameterBinder(),
            new UInt16ParameterBinder(),
            new UInt32ParameterBinder(),
            new UInt64ParameterBinder()
         };

         IntegerTests(unsignedIntegers, signedIntegers);
      }

      void IntegerTests(ParameterBinder[] unsignedbinders, ParameterBinder[] signedBinders) {

         foreach (var binder in signedBinders.Concat(unsignedbinders)) {
            
            foreach (var item in new[] { "5", "0" }) {
               
               object result;
               
               Assert.IsTrue(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
               Assert.AreEqual(item, Convert.ToString(result, CultureInfo.InvariantCulture));
            }

            foreach (var item in new[] { "", " ", null, " 5", "5 ", "00", "05", "+0", "+5" }) {
               
               object result;
               
               Assert.IsFalse(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
               Assert.IsNull(result);
            }
         }

         foreach (var binder in signedBinders) {

            foreach (var item in new[] { "-5" }) {

               object result;

               Assert.IsTrue(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
               Assert.AreEqual(item, Convert.ToString(result, CultureInfo.InvariantCulture));
            }

            // TODO: Disallow -0 in v2

            foreach (var item in new[] { /*"-0",*/ "-05" }) {

               object result;

               Assert.IsFalse(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
               Assert.IsNull(result);
            }
         }
      }

      [TestMethod]
      public void DecimalCanonicalForm() {

         var binder = new DecimalParameterBinder();
         
         IntegerTests(new ParameterBinder[0], new ParameterBinder[] { binder });
         DecimalTests(binder);
      }

      void DecimalTests(ParameterBinder binder) { 

         foreach (var item in new[] { "-0.5", "0.5" }) {

            object result;

            Assert.IsTrue(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
            Assert.AreEqual(item, Convert.ToString(result, CultureInfo.InvariantCulture));
         }

         // TODO: Disallow trailing 0 in v2

         foreach (var item in new[] { "-.5", ".5", "+.5"/*, "0.50"*/, "0.", "5." }) {

            object result;

            Assert.IsFalse(binder.TryBind(item, CultureInfo.InvariantCulture, out result));
            Assert.IsNull(result);
         }
      }

      [TestMethod]
      public void SingleCanonicalForm() {

         var binder = new SingleParameterBinder();

         IntegerTests(new ParameterBinder[0], new ParameterBinder[] { binder });
         DecimalTests(binder);
      }

      [TestMethod]
      public void DoubleCanonicalForm() {

         var binder = new DoubleParameterBinder();

         IntegerTests(new ParameterBinder[0], new ParameterBinder[] { binder });
         DecimalTests(binder);
      }

      [TestMethod]
      public void GuidCanonicalForm() {

         var binder = new GuidParameterBinder();

         var guid = Guid.NewGuid();

         foreach (var format in new[] { "D", "B", "P" }) {

            object result;

            Assert.IsTrue(binder.TryBind(guid.ToString(format), CultureInfo.InvariantCulture, out result));
            Assert.AreEqual(guid, result);
         }

         foreach (var format in new[] { "N", "X" }) {

            object result;

            Assert.IsFalse(binder.TryBind(guid.ToString(format), CultureInfo.InvariantCulture, out result));
            Assert.IsNull(result);
         }
      }
   }
}
