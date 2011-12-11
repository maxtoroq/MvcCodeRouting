// Copyright 2011 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;

namespace MvcCodeRouting {

   /// <summary>
   /// Serves representations of the routes in <see cref="RouteTable.Routes"/> 
   /// for visualization and debugging purposes.
   /// </summary>
   public class RouteDebugHandler : IHttpHandler {

      string format;

      /// <summary>
      /// Gets a value indicating whether another request can use the <see cref="IHttpHandler"/>
      /// instance.
      /// </summary>
      public bool IsReusable { get { return false; } }

      /// <summary>
      /// Gets or sets the default format that the handler should use.
      /// Valid values are: "csharp", "vb".
      /// </summary>
      public static string DefaultFormat { get; set; }

      /// <summary>
      /// Serves representations of the routes in <see cref="RouteTable.Routes"/>.
      /// </summary>
      /// <param name="context">The HTTP context.</param>
      public void ProcessRequest(HttpContext context) {

         var request = context.Request;
         var response = context.Response;

         if (!request.IsLocal) {
            response.StatusCode = 404;
            response.End();
            return;
         }

         string[] formats = new[] { "csharp", "vb" };

         this.format = new[] {  
            request.QueryString["format"],
            DefaultFormat,
            formats[0]
         }.Where(s => formats.Contains(s))
         .First();

         response.ContentType = "text/html";

         switch (this.format) {
            case "csharp":
               response.Write(ToCSharpMapRouteCalls(RouteTable.Routes));
               break;
            
            case "vb":
               response.Write(ToVBMapRouteCalls(RouteTable.Routes));
               break;

            default:
               break;
         }
      }

      string ToCSharpMapRouteCalls(RouteCollection routes) {

         if (routes == null) throw new ArgumentNullException("routes");

         StringBuilder sb = new StringBuilder();
         sb.Append("<!DOCTYPE html>");
         sb.Append("<html>");
         AppendHtmlHead(sb);
         sb.Append("<body class='csharp'>");
         AppendTopComments(sb, "//");

         foreach (Route item in routes.OfType<Route>()) {

            string mapRoute = ToCSharpMapRouteCall(item);

            if (!String.IsNullOrEmpty(mapRoute)) {
               sb.Append(mapRoute)
                  .AppendLine()
                  .AppendLine();
            }
         }

         sb.Append("</body>")
            .Append("</html>");

         return sb.ToString();
      }

      static string ToCSharpMapRouteCall(Route route) {

         if (route == null) throw new ArgumentNullException("route");

         StringBuilder sb = new StringBuilder();

         Type handlerType = route.RouteHandler.GetType();

         if (typeof(StopRoutingHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.IgnoreRoute(<span class='string'>\"{0}\"</span>);", route.Url);

         } else if (typeof(MvcRouteHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.MapRoute(<span class='keyword'>null</span>, <span class='string'>\"{0}\"</span>", route.Url);

            int i = 0;

            if (route.Defaults != null && route.Defaults.Count > 0) {

               sb.Append(", ")
                  .AppendLine()
                  .Append("    <span class='keyword'>new</span> { ");

               foreach (var item in route.Defaults) {

                  if (i > 0)
                     sb.Append(", ");

                  sb.AppendFormat("{0} = {1}", item.Key, ValueToCSharpString(item.Value));

                  i++;
               }

               sb.Append(" }");

               if (route.Constraints != null && route.Constraints.Count > 0) {

                  sb.Append(", ")
                        .AppendLine()
                        .Append("    <span class='keyword'>new</span> { ");

                  int j = 0;

                  foreach (var item in route.Constraints) {

                     if (j > 0)
                        sb.Append(", ");

                     sb.AppendFormat("{0} = {1}", item.Key, ValueToCSharpString(item.Value, constraint: true));

                     j++;
                  }

                  sb.Append(" }");
               }
            }

            string[] namespaces;

            if (route.DataTokens != null && (namespaces = route.DataTokens[DataTokenKeys.Namespaces] as string[]) != null) {

               sb.Append(", ")
                  .AppendLine()
                  .Append("    <span class='keyword'>new</span>[] { ");

               for (int j = 0; j < namespaces.Length; j++) {
                  if (j > 0)
                     sb.Append(", ");

                  sb.Append("<span class='string'>\"")
                     .Append(namespaces[j])
                     .Append("\"</span>");
               }

               sb.Append(" }");
            }

            sb.Append(");");

#if DEBUG
         string crContext = route.DataTokens[DataTokenKeys.CodeRoutingContext] as string;

         if (crContext != null)
            sb.AppendFormat(" <span class='comment'>// {0}: \"{1}\"</span>", DataTokenKeys.CodeRoutingContext, crContext); 
#endif
         }

         return sb.ToString();
      }

      static string ValueToCSharpString(object val, bool constraint = false) {

         string stringVal;
         Type type = (val != null) ? val.GetType() : null;

         if (val == null)
            stringVal = "<span class='keyword'>null</span>";

         else if (type == typeof(string))
            stringVal = String.Concat("<span class='string'>@\"", val, "\"</span>");

         else if (type == typeof(UrlParameter))
            stringVal = "<span class='type'>UrlParameter</span>.Optional";

         else if (constraint)
            stringVal = String.Concat("<span class='keyword'>new</span> ", type.Namespace, (!String.IsNullOrEmpty(type.Namespace) ? "." : ""), "<span class='type'>", type.Name, "</span>", "()");

         else
            stringVal = val.ToString();

         return stringVal;
      }

      string ToVBMapRouteCalls(RouteCollection routes) {

         if (routes == null) throw new ArgumentNullException("routes");

         StringBuilder sb = new StringBuilder();
         sb.Append("<!DOCTYPE html>");
         sb.Append("<html>");
         AppendHtmlHead(sb);
         sb.Append("<body class='vb'>");
         AppendTopComments(sb, "'");

         foreach (Route item in routes.OfType<Route>()) {

            string mapRoute = ToVBMapRouteCall(item);

            if (!String.IsNullOrEmpty(mapRoute)) {
               sb.Append(mapRoute)
                  .AppendLine()
                  .AppendLine();
            }
         }

         sb.Append("</body>")
            .Append("</html>");

         return sb.ToString();
      }

      static string ToVBMapRouteCall(Route route) {

         if (route == null) throw new ArgumentNullException("route");

         StringBuilder sb = new StringBuilder();

         Type handlerType = route.RouteHandler.GetType();

         if (typeof(StopRoutingHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.IgnoreRoute(<span class='string'>\"{0}\"</span>)", route.Url);

         } else if (typeof(MvcRouteHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.MapRoute(<span class='keyword'>Nothing</span>, <span class='string'>\"{0}\"</span>", route.Url);

            int i = 0;

            if (route.Defaults != null && route.Defaults.Count > 0) {

               sb.Append(", _")
                  .AppendLine()
                  .Append("    <span class='keyword'>New With</span> {");

               foreach (var item in route.Defaults) {

                  if (i > 0)
                     sb.Append(", ");

                  sb.AppendFormat(".{0} = {1}", item.Key, ValueToVBString(item.Value));

                  i++;
               }

               sb.Append("}");

               if (route.Constraints != null && route.Constraints.Count > 0) {

                  sb.Append(", _")
                        .AppendLine()
                        .Append("    <span class='keyword'>New With</span> {");

                  int j = 0;

                  foreach (var item in route.Constraints) {

                     if (j > 0)
                        sb.Append(", ");

                     sb.AppendFormat(".{0} = {1}", item.Key, ValueToVBString(item.Value, constraint: true));

                     j++;
                  }

                  sb.Append("}");
               }
            }

            string[] namespaces;

            if (route.DataTokens != null && (namespaces = route.DataTokens[DataTokenKeys.Namespaces] as string[]) != null) {

               sb.Append(", _")
                  .AppendLine()
                  .Append("    <span class='keyword'>New String</span>() {");

               for (int j = 0; j < namespaces.Length; j++) {
                  if (j > 0)
                     sb.Append(", ");

                  sb.Append("<span class='string'>\"")
                     .Append(namespaces[j])
                     .Append("\"</span>");
               }

               sb.Append("}");
            }

            sb.Append(")");

            string crContext = route.DataTokens[DataTokenKeys.CodeRoutingContext] as string;

#if DEBUG
         if (crContext != null)
            sb.AppendFormat(" <span class='comment'>' {0}: \"{1}\"</span>", DataTokenKeys.CodeRoutingContext, crContext);
#endif
         }

         return sb.ToString();
      }

      static string ValueToVBString(object val, bool constraint = false) {

         string stringVal;
         Type type = (val != null) ? val.GetType() : null;

         if (val == null)
            stringVal = "<span class='keyword'>Nothing<span>";

         else if (type == typeof(string))
            stringVal = String.Concat("<span class='string'>\"", val, "\"</span>");

         else if (type == typeof(UrlParameter))
            stringVal = "<span class='type'>UrlParameter</span>.Optional";

         else if (constraint)
            stringVal = String.Concat("<span class='keyword'>New</span> ", type.Namespace, (!String.IsNullOrEmpty(type.Namespace) ? "." : ""), "<span class='type'>", type.Name, "</span>", "()");

         else
            stringVal = val.ToString();

         return stringVal;
      }

      void AppendTopComments(StringBuilder sb, string lineCommentChars) { 

         Assembly thisAssembly = Assembly.GetExecutingAssembly();
         AssemblyName name = thisAssembly.GetName();

         sb.Append("<span class='comment'>")
            .Append(lineCommentChars)
            .AppendFormat(" {0} v{1}", name.Name, name.Version)
            .AppendLine()
            .Append(lineCommentChars)
            .Append(" <a href='http://mvccoderouting.codeplex.com/'>http://mvccoderouting.codeplex.com/</a>")
            .AppendLine()
            .Append(lineCommentChars)
            .Append(" Format: <a" + (this.format == "csharp" ? " class='self'" : " href='?format=csharp'"))
            .Append(">C#</a> - <a" + (this.format == "vb" ? " class='self'" : " href='?format=vb'"))
            .Append(">Visual Basic</a>")
            .Append("</span>")
            .AppendLine()
            .AppendLine();
      }

      static void AppendHtmlHead(StringBuilder sb) {

         sb.Append("<head>")
            .Append("<style type='text/css'>")
            .AppendLine("body { white-space: pre; font-family: Consolas, 'Courier New'; font-size: 80%; }")
            .AppendLine(".comment, .comment a { color: #008000; }")
            .AppendLine(".string { color: #ac1414; }")
            .AppendLine(".keyword { color: #0026fd; }")
            .AppendLine(".type { color: #2b91af; }")
            .AppendLine("a.self { font-weight: bold; }")
            .Append("</style>")
            .Append("</head>")
            ;
      }
   }
}
