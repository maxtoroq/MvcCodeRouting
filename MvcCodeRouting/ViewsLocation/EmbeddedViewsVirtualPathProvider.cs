// Copyright 2011 Max Toro Q.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace MvcCodeRouting {

   sealed class EmbeddedViewsVirtualPathProvider : VirtualPathProvider {

      static readonly List<AssemblyResourceData> AssemblyDataTable = new List<AssemblyResourceData>();
      readonly ConcurrentDictionary<string, AssemblyResourceDataCollection> virtualPathCache = new ConcurrentDictionary<string, AssemblyResourceDataCollection>(VirtualPathComparison.Comparer);
      static bool embeddedViewsEnabled, registered;

      public static void RegisterAssembly(RegisterInfo registerInfo) {

         string basePath = String.Join("/", new[] { "Views", registerInfo.ViewsLocation }.Where(s => !String.IsNullOrEmpty(s)));
         var assemblyData = new AssemblyResourceData(registerInfo, basePath);

         if (assemblyData.HasResources) 
            AssemblyDataTable.Add(assemblyData);

         if (embeddedViewsEnabled && !registered)
            RegisterIfNecessary();
      }

      public static void RegisterIfNecessary() {

         embeddedViewsEnabled = true;

         if (AssemblyDataTable.Count > 0 && !registered) {
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedViewsVirtualPathProvider());
            registered = true;
         }
      }

      public override bool DirectoryExists(string virtualDir) {

         bool prevExists = base.DirectoryExists(virtualDir);

         if (prevExists)
            return prevExists;

         return GetAssemblyData(virtualDir).ResourceExists(virtualDir, isFile: false);
      }

      public override bool FileExists(string virtualPath) {
         
         bool prevExists = base.FileExists(virtualPath);

         if (prevExists)
            return prevExists;

         return GetAssemblyData(virtualPath).ResourceExists(virtualPath, isFile: true);
      }

      public override VirtualDirectory GetDirectory(string virtualDir) {

         VirtualDirectory prev = base.GetDirectory(virtualDir);

         string resourceName;
         AssemblyResourceData assemblyData;

         if (GetAssemblyData(virtualDir).ResourceExists(virtualDir, false, out resourceName, out assemblyData))
            return assemblyData.CreateVirtualDirectory(virtualDir, prev);

         return prev;
      }

      public override VirtualFile GetFile(string virtualPath) {

         bool prevExists = base.FileExists(virtualPath);

         if (prevExists)
            return base.GetFile(virtualPath);

         string resourceName;
         AssemblyResourceData assemblyData;

         if (GetAssemblyData(virtualPath).ResourceExists(virtualPath, true, out resourceName, out assemblyData))
            return assemblyData.CreateVirtualFile(virtualPath, resourceName);

         return null;
      }

      public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) {
         
         bool prevExists = base.FileExists(virtualPath);

         if (prevExists)
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);

         return null;
      }

      AssemblyResourceDataCollection GetAssemblyData(string virtualPath) {

         string appRelativePath = VirtualPathUtility.ToAppRelative(virtualPath);
         
         if (virtualPath.Length > 0 && virtualPath[0] == '~')
            virtualPath = VirtualPathUtility.ToAbsolute(appRelativePath);

         List<string> parts = appRelativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

         if (parts.Count < 1 || !parts[0].Equals("Views", VirtualPathComparison.Comparison))
            return AssemblyResourceDataCollection.Null;

         return this.virtualPathCache.GetOrAdd(virtualPath, (s) => {

            bool isFile = parts.Count > 1
               && virtualPath[virtualPath.Length - 1] != '/'
               && parts[parts.Count - 1].IndexOf('.') != -1;

            if (isFile) 
               parts.RemoveAt(parts.Count - 1);

            var result = new AssemblyResourceDataCollection();

            for (int i = 0; i < parts.Count; i++) {
               string basePath = String.Join("/", parts.Take(parts.Count - i));

               result.AddRange(AssemblyDataTable.Where(d => d.BasePath.Equals(basePath, VirtualPathComparison.Comparison)));

               if (result.Count > 0)
                  break;
            }

            return result;
         });
      }
   }
}