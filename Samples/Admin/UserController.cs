using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Admin {
   
   [Authorize]
   public class UserController : Controller {

      public ActionResult Index() {
         return View();
      }

      public ActionResult Edit([FromRoute]int id) {

         this.ViewBag.UserId = id;

         return View();
      }

      public ActionResult UrlGenerationTests() {
         return View();
      }
   }
}