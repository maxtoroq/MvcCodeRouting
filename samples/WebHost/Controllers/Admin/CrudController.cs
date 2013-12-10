using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting.Web.Mvc;

namespace Samples.Controllers.Admin {

   [Authorize]
   public abstract class CrudController<TEntity, TEntityKey> : Controller {

      protected virtual IQueryable<TEntity> GetAll() {
         return Enumerable.Empty<TEntity>().AsQueryable();
      }

      protected virtual TEntity GetById(TEntityKey id) {
         return default(TEntity);
      }

      [HttpGet]
      public ActionResult Index() {

         this.ViewData.Model = GetAll();

         return View();
      }

      [HttpPost]
      public ActionResult Index(TEntity model) {
         throw new NotImplementedException();
      }

      [CustomRoute("{id}")]
      [HttpGet]
      public ActionResult Edit([FromRoute]TEntityKey id) {

         this.ViewData.Model = GetById(id);

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