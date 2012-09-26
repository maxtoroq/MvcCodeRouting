﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting;

namespace Samples.Controllers {
   
   public class UserController : Controller {

      public ActionResult Index() {
         return View();
      }

      public new ActionResult Profile([FromRoute]string username) {

         this.ViewBag.Username = username;

         return View();
      }

      [RequireRouteParameters]
      public ActionResult Search() {
         throw new NotImplementedException();
      }

      [RequireRouteParameters]
      public ActionResult Search([FromRoute]string q) {
         throw new NotImplementedException();
      }

      [RequireRouteParameters]
      public ActionResult Search([FromRoute]string q, [FromRoute]string sort) {
         throw new NotImplementedException();
      }

      [RequireRouteParameters]
      public ActionResult Search([FromRoute]string q, [FromRoute]string sort, [FromRoute]int startIndex, [FromRoute]int pageSize) {
         throw new NotImplementedException();
      }

      [RequireRouteParameters]
      public ActionResult Search2([FromRoute]string q) {
         throw new NotImplementedException();
      }
   }
}