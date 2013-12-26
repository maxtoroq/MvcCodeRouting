using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcCodeRouting.Web.Http;
using Samples.Models;

namespace Samples.Controllers.Api {

   public class UserController : System.Web.Http.ApiController {
      
      public IEnumerable<User> Get() {
         return Samples.Models.User.All();
      }

      public User Get([FromRoute]int id) {
         return Samples.Models.User.All().SingleOrDefault(u => u.Id == id);
      }

      public void Post([FromBody]User value) {
      }

      public void Put(int id, [FromBody]User value) {
      }

      public void Delete(int id) {
      }
   }
}
