using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting.Web.Mvc;

namespace Samples.Controllers.Book {
   
   [CustomRoute("{bookId}/chapter/{id}")]
   public class ChapterController : Controller {

      [FromRoute]
      public int BookId { get; set; }

      [FromRoute("id")]
      public int ChapterId { get; set; }

      protected override void Initialize(RequestContext requestContext) {

         base.Initialize(requestContext);

         this.BindRouteProperties();
      }

      [CustomRoute("{page}")]
      public ActionResult Index([FromRoute]int page = 1) {
         return Content(String.Join(", ", this.BookId, this.ChapterId.ToString(), page));
      }
   }
}