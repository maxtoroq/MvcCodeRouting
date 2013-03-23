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
using System.Web.Mvc;

namespace MvcCodeRouting.Web.Mvc {

#pragma warning disable 0618

   /// <summary>
   /// An <see cref="ActionMethodSelectorAttribute"/> for overloaded action methods, used 
   /// to help the ASP.NET MVC runtime disambiguate and choose the appropriate overload.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method)]
   public sealed class RequireRouteParametersAttribute : MvcCodeRouting.RequireRouteParametersAttribute { }

#pragma warning restore 0618

}
