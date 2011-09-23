using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers.Book {

   public class BookController : Controller {

      [FromRoute]
      public int BookId { get; set; }

      public ActionResult Index() {
         throw new NotImplementedException();
      }

      [HttpDelete]
      public ActionResult Index(string foo) {
         throw new NotImplementedException();
      }

      public ActionResult Chapter([FromRoute]int chapterId, [FromRoute]int page = 1) {
         throw new NotImplementedException();
      }
   }
}