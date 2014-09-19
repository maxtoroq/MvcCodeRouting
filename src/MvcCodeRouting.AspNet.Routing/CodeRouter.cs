using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;

namespace MvcCodeRouting.AspNet.Routing {
   
   class CodeRouter : TemplateRoute, ICodeRoute {

      public IDictionary<string, object> DataTokens {
         get { throw new NotImplementedException(); }
      }

      public IDictionary<string, string> ControllerMapping {
         get { throw new NotImplementedException(); }
      }

      public IDictionary<string, string> ActionMapping {
         get { throw new NotImplementedException(); }
      }

      public CodeRouter(string routeTemplate, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
         : base(default(IRouter), routeTemplate, defaults, constraints, default(IInlineConstraintResolver)) { }

      public string GetVirtualPath(VirtualPathContext context) {
         throw new NotImplementedException();
      }

      public Task RouteAsync(RouteContext context) {
         throw new NotImplementedException();
      }
   }
}
