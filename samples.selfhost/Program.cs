using System;
using System.Text.RegularExpressions;
using System.Web.Http.SelfHost;
using MvcCodeRouting;
using MvcCodeRouting.Web.Http;

namespace Samples.SelfHost {

   class Program {

      static void Main() {

         var config = new HttpSelfHostConfiguration("http://localhost:50142/");

         CodeRoutingSettings.Defaults.RouteFormatter = args =>
            Regex.Replace(args.OriginalSegment, @"([a-z])([A-Z])", "$1-$2").ToLowerInvariant();

         CodeRoutingSettings.Defaults.HttpConfiguration = config;

         config.Routes.MapCodeRoutes(typeof(Samples.Controllers.Api.ApiController));

         var server = new HttpSelfHostServer(config);

         server.OpenAsync().Wait();

         Console.WriteLine("Listening on " + config.BaseAddress);
         Console.ReadLine();
      }
   }
}
