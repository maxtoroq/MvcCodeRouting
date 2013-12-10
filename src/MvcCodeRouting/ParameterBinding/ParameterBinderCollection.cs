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
   
   public class ParameterBinderCollection : KeyedCollection<Type, ParameterBinder> {

      protected override Type GetKeyForItem(ParameterBinder item) {
         return item.ParameterType;
      }

      public bool TryGetItem(Type key, out ParameterBinder item) {
         return this.Dictionary.TryGetValue(key, out item);
      }
   }
}
