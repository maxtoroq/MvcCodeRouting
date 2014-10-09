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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace MvcCodeRouting.Web.Hosting {

   class AssemblyResourceVirtualDirectory : VirtualDirectory {

      readonly VirtualDirectory prevDirectory;
      readonly AssemblyResources assemblyResources;

      List<VirtualFile> _Files;
      List<VirtualDirectory> _Directories;

      public override IEnumerable Children {
         get {
            return Directories.Cast<object>().Concat(Files.Cast<object>());
         }
      }

      public override IEnumerable Directories {
         get {
            return _Directories
               ?? (_Directories = this.prevDirectory.Directories.Cast<VirtualDirectory>().ToList());
         }
      }

      public override IEnumerable Files {
         get {
            if (_Files == null) {

               List<VirtualFile> prevFiles = this.prevDirectory.Files.Cast<VirtualFile>().ToList();

               string[] fileResources = this.assemblyResources.GetFileResources();

               for (int i = 0; i < fileResources.Length; i++) {

                  string resourceName = fileResources[i];
                  string virtualPath = VirtualPathUtility.ToAbsolute("~/" + this.assemblyResources.ResourceNameToRelativeVirtualPath(resourceName));

                  if (prevFiles.Exists(v => v.VirtualPath == virtualPath)) {
                     continue;
                  }

                  prevFiles.Add(this.assemblyResources.CreateVirtualFile(virtualPath, resourceName));
               }

               _Files = prevFiles;
            }

            return _Files;
         }
      }

      public AssemblyResourceVirtualDirectory(string virtualPath, VirtualDirectory prevDirectory, AssemblyResources assemblyData)
         : base(virtualPath) {

         this.prevDirectory = prevDirectory;
         this.assemblyResources = assemblyData;
      }
   }
}
