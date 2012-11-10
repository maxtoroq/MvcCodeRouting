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
using System.Reflection;
using System.Text;

namespace MvcCodeRouting.Web.Hosting {

   class AssemblyResourcesCollection : Collection<AssemblyResources> {

      public static readonly AssemblyResourcesCollection Null = new AssemblyResourcesCollection();

      public void AddRange(IEnumerable<AssemblyResources> items) {

         foreach (var item in items)
            this.Add(item);
      }

      public bool ResourceExists(string virtualPath, bool isFile) {

         if (this.Count == 0)
            return false;

         string resourceName;
         AssemblyResources data;
         Assembly satelliteAssembly;

         return ResourceExists(virtualPath, isFile, out resourceName, out data, out satelliteAssembly);
      }

      public bool ResourceExists(string virtualPath, bool isFile, out string resourceName, out AssemblyResources data, out Assembly satelliteAssembly) {

         for (int i = 0; i < this.Count; i++) {

            data = this[i];

            if (data.ResourceExists(virtualPath, isFile, out resourceName, out satelliteAssembly))
               return true;
         }

         resourceName = null;
         data = null;
         satelliteAssembly = null;

         return false;
      }
   }
}
