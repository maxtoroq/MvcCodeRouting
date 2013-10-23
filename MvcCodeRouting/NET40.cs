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
using System.Linq;
using System.Text;

namespace MvcCodeRouting {

   static class String {

      public static string Concat(object str1, object str2, object str3) {
         return string.Concat(str1, str2, str3);
      }

      public static string Concat(string str1, string str2, string str3) {
         return string.Concat(str1, str2, str3);
      }

      public static string Concat(params string[] values) {
         return string.Concat(values);
      }

      public static bool Equals(string str1, string str2, StringComparison comparison) {
         return string.Equals(str1, str2, comparison);
      }

      public static bool IsNullOrEmpty(string val) { 
         return string.IsNullOrEmpty(val);
      }

      public static bool IsNullOrWhiteSpace(string val) {

         if (val == null) return true;
         return val.Trim().Length == 0;
      }

      public static string Format(IFormatProvider provider, string format, params object[] args) {
         return string.Format(provider, format, args);
      }

      public static string Join(string separator, string[] values) {
         return string.Join(separator, values);
      }

      public static string Join(string separator, IEnumerable<string> values) {
         return string.Join(separator, values.ToArray());
      }
   }

   static class StringBuilderExtensions {

      public static StringBuilder Clear(this StringBuilder sb) {
         sb.Length = 0;
         return sb;
      }
   }
}

namespace System.Collections.Concurrent {
   
   class ConcurrentDictionary<TKey, TValue> {

      readonly Dictionary<TKey, TValue> dictionary;
      readonly object thisLock = new object();

      public ConcurrentDictionary() 
         : this(EqualityComparer<TKey>.Default) { }

      public ConcurrentDictionary(IEqualityComparer<TKey> comparer) {
         this.dictionary = new Dictionary<TKey, TValue>(comparer);
      }

      public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory) {
         
         TValue value;

         lock (this.thisLock) {

            if (!this.dictionary.TryGetValue(key, out value)) {

               value = valueFactory(key);
               this.dictionary[key] = value;
            }

            return value;
         }
      }
   }
}
