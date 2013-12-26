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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.ParameterBinding {
   
   /// <summary>
   /// Parses route parameters to the type expected by the controller.
   /// </summary>
   /// <remarks>
   /// Implementations should be thread-safe.
   /// </remarks>
   public abstract class ParameterBinder {

      static readonly ConcurrentDictionary<Type, ParameterBinderAttribute> attrCache = new ConcurrentDictionary<Type, ParameterBinderAttribute>();

      /// <summary>
      /// The <see cref=" Type"/> of the instances that this binder creates.
      /// </summary>
      public abstract Type ParameterType { get; }

      internal static ParameterBinder GetInstance(Type modelType, Type binderType) {

         if (binderType != null
            && typeof(ParameterBinder).IsAssignableFrom(binderType)) {

            return CreateInstance(binderType);
         }

         if (modelType != null) {

            ParameterBinderAttribute attr = attrCache.GetOrAdd(modelType, t =>
               t.GetCustomAttributes(typeof(ParameterBinderAttribute), inherit: true)
                  .Cast<ParameterBinderAttribute>()
                  .SingleOrDefault());

            if (attr != null) {
               return CreateInstance(attr.BinderType);
            } 
         }

         return null;
      }

      static ParameterBinder CreateInstance(Type binderType) {

         try {
            return (ParameterBinder)Activator.CreateInstance(binderType);

         } catch (Exception ex) {
            throw new InvalidOperationException("An error occurred when trying to create the ParameterBinder '{0}'. Make sure that the binder has a public parameterless constructor.".FormatInvariant(binderType.FullName), ex);
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ParameterBinder"/> class.
      /// </summary>
      protected ParameterBinder() { }

      /// <summary>
      /// Attempts to bind a route parameter.
      /// </summary>
      /// <param name="value">The value of the route parameter.</param>
      /// <param name="provider">The format provider to be used.</param>
      /// <param name="result">The bound value, an instance of the <see cref="Type"/> specified in <see cref="ParameterType"/>.</param>
      /// <returns>true if the parameter is successfully bound; else, false.</returns>
      public abstract bool TryBind(string value, IFormatProvider provider, out object result);
   }
}
