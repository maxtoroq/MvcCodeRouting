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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace MvcCodeRouting.Web.Hosting {

   class AssemblyResources {

      public readonly string BasePath;
      readonly string[] basePathParts;
      readonly string baseResourceName;

      readonly Assembly assembly;
      readonly string assemblyName;
      readonly int assemblyNamePartsCount;
      readonly string[] resourceNames;

      public bool HasResources { get { return resourceNames.Length > 0; } }

      public AssemblyResources(Assembly assembly, string basePath) {

         this.assembly = assembly;
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

      public bool FileResourceExists(string virtualPath) {

         string resourceName;
         Assembly satelliteAssembly;

         return FileResourceExists(virtualPath, out resourceName, out satelliteAssembly);
      }

      public bool FileResourceExists(string virtualPath, out string resourceName, out Assembly satelliteAssembly) {
         return ResourceExists(virtualPath, true, out resourceName, out satelliteAssembly);
      }

      public bool DirectoryResourceExists(string virtualPath) {

         string resourceName;
         Assembly satelliteAssembly;

         return ResourceExists(virtualPath, false, out resourceName, out satelliteAssembly);
      }

      bool ResourceExists(string virtualPath, bool isFile, out string resourceName, out Assembly satelliteAssembly) {

         if (this.HasResources) {

            string relativeVirtualPath = VirtualPathUtility.ToAppRelative(virtualPath).Remove(0, 2);
            List<string> relativeVirtualPathParts = relativeVirtualPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
               .ToList();

            const int minLocalizedFileParts = 3; // {name}.{culture}.{extension}
            List<string> fileParts;
            string cultureName;
            CultureInfo culture = null;

            if (isFile
               && (fileParts = relativeVirtualPathParts.Last().Split('.').ToList()).Count >= minLocalizedFileParts
               && (cultureName = Enumerable.Reverse(fileParts).Skip(1).First()).Length > 0) {

               try {
                  culture = new CultureInfo(cultureName);
               } catch (CultureNotFoundException) { }

               if (culture != null) {
                  
                  fileParts.RemoveAt(fileParts.Count - 2);
                  relativeVirtualPathParts[relativeVirtualPathParts.Count - 1] = String.Join(".", fileParts);
               }
            }

            for (int i = 0; i < resourceNames.Length; i++) {

               resourceName = resourceNames[i];
               string resourcePath = ResourceNameToRelativeVirtualPath(resourceName);
               string[] resourcePathParts = resourcePath.Split('/');

               bool resourcePathLengthOK = (isFile) ?
                  resourcePathParts.Length == relativeVirtualPathParts.Count
                  : resourcePathParts.Length > relativeVirtualPathParts.Count;

               if (resourcePathLengthOK) {

                  string resourcePath2 = (isFile) ?
                     resourcePath
                     : String.Join("/", resourcePathParts.Take(resourcePathParts.Length - 1));

                  if (String.Equals(String.Join("/", relativeVirtualPathParts), resourcePath2, VirtualPathComparison.Comparison)) {

                     if (isFile
                        && culture != null) {

                        try {
                           satelliteAssembly = this.assembly.GetSatelliteAssembly(culture);

                           if (satelliteAssembly.GetManifestResourceNames().Contains(resourceName))
                              return true;

                        } catch (FileNotFoundException) {
                        } catch (FileLoadException) { }

                     } else {
                        satelliteAssembly = null;
                        return true;
                     }
                  }
               }
            }
         }

         satelliteAssembly = null;
         resourceName = null;
         return false;
      }

      internal string ResourceNameToRelativeVirtualPath(string resourceName) {

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

      string RelativeVirtualPathToResourceName(string relativeVirtualPath) {

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

      public VirtualFile CreateVirtualFile(string virtualPath, string resourceName, Assembly satelliteAssembly = null) {
         return new AssemblyResourceVirtualFile(virtualPath, resourceName, satelliteAssembly ?? this.assembly);
      }

      public VirtualDirectory CreateVirtualDirectory(string virtualPath, VirtualDirectory prev) {
         return new AssemblyResourceVirtualDirectory(virtualPath, prev, this);
      }
   }
}
