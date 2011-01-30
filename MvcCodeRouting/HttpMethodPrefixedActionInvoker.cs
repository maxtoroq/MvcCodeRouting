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
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace MvcCodeRouting {
   
   public class HttpMethodPrefixedActionInvoker : ControllerActionInvoker {

      static readonly Regex WordBoundary = new Regex(@"(\B[A-Z])");

      public static string GetRealActionName(string actionMethodName) {

         if (actionMethodName == null) throw new ArgumentNullException("actionMethodName");

         return actionMethodName.Substring(WordBoundary.Match(actionMethodName).Index);
      }

      public static string GetHttpMethod(string actionMethodName) {
         
         if (actionMethodName == null) throw new ArgumentNullException("actionMethodName");

         return actionMethodName.Substring(0, WordBoundary.Match(actionMethodName).Index);
      }

      protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {

         string httpMethod = controllerContext.HttpContext.Request.GetHttpMethodOverride() ??
            controllerContext.HttpContext.Request.HttpMethod;

         // Implicit support for HEAD method. 
         // Decorate action with [HttpGet] if HEAD support is not wanted (e.g. action has side effects)

         if (String.Equals(httpMethod, WebRequestMethods.Http.Head, StringComparison.OrdinalIgnoreCase))
            httpMethod = WebRequestMethods.Http.Get;

         string httpMethodAndActionName = String.Concat(httpMethod, actionName);

         ActionDescriptor adescr = base.FindAction(controllerContext, controllerDescriptor, httpMethodAndActionName);

         if (adescr != null)
            adescr = new ActionDescriptorWrapper(adescr, actionName);

         return adescr;
      }

      class ActionDescriptorWrapper : ActionDescriptor {

         readonly ActionDescriptor wrapped;
         readonly string realActionName;

         public override string ActionName {
            get { return realActionName; }
         }

         public override ControllerDescriptor ControllerDescriptor {
            get { return wrapped.ControllerDescriptor; }
         }

         public override string UniqueId {
            get { return wrapped.UniqueId; }
         }

         public ActionDescriptorWrapper(ActionDescriptor wrapped, string realActionName) {
            
            this.wrapped = wrapped;
            this.realActionName = realActionName;
         }

         public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters) {
            return wrapped.Execute(controllerContext, parameters);
         }

         public override ParameterDescriptor[] GetParameters() {
            return wrapped.GetParameters();
         }

         public override object[] GetCustomAttributes(bool inherit) {
            return wrapped.GetCustomAttributes(inherit);
         }

         public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return wrapped.GetCustomAttributes(attributeType, inherit);
         }

         public override bool Equals(object obj) {
            return wrapped.Equals(obj);
         }

         public override int GetHashCode() {
            return wrapped.GetHashCode();
         }

         public override ICollection<ActionSelector> GetSelectors() {
            return wrapped.GetSelectors();
         }

         public override bool IsDefined(Type attributeType, bool inherit) {
            return wrapped.IsDefined(attributeType, inherit);
         }

         public override string ToString() {
            return wrapped.ToString();
         }
      }
   }
}
