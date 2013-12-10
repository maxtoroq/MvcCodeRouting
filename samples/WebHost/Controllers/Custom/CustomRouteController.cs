using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCodeRouting.Web.Mvc;

namespace Samples.Controllers.Custom {
   
   [CustomRoute("~/foo")]
   public class CustomRouteController : Controller {

      public void Index() { }
      public void Foo([FromRoute]int id) { }
      public void Bar([FromRoute]int id) { }
   }
}