using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting;

namespace Samples.Controllers.Book {

   public class BookController : Controller {

      [FromRoute]
      public int BookId { get; set; }

      protected override void Initialize(RequestContext requestContext) {
         
         base.Initialize(requestContext);

         this.BindRouteProperties();
      }

      public ActionResult Index() {
         return Content(this.BookId.ToString());
      }

      [HttpDelete]
      public ActionResult Index(string foo) {
         throw new NotImplementedException();
      }

      public ActionResult Chapter([FromRoute]int chapterId, [FromRoute]int page = 1) {
         return Content(chapterId.ToString());
      }
   }
}