using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers.Api {
   
   public abstract class CrudController<TEntity, TEntityKey> : Controller {

      [HttpGet]
      [RequireRouteParameters]
      public ActionResult Entities() {
         throw new NotImplementedException();
      }

      [HttpPost]
      public ActionResult Entities(TEntity model) {
         throw new NotImplementedException();
      }

      [HttpGet]
      [RequireRouteParameters]
      public ActionResult Entities([FromRoute]TEntityKey id) {
         throw new NotImplementedException();
      }

      [HttpPut]
      public ActionResult Entities([FromRoute]TEntityKey id, TEntity model) {
         throw new NotImplementedException();
      }

      [HttpDelete]
      public ActionResult Entities([FromRoute]TEntityKey id, string foo) {
         throw new NotImplementedException();
      }
   }
}