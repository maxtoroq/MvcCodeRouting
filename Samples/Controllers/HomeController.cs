using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers {
   
   public class HomeController : Controller {

      public ActionResult Index() {
         return View();
      }

      public ActionResult About([FromRoute]string id) {
         throw new NotImplementedException();
      }
   }
}