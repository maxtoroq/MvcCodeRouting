using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting.Web.Mvc;

namespace Samples.Controllers.Custom {

   [CustomRoute("~/blog")]
   public class BlogController : Controller {
      
      [HttpGet]
      [CustomRoute("{year}/{month}/{slug}")]
      public ActionResult Post([FromRoute(Constraint = "[0-9]{4}")]int year, [FromRoute(Constraint = "[0-9]{2}")]int month, [FromRoute]string slug) {
         return View();
      }
      
      [HttpPost]
      [CustomRoute("{year}/{month}/{slug}")]
      public ActionResult Post([FromRoute(Constraint = "[0-9]{4}")]int year, [FromRoute(Constraint = "[0-9]{2}")]int month, [FromRoute]string slug, string foo) {
         return View();
      }

      public ActionResult Archive([FromRoute(Constraint = "[0-9]{4}")]int year, [FromRoute(Constraint = "[0-9]{2}")]int month) {
         return View();
      }
   }
}
