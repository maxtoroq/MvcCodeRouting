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

namespace MvcCodeRouting {

   public class RouteDebugHandler : IHttpHandler {

      public bool IsReusable { get { return false; } }

      public void ProcessRequest(HttpContext context) {

         var request = context.Request;
         var response = context.Response;

         if (!request.IsLocal)
            throw new HttpException(403, "Forbidden");

         string lang = request.QueryString["lang"];

         if (String.IsNullOrEmpty(lang))
            response.Redirect(request.Url.AbsolutePath + "?lang=csharp", endResponse: true);

         response.ContentType = "text/plain";

         switch (lang) {
            case "csharp":
               response.Write(RouteTable.Routes.ToCSharpMapRouteCalls());
               break;
            
            case "vb":
               response.Write(RouteTable.Routes.ToVBMapRouteCalls());
               break;

            default:
               break;
         }
      }
   }

   public static class RouteDebugExtensions {

      public static string ToCSharpMapRouteCalls(this RouteCollection routes) {

         if (routes == null) throw new ArgumentNullException("routes");

         StringBuilder sb = new StringBuilder();

         foreach (Route item in routes.OfType<Route>()) {

            string mapRoute = item.ToCSharpMapRouteCall();

            if (!String.IsNullOrEmpty(mapRoute)) {
               sb.Append(mapRoute)
                  .AppendLine()
                  .AppendLine();
            }
         }

         return sb.ToString();
      }

      private static string ToCSharpMapRouteCall(this Route route) {

         if (route == null) throw new ArgumentNullException("route");

         StringBuilder sb = new StringBuilder();

         Type handlerType = route.RouteHandler.GetType();

         if (typeof(StopRoutingHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.IgnoreRoute(\"{0}\");", route.Url);

         } else if (typeof(MvcRouteHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.MapRoute(null, \"{0}\"", route.Url);

            int i = 0;

            if (route.Defaults != null && route.Defaults.Count > 0) {

               sb.Append(", ")
                  .AppendLine()
                  .Append("    new { ");

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
                        .Append("    new { ");

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

            if (route.DataTokens != null && (namespaces = route.DataTokens["Namespaces"] as string[]) != null) {

               sb.Append(", ")
                  .AppendLine()
                  .Append("    new[] { ");

               for (int j = 0; j < namespaces.Length; j++) {
                  if (j > 0)
                     sb.Append(", ");
                  
                  sb.Append("\"")
                     .Append(namespaces[j])
                     .Append("\"");
               }

               sb.Append(" }");
            }

            sb.Append(");");
         }

         return sb.ToString();
      }

      private static string ValueToCSharpString(object val, bool constraint = false) {

         string stringVal;

         if (val == null)
            stringVal = "null";

         else if (val.GetType() == typeof(string))
            stringVal = String.Concat("@\"", val, "\"");

         else if (val.GetType() == typeof(UrlParameter))
            stringVal = "UrlParameter.Optional";

         else if (constraint)
            stringVal = String.Concat("new ", val.GetType().FullName, "()");
         
         else
            stringVal = val.ToString();

         return stringVal;
      }

      public static string ToVBMapRouteCalls(this RouteCollection routes) {

         if (routes == null) throw new ArgumentNullException("routes");

         StringBuilder sb = new StringBuilder();

         foreach (Route item in routes.OfType<Route>()) {

            string mapRoute = item.ToVBMapRouteCall();

            if (!String.IsNullOrEmpty(mapRoute)) {
               sb.Append(mapRoute)
                  .AppendLine()
                  .AppendLine();
            }
         }

         return sb.ToString();
      }

      private static string ToVBMapRouteCall(this Route route) {

         if (route == null) throw new ArgumentNullException("route");

         StringBuilder sb = new StringBuilder();

         Type handlerType = route.RouteHandler.GetType();

         if (typeof(StopRoutingHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.IgnoreRoute(\"{0}\")", route.Url);

         } else if (typeof(MvcRouteHandler).IsAssignableFrom(handlerType)) {

            sb.AppendFormat("routes.MapRoute(Nothing, \"{0}\"", route.Url);

            int i = 0;

            if (route.Defaults != null && route.Defaults.Count > 0) {

               sb.Append(", _")
                  .AppendLine()
                  .Append("    New With {");

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
                        .Append("    New With {");

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

            if (route.DataTokens != null && (namespaces = route.DataTokens["Namespaces"] as string[]) != null) {

               sb.Append(", _")
                  .AppendLine()
                  .Append("    New String() {");

               for (int j = 0; j < namespaces.Length; j++) {
                  if (j > 0)
                     sb.Append(", ");

                  sb.Append("\"")
                     .Append(namespaces[j])
                     .Append("\"");
               }

               sb.Append("}");
            }

            sb.Append(")");
         }

         return sb.ToString();
      }

      private static string ValueToVBString(object val, bool constraint = false) {

         string stringVal;

         if (val == null)
            stringVal = "Nothing";

         else if (val.GetType() == typeof(string))
            stringVal = String.Concat("\"", val, "\"");

         else if (val.GetType() == typeof(UrlParameter))
            stringVal = "UrlParameter.Optional";

         else if (constraint)
            stringVal = String.Concat("New ", val.GetType().FullName, "()");

         else
            stringVal = val.ToString();

         return stringVal;
      }
   }
}
