// Copyright 2012 Max Toro Q.
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
using System.Globalization;
using System.Linq;
using System.Text;

namespace MvcCodeRouting {

   static class FormatInvariantExtensions {

      public static string FormatInvariant(this string value, params object[] args) {
         return String.Format(CultureInfo.InvariantCulture, value, args);
      }

      public static StringBuilder AppendFormatInvariant(this StringBuilder value, string format, params object[] args) {
         return value.AppendFormat(CultureInfo.InvariantCulture, format, args);
      }

      public static string ToStringInvariant(this int value) {
         return value.ToString(CultureInfo.InvariantCulture);
      }

      public static string ToStringInvariant(this decimal value) {
         return value.ToString(CultureInfo.InvariantCulture);
      }

      public static string ToStringInvariant(this long value) {
         return value.ToString(CultureInfo.InvariantCulture);
      }

      public static string ToStringInvariant(this object value) {
         return Convert.ToString(value, CultureInfo.InvariantCulture);
      }
   }
}
