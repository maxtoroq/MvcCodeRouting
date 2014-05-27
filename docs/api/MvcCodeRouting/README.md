MvcCodeRouting Namespace
========================
The MvcCodeRouting namespace contains extension methods and classes used to register and configure modules in a host application.


Classes
-------

Class                                | Description                                                                                                                                                                                                                                                                                                                                                                                         
------------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[CodeRoutingExtensions][1]           | Provides the extension methods to register and configure modules in a host ASP.NET MVC application.                                                                                                                                                                                                                                                                                                 
[CodeRoutingHttpExtensions][2]       | Provides the extension methods to register and configure modules in a host ASP.NET Web API application.                                                                                                                                                                                                                                                                                             
[CodeRoutingSettings][3]             | Specifies a set of features that affect the way modules are exposed in a host application.                                                                                                                                                                                                                                                                                                          
[CustomRouteAttribute][4]            | **Obsolete.** Represents an attribute that is used to customize the URL for the decorated action method or controller class.                                                                                                                                                                                                                                                                        
[FromRouteAttribute][5]              | **Obsolete.** Represents an attribute that is used to mark action method parameters and controller properties, whose values must be bound using [RouteDataValueProvider][6]. It also instructs the route creation process to add route parameters after the {action} segment for each decorated action method parameter, and after the {controller} segment for each decorated controller property. 
[RequireRouteParametersAttribute][7] | **Obsolete.** An [ActionMethodSelectorAttribute][8] for overloaded action methods, used to help the ASP.NET MVC runtime disambiguate and choose the appropriate overload.                                                                                                                                                                                                                           
[RouteDebugHandler][9]               | Serves representations of the routes in [Routes][10] for visualization and debugging purposes.                                                                                                                                                                                                                                                                                                      
[RouteFormatterArgs][11]             | Provides data for custom route formatting.                                                                                                                                                                                                                                                                                                                                                          


Enumerations
------------

Enumeration            | Description                                       
---------------------- | ------------------------------------------------- 
[RouteSegmentType][12] | Represents the mapping source of a route segment. 

[1]: CodeRoutingExtensions/README.md
[2]: CodeRoutingHttpExtensions/README.md
[3]: CodeRoutingSettings/README.md
[4]: CustomRouteAttribute/README.md
[5]: FromRouteAttribute/README.md
[6]: http://msdn.microsoft.com/en-us/library/ee703614
[7]: RequireRouteParametersAttribute/README.md
[8]: http://msdn.microsoft.com/en-us/library/dd470807
[9]: RouteDebugHandler/README.md
[10]: http://msdn.microsoft.com/en-us/library/cc679803
[11]: RouteFormatterArgs/README.md
[12]: RouteSegmentType/README.md