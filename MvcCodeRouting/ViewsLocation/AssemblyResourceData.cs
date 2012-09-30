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
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace MvcCodeRouting.ViewsLocation {

   class AssemblyResourceData {

      public readonly string BasePath;
      readonly string[] basePathParts;
      readonly string baseResourceName;

      readonly Assembly assembly;
      readonly string assemblyName;
      readonly int assemblyNamePartsCount;
      readonly string[] resourceNames;

      public bool HasResources { get { return resourceNames.Length > 0; } }

      public AssemblyResourceData(RegisterSettings registerSettings, string basePath) {

         this.assembly = registerSettings.Assembly;
         this.assemblyName = this.assembly.GetName().Name;
         this.assemblyNamePartsCount = this.assemblyName.Split('.').Count();
         this.BasePath = basePath;
         this.basePathParts = basePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
         this.baseResourceName = RelativeVirtualPathToResourceName(basePath);
         this.resourceNames =
            (from n in assembly.GetManifestResourceNames()
             where n.StartsWith(this.baseResourceName + ".", StringComparison.Ordinal)
                && n.Split('.').Length >= 4
             select n).ToArray();
      }

      public bool ResourceExists(string virtualPath, bool isFile, out string resourceName) {

         if (this.HasResources) {

            string relativeVirtualPath = VirtualPathUtility.ToAppRelative(virtualPath).Remove(0, 2);
            string[] relativeVirtualPathParts = relativeVirtualPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < resourceNames.Length; i++) {

               resourceName = resourceNames[i];
               string resourcePath = ResourceNameToRelativeVirtualPath(resourceName);
               string[] resourcePathParts = resourcePath.Split('/');

               bool resourcePathLengthOK = (isFile) ?
                  resourcePathParts.Length == relativeVirtualPathParts.Length
                  : resourcePathParts.Length > relativeVirtualPathParts.Length;

               if (resourcePathLengthOK) {

                  string resourcePath2 = (isFile) ?
                     resourcePath
                     : String.Join("/", resourcePathParts.Take(resourcePathParts.Length - 1));

                  if (String.Equals(String.Join("/", relativeVirtualPathParts), resourcePath2, VirtualPathComparison.Comparison))
                     return true;
               }
            }
         }

         resourceName = null;
         return false;
      }

      public string ResourceNameToRelativeVirtualPath(string resourceName) {

         int dotFirstIndex = resourceName.IndexOf('.');
         int dotLastIndex = resourceName.LastIndexOf('.');

         string virtualPath = resourceName;

         if (dotFirstIndex > 0
            && dotFirstIndex != dotLastIndex) {

            StringBuilder sb = new StringBuilder(resourceName);
            sb.Replace('.', '/', 0, dotLastIndex);

            virtualPath = sb.ToString();
         }

         List<string> parts = virtualPath.Split('/').ToList();
         parts.RemoveRange(0, this.assemblyNamePartsCount);
         parts.InsertRange(1, this.basePathParts.Skip(1));

         return String.Join("/", parts);
      }

      public string RelativeVirtualPathToResourceName(string relativeVirtualPath) {

         List<string> parts = relativeVirtualPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();

         if (parts.Count > 1)
            parts.RemoveRange(1, basePathParts.Length - 1);

         parts.Insert(0, this.assemblyName);

         return String.Join(".", parts);
      }

      public string[] GetFileResources() {

         string[] baseNameParts = this.baseResourceName.Split('.');

         return
            (from resourceName in resourceNames
             let resourceParts = resourceName.Split('.')
             where resourceParts.Length == baseNameParts.Length + 2
                && String.Equals(String.Join(".", resourceParts.Take(baseNameParts.Length)), this.baseResourceName, StringComparison.Ordinal)
             select resourceName).ToArray();
      }

      public VirtualFile CreateVirtualFile(string virtualPath, string resourceName) {
         return new AssemblyResourceVirtualFile(virtualPath, resourceName, this.assembly);
      }

      public VirtualDirectory CreateVirtualDirectory(string virtualPath, VirtualDirectory prev) {
         return new AssemblyResourceVirtualDirectory(virtualPath, prev, this);
      }
   }
}
