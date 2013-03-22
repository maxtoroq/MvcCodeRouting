// Copyright 2013 Max Toro Q.
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
using System.Web.Mvc;

namespace MvcCodeRouting.Web.Mvc {
   
   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add token segments for each
   /// action method parameter after the {action} token, and for each controller property
   /// after the {controller} token.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public sealed class FromRouteAttribute : MvcCodeRouting.FromRouteAttribute {

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class.
      /// </summary>
      public FromRouteAttribute() 
         : base() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class 
      /// using the specified token name.
      /// </summary>
      /// <param name="tokenName">The token name.</param>
      public FromRouteAttribute(string tokenName) 
         : base(tokenName) { }
   }
}
