using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Samples.Controllers {
   
   public class HomeController : Controller {

      public ActionResult Index() {
         return View();
      }

      public ActionResult MvcVersion() {

         Assembly mvcAssembly = typeof(Controller).Assembly;
         Assembly webApiAssembly = typeof(System.Web.Http.ApiController).Assembly;

         var sb = new StringBuilder()
            .AppendFormat("ASP.NET MVC version: {0} ({1})", mvcAssembly.GetName().Version, ((AssemblyFileVersionAttribute)mvcAssembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version)
            .AppendLine()
            .AppendFormat("ASP.NET Web API version: {0} ({1})", webApiAssembly.GetName().Version, ((AssemblyFileVersionAttribute)webApiAssembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version)
            ;

         return Content(sb.ToString(), "text/plain");
      }
   }
}