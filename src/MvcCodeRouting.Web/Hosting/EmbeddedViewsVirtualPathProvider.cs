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
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace MvcCodeRouting.Web.Hosting {

   sealed class EmbeddedViewsVirtualPathProvider : VirtualPathProvider {

      static readonly List<AssemblyResources> ViewResources = new List<AssemblyResources>();
      static readonly AssemblyResources[] NullViewResources = new AssemblyResources[0];
      static bool embeddedViewsEnabled, registered;
      
      readonly ConcurrentDictionary<string, IList<AssemblyResources>> virtualPathCache = new ConcurrentDictionary<string, IList<AssemblyResources>>(VirtualPathComparison.Comparer);

      public static void RegisterAssembly(RegisterSettings registerSettings) {

         string basePath = String.Join("/", new[] { "Views", registerSettings.ViewsLocation }.Where(s => !String.IsNullOrEmpty(s)));
         var assemblyResources = new AssemblyResources(registerSettings.Assembly, basePath);

         if (assemblyResources.HasResources) {
            ViewResources.Add(assemblyResources);
         }

         if (embeddedViewsEnabled 
            && !registered) {

            RegisterIfNecessary();
         }
      }

      public static void RegisterIfNecessary() {

         embeddedViewsEnabled = true;

         if (ViewResources.Count > 0 
            && !registered) {

            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedViewsVirtualPathProvider());
            registered = true;
         }
      }

      public override bool DirectoryExists(string virtualDir) {

         bool prevExists = base.DirectoryExists(virtualDir);

         if (prevExists) {
            return prevExists;
         }

         return GetAssemblyMatches(virtualDir)
            .Any(r => r.DirectoryResourceExists(virtualDir));
      }

      public override bool FileExists(string virtualPath) {
         
         bool prevExists = base.FileExists(virtualPath);

         if (prevExists) {
            return prevExists;
         }

         return GetAssemblyMatches(virtualPath)
            .Any(r => r.FileResourceExists(virtualPath));
      }

      public override VirtualDirectory GetDirectory(string virtualDir) {

         VirtualDirectory prev = base.GetDirectory(virtualDir);

         AssemblyResources firstMatch = GetAssemblyMatches(virtualDir)
            .FirstOrDefault(r => r.DirectoryResourceExists(virtualDir));

         if (firstMatch != null) {
            return firstMatch.CreateVirtualDirectory(virtualDir, prev);
         }

         return prev;
      }

      public override VirtualFile GetFile(string virtualPath) {

         bool prevExists = base.FileExists(virtualPath);

         if (prevExists) {
            return base.GetFile(virtualPath);
         }

         string resourceName = null;
         Assembly satelliteAssembly = null;

         AssemblyResources firstMatch = GetAssemblyMatches(virtualPath)
            .FirstOrDefault(r => r.FileResourceExists(virtualPath, out resourceName, out satelliteAssembly));

         if (firstMatch != null) {
            return firstMatch.CreateVirtualFile(virtualPath, resourceName, satelliteAssembly);
         }

         return null;
      }

      public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) {
         
         bool prevExists = base.FileExists(virtualPath);

         if (prevExists) {
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
         }

         return null;
      }

      IEnumerable<AssemblyResources> GetAssemblyMatches(string virtualPath) {

         string appRelativePath = VirtualPathUtility.ToAppRelative(virtualPath);

         if (virtualPath.Length > 0 
            && virtualPath[0] == '~') {

            virtualPath = VirtualPathUtility.ToAbsolute(appRelativePath);
         }

         List<string> parts = appRelativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

         if (parts.Count < 1 
            || !parts[0].Equals("Views", VirtualPathComparison.Comparison)) {

            return NullViewResources;
         }
         
         return this.virtualPathCache.GetOrAdd(virtualPath, (s) => {

            bool isFile = parts.Count > 1
               && virtualPath[virtualPath.Length - 1] != '/'
               && parts[parts.Count - 1].IndexOf('.') != -1;

            if (isFile) {
               parts.RemoveAt(parts.Count - 1);
            }

            for (int i = 0; i < parts.Count; i++) {

               string basePath = String.Join("/", parts.Take(parts.Count - i));

               AssemblyResources[] matches = ViewResources
                  .Where(d => d.BasePath.Equals(basePath, VirtualPathComparison.Comparison))
                  .ToArray();

               if (matches.Length > 0) {
                  return matches;
               }
            }

            return NullViewResources;
         });
      }
   }
}