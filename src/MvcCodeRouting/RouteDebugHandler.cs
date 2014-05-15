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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeRouting {

   /// <summary>
   /// Serves representations of the routes in <see cref="RouteTable.Routes"/> 
   /// for visualization and debugging purposes.
   /// </summary>
   [EditorBrowsable(EditorBrowsableState.Never)]
   public class RouteDebugHandler : IHttpHandler {

      string format;
      TextWriter writer;

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

         this.writer = response.Output;

         response.ContentType = "text/html";

         switch (this.format) {
            case "csharp":
               RenderRoutesCSharp(RouteTable.Routes);
               break;

            case "vb":
               RenderRoutesVB(RouteTable.Routes);
               break;

            default:
               break;
         }
      }

      void RenderRoutesCSharp(RouteCollection routes) {

         if (routes == null) throw new ArgumentNullException("routes");

         writer.Write("<!DOCTYPE html>");
         writer.Write("<html>");
         RenderHtmlHead();
         writer.Write("<body class='csharp'>");
         RenderTopComments("//");
         writer.WriteLine();

         string prevRouteContext = null;

         foreach (Route route in routes.OfType<Route>()) {

            string routeContext = (route.DataTokens != null) ?
               route.DataTokens[DataTokenKeys.RouteContext] as string
               : null;

            if (routeContext != prevRouteContext) {

               if (prevRouteContext != null) {
                  writer.WriteLine();
                  writer.WriteLine("<span class='keyword'>#endregion</span>");
               }

               if (routeContext != null) {

                  string display = routeContext.Length == 0 ? "(root)" : routeContext;

                  writer.WriteLine();
                  writer.WriteLine("<span class='keyword'>#region</span> " + display);
               }
            }

            writer.WriteLine();
            RenderRouteCSharp(route);

            prevRouteContext = routeContext;
         }

         if (prevRouteContext != null) {
            writer.WriteLine();
            writer.Write("<span class='keyword'>#endregion</span>");
         }

         writer.Write("</body>");
         writer.Write("</html>");
      }

      void RenderRouteCSharp(Route route) {

         if (route == null) throw new ArgumentNullException("route");

         Type handlerType = route.RouteHandler.GetType();

         if (typeof(StopRoutingHandler).IsAssignableFrom(handlerType)) {
            RenderIgnoreRouteCSharp(route);

         } else if (IsMvcHandler(handlerType)) {
            RenderMapRouteCSharp(route);

         } else if (IsWebApiHandler(handlerType)) {
            RenderMapHttpRouteCSharp(route);

         } else {
            writer.Write("<span class='comment'>// route: \"{0}\", handler: {1}</span>", route.Url, handlerType.AssemblyQualifiedName);
         }

         writer.WriteLine();
      }

      void RenderIgnoreRouteCSharp(Route route) {
         writer.Write("routes.IgnoreRoute(<span class='string url'>\"{0}\"</span>);", route.Url);
      }

      void RenderMapRouteCSharp(Route route, bool httpRoute = false) {

         writer.Write("routes.");
         writer.Write(httpRoute ? "MapHttpRoute" : "MapRoute");
         writer.Write("(<span class='keyword'>null</span>, <span class='string url'>\"{0}\"</span>", route.Url);

         int i = 0;

         if (route.Defaults != null
            && route.Defaults.Count > 0) {

            writer.Write(", ");
            writer.WriteLine();
            writer.Write("    <span class='keyword'>new</span> { ");

            foreach (var item in route.Defaults) {

               if (i > 0) {
                  writer.Write(", ");
               }

               writer.Write("{0} = {1}", item.Key, ValueToCSharpString(item.Value));

               i++;
            }

            writer.Write(" }");

            var constraints = (route.Constraints != null) ?
               new RouteValueDictionary(route.Constraints)
               : new RouteValueDictionary();

            if (constraints.Count > 0) {

               writer.Write(", ");
               writer.WriteLine();
               writer.Write("    <span class='keyword'>new</span> { ");

               int j = 0;

               foreach (var item in constraints) {

                  if (j > 0) {
                     writer.Write(", ");
                  }

                  writer.Write("{0} = {1}", item.Key, ValueToCSharpString(item.Value, constraint: true));

                  j++;
               }

               writer.Write(" }");
            }
         }

         if (!httpRoute) {

            string[] namespaces;

            if (route.DataTokens != null
               && (namespaces = route.DataTokens[DataTokenKeys.Namespaces] as string[]) != null) {

               writer.Write(", ");
               writer.WriteLine();
               writer.Write("    <span class='keyword'>new</span>[] { ");

               for (int j = 0; j < namespaces.Length; j++) {

                  if (j > 0) {
                     writer.Write(", ");
                  }

                  writer.Write("<span class='string'>\"");
                  writer.Write(namespaces[j]);
                  writer.Write("\"</span>");
               }

               writer.Write(" }");
            }
         }

         writer.Write(");");
      }

      void RenderMapHttpRouteCSharp(Route route) {
         RenderMapRouteCSharp(route, httpRoute: true);
      }

      static string ValueToCSharpString(object val, bool constraint = false) {

         string stringVal;
         Type type = (val != null) ? val.GetType() : null;

         if (val == null) {
            stringVal = "<span class='keyword'>null</span>";

         } else if (type == typeof(string)) {
            stringVal = String.Concat("<span class='string'>", (constraint ? "@" : ""), "\"", val.ToString(), "\"</span>");

         } else if (IsMvcParameter(type)) {
            stringVal = String.Concat("<span class='type' title='", type.FullName, "'>UrlParameter</span>.Optional");

         } else if (IsWebApiParameter(type)) {
            stringVal = String.Concat("<span class='type' title='", type.FullName, "'>RouteParameter</span>.Optional");

         } else if (constraint) {

            var regexConstraint = val as Web.Routing.RegexRouteConstraint;

            if (regexConstraint != null) {
               stringVal = String.Concat("<span class='keyword'>new</span> ", TypeReferenceCSharp(type), "(", ValueToCSharpString(regexConstraint.OriginalPattern, constraint: true), ")");

            } else {

               var paramBindingConstraint = val as Web.Routing.ParameterBindingRouteConstraint;

               if (paramBindingConstraint != null) {
                  stringVal = String.Concat("<span class='keyword'>new</span> ", TypeReferenceCSharp(type), "(", ValueToCSharpString(paramBindingConstraint.Binder, constraint: true), ")");

               } else {

                  var setConstraint = val as Web.Routing.SetRouteConstraint;

                  if (setConstraint != null) {
                     stringVal = String.Concat("<span class='keyword'>new</span> ", TypeReferenceCSharp(type), "(", String.Join(", ", setConstraint.GetValues().Select(s => ValueToCSharpString(s))), ")");
                  
                  } else {
                     stringVal = String.Concat("<span class='keyword'>new</span> ", TypeReferenceCSharp(type), "()");
                  }
               }
            }

         } else {
            stringVal = val.ToString();
         }

         return stringVal;
      }

      static string TypeReferenceCSharp(Type type) {

         var sb = new StringBuilder();
         sb.AppendFormat("<span class='type' title='{0}'>", type.FullName);
         sb.Append((type.IsGenericType) ? type.Name.Substring(0, type.Name.IndexOf('`')) : type.Name);
         sb.Append("</span>");

         if (type.IsGenericType) {

            Type[] genericArgs = type.GetGenericArguments();

            sb.Append("&lt;");

            for (int i = 0; i < genericArgs.Length; i++) {

               if (i > 0) {
                  sb.Append(", ");
               }

               sb.Append(TypeReferenceCSharp(genericArgs[i]));
            }

            sb.Append(">"); 
         }

         return sb.ToString();
      }

      void RenderRoutesVB(RouteCollection routes) {

         if (routes == null) throw new ArgumentNullException("routes");

         writer.Write("<!DOCTYPE html>");
         writer.Write("<html>");
         RenderHtmlHead();
         writer.Write("<body class='vb'>");
         RenderTopComments("'");
         writer.WriteLine();

         string prevRouteContext = null;

         foreach (Route route in routes.OfType<Route>()) {

            string routeContext = (route.DataTokens != null) ?
               route.DataTokens[DataTokenKeys.RouteContext] as string
               : null;

            if (routeContext != prevRouteContext) {

               if (prevRouteContext != null) {
                  writer.WriteLine();
                  writer.WriteLine("<span class='keyword'>#End Region</span>");
               }

               if (routeContext != null) {

                  string display = routeContext.Length == 0 ? "(root)" : routeContext;

                  writer.WriteLine();
                  writer.Write("<span class='keyword'>#Region</span> ");
                  writer.Write(ValueToVBString(display));
                  writer.WriteLine();
               }
            }

            writer.WriteLine();
            RenderRouteVB(route);

            prevRouteContext = routeContext;
         }

         if (prevRouteContext != null) {
            writer.WriteLine();
            writer.Write("<span class='keyword'>#End Region</span>");
         }

         writer.Write("</body>");
         writer.Write("</html>");
      }

      void RenderRouteVB(Route route) {

         if (route == null) throw new ArgumentNullException("route");

         Type handlerType = route.RouteHandler.GetType();

         if (typeof(StopRoutingHandler).IsAssignableFrom(handlerType)) {
            RenderIgnoreRouteVB(route);

         } else if (IsMvcHandler(handlerType)) {
            RenderMapRouteVB(route);

         } else if (IsWebApiHandler(handlerType)) {
            RenderMapHttpRouteVB(route);

         } else {
            writer.Write("<span class='comment'>' route: \"{0}\", handler: {1}</span>", route.Url, handlerType.AssemblyQualifiedName);
         }

         writer.WriteLine();
      }

      void RenderIgnoreRouteVB(Route route) {
         writer.Write("routes.IgnoreRoute(<span class='string url'>\"{0}\"</span>)", route.Url);
      }

      void RenderMapRouteVB(Route route, bool httpRoute = false) {

         writer.Write("routes.");
         writer.Write(httpRoute ? "MapHttpRoute" : "MapRoute");
         writer.Write("(<span class='keyword'>Nothing</span>, <span class='string url'>\"{0}\"</span>", route.Url);

         int i = 0;

         if (route.Defaults != null
            && route.Defaults.Count > 0) {

            writer.Write(", _");
            writer.WriteLine();
            writer.Write("    <span class='keyword'>New With</span> {");

            foreach (var item in route.Defaults) {

               if (i > 0) {
                  writer.Write(", ");
               }

               writer.Write(".{0} = {1}", item.Key, ValueToVBString(item.Value));

               i++;
            }

            writer.Write("}");

            var constraints = (route.Constraints != null) ?
               new RouteValueDictionary(route.Constraints)
               : new RouteValueDictionary();

            if (constraints.Count > 0) {

               writer.Write(", _");
               writer.WriteLine();
               writer.Write("    <span class='keyword'>New With</span> {");

               int j = 0;

               foreach (var item in constraints) {

                  if (j > 0) {
                     writer.Write(", ");
                  }

                  writer.Write(".{0} = {1}", item.Key, ValueToVBString(item.Value, constraint: true));

                  j++;
               }

               writer.Write("}");
            }
         }

         if (!httpRoute) {

            string[] namespaces;

            if (route.DataTokens != null
               && (namespaces = route.DataTokens[DataTokenKeys.Namespaces] as string[]) != null) {

               writer.Write(", _");
               writer.WriteLine();
               writer.Write("    <span class='keyword'>New String</span>() {");

               for (int j = 0; j < namespaces.Length; j++) {

                  if (j > 0) {
                     writer.Write(", ");
                  }

                  writer.Write("<span class='string'>\"");
                  writer.Write(namespaces[j]);
                  writer.Write("\"</span>");
               }

               writer.Write("}");
            }
         }

         writer.Write(")");
      }

      void RenderMapHttpRouteVB(Route route) {
         RenderMapRouteVB(route, httpRoute: true);
      }

      static string ValueToVBString(object val, bool constraint = false) {

         string stringVal;
         Type type = (val != null) ? val.GetType() : null;

         if (val == null) {
            stringVal = "<span class='keyword'>Nothing<span>";

         } else if (type == typeof(string)) {
            stringVal = String.Concat("<span class='string'>\"", val.ToString(), "\"</span>");

         } else if (IsMvcParameter(type)) {
            stringVal = String.Concat("<span class='type' title='", type.FullName, "'>UrlParameter</span>.Optional");

         } else if (IsWebApiParameter(type)) {
            stringVal = String.Concat("<span class='type' title='", type.FullName, "'>RouteParameter</span>.Optional");

         } else if (constraint) {

            var regexConstraint = val as Web.Routing.RegexRouteConstraint;

            if (regexConstraint != null) {
               stringVal = String.Concat("<span class='keyword'>New</span> ", TypeReferenceVB(type), "(", ValueToVBString(regexConstraint.OriginalPattern, constraint: true), ")");

            } else {

               var paramBindingConstraint = val as Web.Routing.ParameterBindingRouteConstraint;

               if (paramBindingConstraint != null) {
                  stringVal = String.Concat("<span class='keyword'>New</span> ", TypeReferenceVB(type), "(", ValueToVBString(paramBindingConstraint.Binder, constraint: true), ")");

               } else {
                  
                  var setConstraint = val as Web.Routing.SetRouteConstraint;

                  if (setConstraint != null) {
                     stringVal = String.Concat("<span class='keyword'>New</span> ", TypeReferenceVB(type), "(", String.Join(", ", setConstraint.GetValues().Select(s => ValueToVBString(s))), ")");

                  } else {
                     stringVal = String.Concat("<span class='keyword'>New</span> ", TypeReferenceVB(type), "()");
                  }
               }
            }

         } else {
            stringVal = val.ToString();
         }

         return stringVal;
      }

      static string TypeReferenceVB(Type type) {

         var sb = new StringBuilder();
         sb.AppendFormat("<span class='type' title='{0}'>", type.FullName);
         sb.Append((type.IsGenericType) ? type.Name.Substring(0, type.Name.IndexOf('`')) : type.Name);
         sb.Append("</span>");

         if (type.IsGenericType) {

            Type[] genericArgs = type.GetGenericArguments();

            sb.Append("(<span class='keyword'>Of</span> ");

            for (int i = 0; i < genericArgs.Length; i++) {

               if (i > 0) {
                  sb.Append(", ");
               }

               sb.Append(TypeReferenceVB(genericArgs[i]));
            }

            sb.Append(")");
         }

         return sb.ToString();
      }

      void RenderTopComments(string lineCommentChars) {

         Assembly thisAssembly = Assembly.GetExecutingAssembly();
         AssemblyName assemName = thisAssembly.GetName();

         string name = assemName.Name;
         string version = thisAssembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)
            .Cast<AssemblyFileVersionAttribute>()
            .Select(attr => attr.Version)
            .FirstOrDefault()
            ?? assemName.Version.ToString();

         writer.Write("<span class='comment'>");
         writer.Write(lineCommentChars);
         writer.WriteLine(" <a href='http://mvccoderouting.codeplex.com/'>{0}</a> v{1}", name, version);
         writer.Write(lineCommentChars);
         writer.Write(" Format: <a" + (this.format == "csharp" ? " class='self'" : " href='?format=csharp'"));
         writer.Write(">C#</a> - <a" + (this.format == "vb" ? " class='self'" : " href='?format=vb'"));
         writer.Write(">Visual Basic</a>");
         writer.Write("</span>");
      }

      void RenderHtmlHead() {

         writer.Write("<head>");
         writer.WriteLine("<style type='text/css'>");
         writer.WriteLine("body { white-space: pre; font-family: Consolas, 'Courier New'; font-size: 80%; }");
         writer.WriteLine(".comment, .comment a { color: #008000; }");
         writer.WriteLine(".string { color: #ac1414; }");
         writer.WriteLine(".keyword { color: #0026fd; }");
         writer.WriteLine(".type { color: #2b91af; }");
         writer.WriteLine("a.self { font-weight: bold; }");
         writer.WriteLine(".url { background-color: #ecf2f5; font-weight: bold; }");
         writer.Write("</style>");
         writer.Write("</head>");
      }

      static bool IsMvcHandler(Type handlerType) {
         return typeof(MvcRouteHandler).IsAssignableFrom(handlerType);
      }

      static bool IsMvcParameter(Type parameterType) {
         return parameterType == typeof(UrlParameter);
      }

      static bool IsWebApiHandler(Type handlerType) {

         while (handlerType != null) {

            if (handlerType.FullName == "System.Web.Http.WebHost.HttpControllerRouteHandler") {
               return true;
            }

            handlerType = handlerType.BaseType;
         }

         return false;
      }

      static bool IsWebApiParameter(Type parameterType) {
         return (parameterType.FullName == "System.Web.Http.RouteParameter");
      }
   }
}
