using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers.Api {
   
   public abstract class CrudController<TEntity, TEntityKey> : Controller {

      [HttpGet]
      public ActionResult Index() {
         throw new NotImplementedException();
      }

      [HttpPost]
      public ActionResult Index(TEntity model) {
         throw new NotImplementedException();
      }

      [CustomRoute("{id}")]
      [HttpGet]
      public ActionResult Entity([FromRoute]TEntityKey id) {
         throw new NotImplementedException();
      }

      [CustomRoute("{id}")]
      [HttpPut]
      public ActionResult Entity([FromRoute]TEntityKey id, TEntity model) {
         throw new NotImplementedException();
      }

      [CustomRoute("{id}")]
      [HttpDelete]
      public ActionResult Entity([FromRoute]TEntityKey id, string foo) {
         throw new NotImplementedException();
      }
   }
}