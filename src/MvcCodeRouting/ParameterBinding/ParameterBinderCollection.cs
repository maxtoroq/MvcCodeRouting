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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MvcCodeRouting.ParameterBinding {
   
   /// <summary>
   /// Represents a collection of parameter binders. Each item in this collection
   /// must have a unique <see cref="ParameterBinder.ParameterType"/>.
   /// </summary>
   public class ParameterBinderCollection : KeyedCollection<Type, ParameterBinder> {

      /// <summary>
      /// Extracts the key from the specified <paramref name="item"/>.
      /// </summary>
      /// <param name="item">The item from which to extract the key.</param>
      /// <returns>The key for the specified item.</returns>
      protected override Type GetKeyForItem(ParameterBinder item) {
         return item.ParameterType;
      }

      /// <summary>
      /// Gets the item associated with the specified key.
      /// </summary>
      /// <param name="key">The key whose item to get.</param>
      /// <param name="item">
      /// When this method returns, the item associated with the specified key, if
      /// the key is found; otherwise, the default value for the type of the <paramref name="item"/>
      /// parameter. This parameter is passed uninitialized.
      /// </param>
      /// <returns>true if an item with the specified key is found; otherwise, false.</returns>
      public bool TryGetItem(Type key, out ParameterBinder item) {
         return this.Dictionary.TryGetValue(key, out item);
      }
   }
}
