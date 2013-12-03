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
using System.Globalization;

namespace MvcCodeRouting.ParameterBinding.Binders {
   
   public class UInt32ParameterBinder : ParameterBinder {

      public UInt32ParameterBinder() 
         : base(typeof(uint)) { }

      public override bool TryBind(string value, IFormatProvider provider, out object result) {

         result = null;

         if (String.IsNullOrWhiteSpace(value))
            return false;

         const uint zero = 0;
         uint parsedResult;

         if (!UInt32.TryParse(value, NumberStyles.None, provider, out parsedResult))
            return false;

         // disallow leading sign or leading zero

         if (parsedResult == zero
            && value.Length != 1) {

            return false;
         }

         if (parsedResult > zero
            && (value[0] == '+' || value[0] == '0')) {

            return false;
         }

         result = parsedResult;

         return true;
      }
   }
}
