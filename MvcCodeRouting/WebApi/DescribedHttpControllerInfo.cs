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
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MvcCodeRouting.WebApi {

   class DescribedHttpControllerInfo : ControllerInfo {

      readonly HttpControllerDescriptor controllerDescr;

      public override string Name {
         get {
            return controllerDescr.ControllerName;
         }
      }

      public DescribedHttpControllerInfo(HttpControllerDescriptor controllerDescr, Type type, RegisterInfo registerInfo) 
         : base(type, registerInfo) {

         this.controllerDescr = controllerDescr;
      }

      protected internal override ActionInfo[] GetActions() {

         return this.controllerDescr.Configuration.Services
            .GetActionSelector()
            .GetActionMapping(this.controllerDescr)
            .SelectMany(x => x)
            .Select(a => new DescribedHttpActionInfo(a, this))
            .ToArray();
      }

      public override object[] GetCustomAttributes(bool inherit) {
         return this.Type.GetCustomAttributes(inherit);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
         return this.Type.GetCustomAttributes(attributeType, inherit);
      }

      public override bool IsDefined(Type attributeType, bool inherit) {
         return this.Type.IsDefined(attributeType, inherit);
      }
   }
}
