using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.User {
   
   public class UserController : Controller {

      public ActionResult Index() {
         return View();
      }

      public ActionResult Profile([FromRoute]string username) {

         this.ViewBag.Username = username;

         return View();
      }

      public ActionResult UrlGenerationTests() {
         return View();
      }
   }
}