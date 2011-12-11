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

         routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

         routes.MapCodeRoutes(
            rootController: typeof(Controllers.HomeController),
            settings: new CodeRoutingSettings {
               RouteFormatter = args => { 
                  
                  if (args.ControllerType == typeof(Controllers.SomeLongNamespace.SomeController) && args.SegmentType == RouteSegmentType.Namespace)
                     return System.Text.RegularExpressions.Regex.Replace(args.OriginalSegment, @"(\B[A-Z])", "-$1")
                        .ToLowerInvariant();

                  return args.OriginalSegment;
               }
            }
         );
      }

      void RegisterViewEngines(ViewEngineCollection viewEngines) {
         viewEngines.EnableCodeRouting();
      }
   }
}