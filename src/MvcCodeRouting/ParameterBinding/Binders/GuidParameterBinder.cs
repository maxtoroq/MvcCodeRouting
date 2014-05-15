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

namespace MvcCodeRouting.ParameterBinding.Binders {

   /// <summary>
   /// Binds <see cref="Guid"/> route parameters.
   /// </summary>
   public class GuidParameterBinder : ParameterBinder {

      /// <summary>
      /// Returns the <see cref="Type"/> for <see cref="Guid"/>.
      /// </summary>
      public override Type ParameterType {
         get { return typeof(Guid); }
      }

      /// <summary>
      /// Attempts to bind a route parameter.
      /// </summary>
      /// <param name="value">The value of the route parameter.</param>
      /// <param name="provider">The format provider to be used.</param>
      /// <param name="result">The bound value, an instance of <see cref="Guid"/>.</param>
      /// <returns>true if the parameter is successfully bound; else, false.</returns>
      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = null;

         if (String.IsNullOrWhiteSpace(value)) {
            return false;
         }

         if (value.Length < 36
            || value.Length > 38) {
            
            return false;
         }

         // TODO: Should only accept D form, but also accepting B and P for back compat

         Guid parsedResult;

#if NET35

         try {
            parsedResult = new Guid(value);
         
         } catch (FormatException) {
            
            return false;   
         }
#else

         if (!Guid.TryParse(value, out parsedResult)) {
            return false;
         }
#endif

         result = parsedResult;

         return true;
      }
   }
}
