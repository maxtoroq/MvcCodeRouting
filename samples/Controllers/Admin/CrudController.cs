using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers.Admin {

   [Authorize]
   public abstract class CrudController<TEntity, TEntityKey> : Controller {

      [HttpGet]
      public ActionResult Index() {
         return View();
      }

      [HttpPost]
      public ActionResult Index(TEntity model) {
         throw new NotImplementedException();
      }

      [CustomRoute("{id}")]
      [HttpGet]
      public ActionResult Edit([FromRoute]TEntityKey id) {

         this.ViewBag.Id = id;

         return View();
      }

      [CustomRoute("{id}")]
      [HttpPut]
      public ActionResult Edit([FromRoute]TEntityKey id, TEntity model) {
         throw new NotImplementedException();
      }

      [CustomRoute("{id}")]
      [HttpDelete]
      public ActionResult Edit([FromRoute]TEntityKey id, string foo) {
         throw new NotImplementedException();
      }
   }
}