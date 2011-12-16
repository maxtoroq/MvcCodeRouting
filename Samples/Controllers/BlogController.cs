using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers {
   
   public class BlogController : Controller {

      [CustomRoute("{year}/{month}/{slug}")]
      public ActionResult Post([FromRoute(Constraint = "[0-9]{4}")]int year, [FromRoute(Constraint = "[0-9]{2}")]int month, [FromRoute]string slug) {
         return View();
      }

      public ActionResult Archive([FromRoute(Constraint = "[0-9]{4}")]int year, [FromRoute(Constraint = "[0-9]{2}")]int month) {
         return View();
      }
   }
}
