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
using System.Reflection;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Holds settings that customize the route creation process.
   /// </summary>
   public class CodeRoutingSettings {

      Func<RouteFormatterArgs, string> _RouteFormatter;
      readonly IDictionary<Type, string> _DefaultConstraints;
      readonly Collection<Type> _IgnoredControllers;

      /// <summary>
      /// Gets default constraints used for tokens that represents action parameters
      /// and controller properties.
      /// </summary>
      /// <remarks>
      /// This dictionary includes default values for <see cref="Boolean"/>, <see cref="Guid"/>,
      /// <see cref="Decimal"/>, <see cref="Double"/>, <see cref="Single"/>, <see cref="SByte"/>,
      /// <see cref="Int16"/>, <see cref="Int32"/>, <see cref="Int64"/>, <see cref="Byte"/>,
      /// <see cref="UInt16"/>, <see cref="UInt32"/> and <see cref="UInt64"/>.
      /// </remarks>
      public IDictionary<Type, string> DefaultConstraints {
         get { return _DefaultConstraints; }
      }

      /// <summary>
      /// Gets a collection of controller types that must be ignored by the route creation process.
      /// </summary>
      public Collection<Type> IgnoredControllers { 
         get { return _IgnoredControllers; } 
      }

      /// <summary>
      /// Gets or sets a delegate for custom route formatting.
      /// </summary>
      public Func<RouteFormatterArgs, string> RouteFormatter {
         get {
            if (_RouteFormatter == null)
               _RouteFormatter = (args) => args.OriginalSegment;
            return _RouteFormatter;
         }
         set { _RouteFormatter = value; }
      }

      /// <summary>
      /// true to look for views embedded in assemblies. View resources must
      /// be named using the following format: {rootNamespace}.Views.{controller}.{viewFilePath} .
      /// </summary>
      public bool EnableEmbeddedViews { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="CodeRoutingSettings"/> class.
      /// </summary>
      public CodeRoutingSettings() {

         _IgnoredControllers = new Collection<Type>();

         _DefaultConstraints = new Dictionary<Type, string>();
         _DefaultConstraints.Add(typeof(Boolean), "true|false");
         _DefaultConstraints.Add(typeof(Guid), @"\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Za-z0-9]{12}\b");

         foreach (var type in new[] { typeof(Decimal), typeof(Double), typeof(Single) })
            _DefaultConstraints.Add(type, @"-?(0|[1-9]\d*)(\.\d+)?");

         foreach (var type in new[] { typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64) })
            _DefaultConstraints.Add(type, @"0|-?[1-9]\d*");

         foreach (var type in new[] { typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64) })
            _DefaultConstraints.Add(type, @"0|[1-9]\d*");
      }

      internal string FormatRouteSegment(RouteFormatterArgs args, bool caseOnly) {

         string formattedSegment = this.RouteFormatter(args);

         if (caseOnly && !String.Equals(args.OriginalSegment, formattedSegment, StringComparison.OrdinalIgnoreCase)) { 
            
            throw new InvalidOperationException(
               String.Format(CultureInfo.InvariantCulture, 
                  "Only case formatting is currently supported for {0} route segments (Original segment: '{1}', formatted segment: '{2}').",
                  args.SegmentType,
                  args.OriginalSegment,
                  formattedSegment
               )
            );
         }

         return formattedSegment;
      }
   }
}
