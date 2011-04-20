using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Samples.Home {
   
   public class HomeController : Controller {

      public ActionResult Index() {
         return View();
      }

      public ActionResult About() {
         throw new NotImplementedException();
      }

      public ActionResult UrlGenerationTests() {
         return View();
      }
   }
}
