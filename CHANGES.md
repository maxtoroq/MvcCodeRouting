Changes
=======

Web.Http.WebHost.v1.0.0
-----------------------
First MvcCodeRouting.Web.Http.WebHost release

Web.Http.v1.0.0
---------------
- Requires MvcCodeRouting v1.2
- Moved WebHost to separate assembly/package

v1.2.0
------
- Parameter Binding API
- Using regions in routes.axd to group routes by route context

Web.Http.v0.10.0
----------------  
- Implemented BindRouteProperties
- Fixed [#1152](https://mvccoderouting.codeplex.com/workitem/1152): Both traditional and verb-based routing cannot be used in the same ApiController
- Using Guid as route name in self host

v1.1.1
------
Fixed [#1155](https://mvccoderouting.codeplex.com/workitem/1155): FromRoute.BinderType ignored by BindRouteProperties

Web.Http.v0.9.3
---------------
- Requires MvcCodeRouting v1.1.0
- Fix: Web.Http.FromRoute attribute does not bind when using a custom name
- Changed Web.Http MapCodeRoutes to extend HttpConfiguration instead of HttpRouteCollection. Calling CodeRoutingSettings.HttpConfiguration() is no longer needed, that method is now internal

v1.1.0
------
- Copied attributes to Web.Mvc subnamespace, deprecated old attributes although they still work
- Custom default actions using DefaultAction attribute
- BinderType property on FromRoute attribute
- Base route relative (aka absolute) custom route support for controllers
- Highlight URL on routes.axd
- Display assembly file version on routes.axd

Web.Http.v0.9.2
---------------
No binary changes, updated MvcCodeRouting dependency version range

v1.0.2
------
- Support for loading views embedded in satellite assemblies
- Fixed [#1148](https://mvccoderouting.codeplex.com/workitem/1148): RouteDebugHandler fails if RouteTable has Route with null DataTokens

Web.Http.v0.9.1
---------------
First MvcCodeRouting.Web.Http CTP release

v1.0.1
------
- URL generation performance improvement

v1.0.0
------
- .NET 3.5 support (compiled against MVC 2)
- Fixed [#1147](https://mvccoderouting.codeplex.com/workitem/1147): Generic methods should not be considered actions

Special thanks to [tylerburd](http://www.codeplex.com/site/users/view/tylerburd), [Grenaderov](http://www.codeplex.com/site/users/view/Grenaderov) and [JoshuaGough](http://www.codeplex.com/site/users/view/JoshuaGough) for their invaluable feedback

v0.9.8
------
- Async controller support
- MVC 4 Release Candidate support
- Added Configuration setting
- Fixed [#950](https://mvccoderouting.codeplex.com/workitem/950): Visual Studio build fails after using Extract-Views command
- Absolute custom route support
- CustomRoute attribute support for controllers

v0.9.7
------
- Fixed [#890](https://mvccoderouting.codeplex.com/workitem/890): Embedded views don't work when assembly has multipart name
- Added CodeRoutingSettings.Defaults property
- Added CodeRoutingSettings.Reset() method
- Added Extract-Views powershell command, use from Package Manager Console   

v0.9.6
------
- Fixed [#783](https://mvccoderouting.codeplex.com/workitem/783): Default action with optional route parameters does not work
- Fixed [#779](https://mvccoderouting.codeplex.com/workitem/779): Allow multiple actions with same custom route if {action} token is present
- Added RootOnly setting
- Removed limitation that required ViewEngineCollection.EnableCodeRouting() to be called after RouteCollection.MapCodeRoutes(), when using embedded views. Now both methods can be called in any order

v0.9.5
------
- Fixed [#708](https://mvccoderouting.codeplex.com/workitem/708): Optional string parameter with a null default value does not create an optional token
- Fixed [#744](https://mvccoderouting.codeplex.com/workitem/744): Create only one route for multiple actions with equal custom routes
- Fixed [#746](https://mvccoderouting.codeplex.com/workitem/746): UseImplicitIdToken is not copied to new CodeRoutingSettings instance
- Fixed [#747](https://mvccoderouting.codeplex.com/workitem/747): IgnoredControllers is wrapped by new CodeRoutingSettings instance instead of copying its items
- Automatic constraints for enum parameters and properties, using Enum.GetNames(Type)
- Custom model binder support for parameters and properties decorated with FromRouteAttribute

v0.9.4
------
- Added CodeRoutingSettings constructor that takes another CodeRoutingSettings instance to copy the settings from
- Added TokenName property to FromRouteAttribute
- CodeRoutingConstraint parameter name renamed to __routecontext
- Ambiguous route check now done on actions just created and not all registered actions. This allows you to override routes
- Added UseImplicitIdToken setting, useful for existing applications that only use a generic {controller}/{action}/{id} route
- Case-only constraint on action segment formatting removed, now SomeAction can be formatted as Some-Action, or some_action, etc

BREAKING CHANGES:
- ~controller syntax now matches routes in the same baseRoute

v0.9.3
------
- Fixed two bugs
- Changed RouteFormatter delegate
- Removed case-only constraint on namespace and controller segment formatting (e.g. SomeLongNamespace/SomeLongController can be formatted as some-long-namespace/some-long-controller)
- CodeRoutingConstraint hidden from RouteDebugHandler
- Added CustomRouteAttribute for custom routes
- Support for embedded views (as assembly resources), enable with CodeRoutingSettings.EnableEmbeddedViews 

v0.9.2
------
- Support for ControllerDescriptor (via Controller.CreateActionInvoker)
- Added complete XML documentation comments

BREAKING CHANGES:
- Removed ActionNameExtractor from CodeRoutingSettings, action names can be customized using a custom ControllerDescriptor
- Removed DefaultAction from CodeRoutingSettings (Index is the convention)
- Removed rootNamespace and assembly parameters, and settings.RootController property, and replaced with rootController. The API and the implementation are now greatly simplified thanks to this change. I strongly believe this is the definitive API

v0.9.1
------
- Fixed 2 issues
- New hierarchical (a.k.a. RESTful) route support (see BookController in samples project)

v0.9.0
------
- Resolved 3 issues
- Renamed MapCodeRoutes parameter baseController to rootController
- MapCodeRoutes now returns collection of created routes
- Added MapCodeRoutes baseRoute parameter
- Color formatting in RouteDebugHandler
- Added IgnoredControllers setting
- Added RouteSegmentType parameter to RouteFormatter setting
- Assembly now has strong name

v0.8.3
------
- Fixed one bug
- Implemented baseRoute-aware URL generation
- Added RouteDebugHandler

v0.8.2
------
- Fixed two issues

v0.8.1.487
----------
- Fixed one bug

v0.8.0.369
----------
First release