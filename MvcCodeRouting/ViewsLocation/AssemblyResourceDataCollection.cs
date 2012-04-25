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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MvcCodeRouting {

   class AssemblyResourceDataCollection : Collection<AssemblyResourceData> {

      public static readonly AssemblyResourceDataCollection Null = new AssemblyResourceDataCollection();

      public void AddRange(IEnumerable<AssemblyResourceData> items) {

         foreach (var item in items)
            this.Add(item);
      }

      public bool ResourceExists(string virtualPath, bool isFile) {

         if (this.Count == 0)
            return false;

         string resourceName;
         AssemblyResourceData data;

         return ResourceExists(virtualPath, isFile, out resourceName, out data);
      }

      public bool ResourceExists(string virtualPath, bool isFile, out string resourceName, out AssemblyResourceData data) {

         for (int i = 0; i < this.Count; i++) {

            data = this[i];

            if (data.ResourceExists(virtualPath, isFile, out resourceName))
               return true;
         }

         resourceName = null;
         data = null;

         return false;
      }
   }
}
