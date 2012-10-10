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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Routing;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Holds settings that customize the route creation process.
   /// </summary>
   public class CodeRoutingSettings {

      // IMPORTANT: When new properties are added Import and Reset must be updated.

      static readonly CodeRoutingSettings _Defaults = new CodeRoutingSettings(defaultsConstructor: true);

      readonly IDictionary<Type, string> _DefaultConstraints = new Dictionary<Type, string>();
      readonly Collection<Type> _IgnoredControllers = new Collection<Type>();
      readonly IDictionary<string, object> _Properties = new Dictionary<string, object>();
      Func<RouteFormatterArgs, string> _RouteFormatter;

      /// <summary>
      /// The settings that all new <see cref="CodeRoutingSettings"/> instances inherit.
      /// Use this property to affect the behavior of the <see cref="CodeRoutingExtensions.MapCodeRoutes(RouteCollection, Type)"/> 
      /// methods without having to pass a settings instance for each call.
      /// </summary>
      public static CodeRoutingSettings Defaults {
         get { return _Defaults; }
      }

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
      /// true to look for views embedded in assemblies.
      /// </summary>
      public bool EnableEmbeddedViews { get; set; }

      /// <summary>
      /// true to include an {id} token for actions with a parameter named id.
      /// </summary>
      public bool UseImplicitIdToken { get; set; }

      /// <summary>
      /// true to create routes for the root controller only.
      /// </summary>
      public bool RootOnly { get; set; }

      /// <summary>
      /// Gets or sets an object that is associated to each created route as a data token
      /// named 'Configuration'. Use to provide configuration settings to controllers.
      /// </summary>
      public object Configuration { get; set; }

      public IDictionary<string, object> Properties {
         get { return _Properties; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CodeRoutingSettings"/> class,
      /// using the values from the <see cref="CodeRoutingSettings.Defaults"/> property.
      /// </summary>
      public CodeRoutingSettings() 
         : this(_Defaults) { }

      /// <summary>
      /// Initializes a new instance of the <see cref="CodeRoutingSettings"/> class,
      /// using the values from the provided settings instance.
      /// </summary>
      /// <param name="settings">Another <see cref="CodeRoutingSettings"/> instance to copy the settings from.</param>
      public CodeRoutingSettings(CodeRoutingSettings settings) {
         
         if (settings == null) throw new ArgumentNullException("settings");

         Import(settings);
      }

      private CodeRoutingSettings(bool defaultsConstructor) {
         Reset();
      }

      void Import(CodeRoutingSettings settings) {

         this.EnableEmbeddedViews = settings.EnableEmbeddedViews;
         this.RootOnly = settings.RootOnly;
         this.RouteFormatter = settings.RouteFormatter;
         this.UseImplicitIdToken = settings.UseImplicitIdToken;
         this.Configuration = settings.Configuration;

         this.IgnoredControllers.Clear();

         foreach (Type item in settings.IgnoredControllers)
            this.IgnoredControllers.Add(item);

         this.DefaultConstraints.Clear();

         foreach (var item in settings.DefaultConstraints)
            this.DefaultConstraints.Add(item.Key, item.Value);

         this.Properties.Clear();

         foreach (var item in settings.Properties) 
            this.Properties.Add(item.Key, item.Value);
      }

      /// <summary>
      /// Resets the members of the settings class to their original default values, that is,
      /// the values from the <see cref="CodeRoutingSettings.Defaults"/> property before any
      /// changes were made.
      /// </summary>
      public void Reset() {

         this.EnableEmbeddedViews = false;
         this.RootOnly = false;
         this.RouteFormatter = null;
         this.UseImplicitIdToken = false;
         this.Configuration = null;

         this.Properties.Clear();

         this.IgnoredControllers.Clear();

         this.DefaultConstraints.Clear();
         this.DefaultConstraints.Add(typeof(Boolean), "true|false");
         this.DefaultConstraints.Add(typeof(Guid), @"\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Za-z0-9]{12}\b");

         foreach (Type type in new[] { typeof(Decimal), typeof(Double), typeof(Single) })
            this.DefaultConstraints.Add(type, @"-?(0|[1-9]\d*)(\.\d+)?");

         foreach (Type type in new[] { typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64) })
            this.DefaultConstraints.Add(type, @"0|-?[1-9]\d*");

         foreach (Type type in new[] { typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64) })
            this.DefaultConstraints.Add(type, @"0|[1-9]\d*");
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

      internal string GetConstraintForType(Type type, IFromRouteAttribute routeAttr) {

         bool isNullableValueType = type.IsGenericType 
            && type.GetGenericTypeDefinition() == typeof(Nullable<>);

         string constraint = null;

         if (routeAttr != null)
            constraint = routeAttr.Constraint;

         if (constraint == null) {
            type = (isNullableValueType) ? Nullable.GetUnderlyingType(type) : type;

            this.DefaultConstraints.TryGetValue(type, out constraint);

            if (constraint == null
               && type.IsEnum) {

               constraint = String.Join("|", Enum.GetNames(type));
            }
         }

         return constraint;
      }
   }
}
