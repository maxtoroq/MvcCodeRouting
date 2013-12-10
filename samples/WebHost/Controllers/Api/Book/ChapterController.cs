using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using MvcCodeRouting.Web.Http;

namespace Samples.Controllers.Api.Book {
   
   [CustomRoute("{bookId}/chapter/{id}")]
   public class ChapterController : System.Web.Http.ApiController {

      [FromRoute]
      public int BookId { get; set; }

      [FromRoute("id")]
      public int ChapterId { get; set; }

      protected override void Initialize(HttpControllerContext controllerContext) {

         base.Initialize(controllerContext);

         this.BindRouteProperties();
      }

      public string Get([FromRoute]int page = 1) {
         return String.Join(", ", this.BookId, this.ChapterId.ToString(), page);
      }
   }
}