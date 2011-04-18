using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Samples.Admin {

   [Authorize]
   public class AdminController : Controller {

      public ActionResult Index() {
         return View();
      }
   }
}