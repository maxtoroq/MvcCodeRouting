using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Samples {

   public class WebApiConfig {

      public static void Register(HttpConfiguration config) {

         // XML displays better on the browser

         var jsonMediaTypeFormatters = config.Formatters
             .Where(f => f.SupportedMediaTypes.Any(m => m.MediaType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase)))
             .ToList();

         foreach (var formatter in jsonMediaTypeFormatters) {
            config.Formatters.Remove(formatter);
         }
      }
   }
}