using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting;

namespace Samples {

   public class MvcApplication : System.Web.HttpApplication {

      void Application_Start() {

         RegisterRoutes(RouteTable.Routes);
         RegisterViewEngines(ViewEngines.Engines);
      }

      void RegisterRoutes(RouteCollection routes) {

         routes.RouteExistingFiles = true;

         routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

         routes.MapCodeRoutes(
            baseNamespace: typeof(MvcApplication).Namespace
         );
      }

      void RegisterViewEngines(ViewEngineCollection viewEngines) {
         viewEngines.EnableCodeRouting();
      }
   }
}