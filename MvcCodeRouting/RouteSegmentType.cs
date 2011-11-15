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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Represents the mapping source of a route segment.
   /// </summary>
   public enum RouteSegmentType {
      
      /// <summary>
      /// The segment maps to a namespace segment.
      /// </summary>
      Namespace,
      
      /// <summary>
      /// The segment maps to a controller name.
      /// </summary>
      Controller,
      
      /// <summary>
      /// The segment maps to an action name.
      /// </summary>
      Action
   }
}
