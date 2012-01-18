﻿// Copyright 2011 Max Toro Q.
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
using System.Linq;
using System.Text;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Represents an attribute that is used to customize the URL for the decorated
   /// action method.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method)]
   public sealed class CustomRouteAttribute : Attribute {

      /// <summary>
      /// The URL pattern.
      /// </summary>
      public string Url { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="CustomRouteAttribute"/> class, 
      /// using the provided URL pattern.
      /// </summary>
      /// <param name="url">
      /// The URL pattern. Constraints can be specified using the <see cref="FromRouteAttribute"/>
      /// on the action method parameters.
      /// </param>
      public CustomRouteAttribute(string url) {
         this.Url = url;
      }
   }
}