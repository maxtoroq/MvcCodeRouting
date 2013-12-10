using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting.Web.Mvc;

namespace Samples.Controllers.Book {

   public class BookController : Controller {

      public ActionResult Index() {
         throw new NotImplementedException();
      }

      public ActionResult Search() {
         throw new NotImplementedException();
      }

      public ActionResult New() {
         throw new NotImplementedException();
      }

      [CustomRoute("{id}")]
      public ActionResult Details([FromRoute]int id) {
         return Content(id.ToString());
      }

      [CustomRoute("{id}")]
      [HttpDelete]
      public ActionResult Details([FromRoute]int id, string foo) {
         throw new NotImplementedException();
      }
   }
}