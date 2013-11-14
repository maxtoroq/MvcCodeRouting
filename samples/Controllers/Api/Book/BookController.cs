using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcCodeRouting.Web.Http;

namespace Samples.Controllers.Api.Book {

   public class BookController : System.Web.Http.ApiController {

      public void Get() {
         throw new NotImplementedException();
      }

      [HttpGet]
      public void Search() {
         throw new NotImplementedException();
      }

      [HttpGet]
      public void New() {
         throw new NotImplementedException();
      }

      public string Get([FromRoute]int id) {
         return id.ToString();
      }

      public string Delete([FromRoute]int id, string foo) {
         throw new NotImplementedException();
      }
   }
}