using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Samples.Controllers {
   
   public class AccountController : Controller {

      [Authorize]
      public ActionResult Index() {
         return View();
      }

      public ActionResult LogOn() {
         return View();
      }

      [HttpPost]
      public ActionResult LogOn(string username, string password, string returnUrl = null) {

         if (!FormsAuthentication.Authenticate(username, password)) {
            this.ModelState.AddModelError("", "Username and password do not match.");
            return View();
         }

         FormsAuthentication.SetAuthCookie(username, createPersistentCookie: false);

         return Redirect(returnUrl ?? this.Url.Action("", "~Home"));
      }

      public ActionResult LogOnStatus() {

         if (this.ControllerContext.IsChildAction) {
            return PartialView();
         }

         return new EmptyResult();
      }

      public ActionResult LogOff() {
         
         FormsAuthentication.SignOut();
         
         return RedirectToAction("", "~Home");
      }

      public ActionResult Register() {
         throw new NotImplementedException();
      }

      [Authorize]
      public ActionResult ChangePassword() {
         throw new NotImplementedException();
      }

      public ActionResult ChangePasswordSuccess() {
         throw new NotImplementedException();
      }
   }
}
