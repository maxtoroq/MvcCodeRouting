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
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace MvcCodeRouting.Web.Mvc {

#pragma warning disable 0618

   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add route parameters after the {action} segment 
   /// for each decorated action method parameter, and after the {controller} segment for each 
   /// decorated controller property.
   /// </summary>
   [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Want constructor argument shortcut.")]
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public sealed class FromRouteAttribute : MvcCodeRouting.FromRouteAttribute {

#pragma warning restore 0618

      /// <summary>
      /// Gets or sets the route parameter name. The default name used is the parameter or property name.
      /// </summary>
      public override sealed string Name {
         get { return base.Name; }
         set { base.Name = value; }
      }

      /// <summary>
      /// Gets or sets a regular expression that specify valid values for the decorated parameter or property.
      /// </summary>
      public override sealed string Constraint {
         get { return base.Constraint; }
         set { base.Constraint = value; }
      }

      /// <summary>
      /// true if the parameter represents a catch-all parameter; otherwise, false.
      /// This setting is ignored when used on controller properties.
      /// </summary>
      public override sealed bool CatchAll {
         get { return base.CatchAll; }
         set { base.CatchAll = value; }
      }

      /// <summary>
      /// Gets or sets the type of the binder.
      /// </summary>
      public override sealed Type BinderType {
         get { return base.BinderType; }
         set { base.BinderType = value; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class.
      /// </summary>
      public FromRouteAttribute() 
         : base() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class 
      /// using the specified name.
      /// </summary>
      /// <param name="name">The name of the route parameter.</param>
      public FromRouteAttribute(string name) 
         : base(name) { }
   }
}
