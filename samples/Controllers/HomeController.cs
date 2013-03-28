using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Samples.Controllers {
   
   public class HomeController : Controller {

      public ActionResult Index() {
         return View();
      }

      public ActionResult MvcVersion() { 
         
         var assembly = typeof(Controller).Assembly;

         return Content(String.Format("ASP.NET MVC version: {0} ({1})", assembly.GetName().Version, ((AssemblyFileVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version));
      }
   }
}