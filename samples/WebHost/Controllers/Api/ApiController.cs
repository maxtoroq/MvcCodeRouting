using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Samples.Controllers.Api {

   public class ApiController : System.Web.Http.ApiController {

      public api Get() {

         string usersUrl = (Request.RequestUri.AbsolutePath.Length > 1) ?
            Url.Route(null, new { controller = "+User" })
            : "/user";

         return new api {
            users = new link { text = "Users", href = usersUrl }
         };
      }
   }

   public class api {
      public link users { get; set; }
   }

   public class link {
      public string href { get; set; }
      public string text { get; set; }
   }
}