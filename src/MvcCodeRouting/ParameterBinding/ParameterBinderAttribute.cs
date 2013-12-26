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

namespace MvcCodeRouting.ParameterBinding {

   /// <summary>
   /// Represents an attribute that is used to associate a route parameter type
   /// to a <see cref="ParameterBinder"/> implementation that can parse it.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public sealed class ParameterBinderAttribute : Attribute {

      /// <summary>
      /// Gets the type of the binder.
      /// </summary>
      public Type BinderType { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="ParameterBinderAttribute"/> class,
      /// using the provided <paramref name="binderType"/>.
      /// </summary>
      /// <param name="binderType">The type of the binder.</param>
      public ParameterBinderAttribute(Type binderType) {

         if (binderType == null) throw new ArgumentNullException("binderType");

         this.BinderType = binderType;
      }
   }
}
