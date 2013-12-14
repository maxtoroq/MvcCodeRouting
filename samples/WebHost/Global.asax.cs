using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting;
using System.Web.Http;

namespace Samples {

   public class MvcApplication : System.Web.HttpApplication {
      
      protected void Application_Start() {

         RouteConfig.RegisterRoutes(RouteTable.Routes);
         FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
         ViewEngineConfig.RegisterViewEngines(ViewEngines.Engines);
         WebApiConfig.Register(GlobalConfiguration.Configuration);
      }
   }
}