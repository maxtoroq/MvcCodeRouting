using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers {
   
   public class AsyncController : System.Web.Mvc.AsyncController {

      public void IndexAsync() { }

      public ActionResult IndexCompleted() {
         return Content("async!");
      }

      public ActionResult Sync() {
         return Content("sync!");
      }
   }
}