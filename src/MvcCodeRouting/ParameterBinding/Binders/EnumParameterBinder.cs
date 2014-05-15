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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcCodeRouting.ParameterBinding.Binders {
   
   /// <summary>
   /// Binds <see cref="Enum"/> route parameters.
   /// </summary>
   /// <typeparam name="TEnum">The enumeration type.</typeparam>
   public class EnumParameterBinder<TEnum> : ParameterBinder 
      where TEnum : struct {

      readonly HashSet<string> nameSet;

      /// <summary>
      /// Returns the <see cref="Type"/> for <typeparamref name="TEnum"/>.
      /// </summary>
      public override Type ParameterType {
         get { return typeof(TEnum); }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="EnumParameterBinder&lt;TEnum>"/> class.
      /// </summary>
      public EnumParameterBinder() {

         StringComparer comparer = StringComparer.OrdinalIgnoreCase;

         this.nameSet = new HashSet<string>(Enum.GetNames(typeof(TEnum)).Distinct(comparer), comparer);
      }

      /// <summary>
      /// Attempts to bind a route parameter.
      /// </summary>
      /// <param name="value">The value of the route parameter.</param>
      /// <param name="provider">The format provider to be used.</param>
      /// <param name="result">The bound value, an instance of <typeparamref name="TEnum"/>.</param>
      /// <returns>true if the parameter is successfully bound; else, false.</returns>
      public override bool TryBind(string value, IFormatProvider provider, out object result) {
         
         if (this.nameSet.Contains(value)) {
            result = Enum.Parse(typeof(TEnum), value, ignoreCase: true);
            return true;
         }

         result = null;

         return false;
      }
   }
}
