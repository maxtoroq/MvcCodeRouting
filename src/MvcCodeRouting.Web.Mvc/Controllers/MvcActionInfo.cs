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
using System.Text;
using System.Web.Mvc;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting.Web.Mvc {
   
   abstract class MvcActionInfo : ActionInfo {

      public MvcActionInfo(ControllerInfo controller) 
         : base(controller) { }

      protected override string GetName() {

         ActionNameAttribute nameAttr = GetCustomAttributes(typeof(ActionNameAttribute), inherit: true)
            .Cast<ActionNameAttribute>()
            .SingleOrDefault();

         return (nameAttr != null) ?
            nameAttr.Name 
            : null;
      }
   }
}
