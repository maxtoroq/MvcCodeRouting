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
using System.ComponentModel;
using System.Linq;
using System.Text;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Represents an attribute that is used to customize the URL for the decorated
   /// action method or controller class.
   /// </summary>
   [Obsolete("Please use MvcCodeRouting.Web.Mvc.CustomRouteAttribute instead.")]
   [EditorBrowsable(EditorBrowsableState.Never)]
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
   public class CustomRouteAttribute : Attribute, ICustomRouteAttribute {

      readonly string _Url;

      /// <summary>
      /// The URL pattern.
      /// </summary>
      public virtual string Url { get { return _Url; } }

      /// <summary>
      /// Initializes a new instance of the <see cref="CustomRouteAttribute"/> class, 
      /// using the provided URL pattern.
      /// </summary>
      /// <param name="url">
      /// The URL pattern. Constraints can be specified using the <see cref="FromRouteAttribute"/>
      /// on the action method parameters or controller class properties.
      /// </param>
      public CustomRouteAttribute(string url) {

         if (!String.IsNullOrEmpty(url)
            && url[0] == '/') {

            throw new ArgumentException("Custom route cannot start with '/'.", "url");
         }

         this._Url = url;
      }
   }
}
