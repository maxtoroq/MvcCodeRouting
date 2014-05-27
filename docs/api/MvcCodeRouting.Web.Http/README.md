MvcCodeRouting.Web.Http Namespace
=================================
The MvcCodeRouting.Web.Http namespace contains attributes and extension methods for ASP.NET Web API controllers.


Classes
-------

Class                     | Description                                                                                                                                                                                                                                                                                                                                                                           
------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[CustomRouteAttribute][1] | Represents an attribute that is used to customize the URL for the decorated action method or controller class.                                                                                                                                                                                                                                                                        
[FromRouteAttribute][2]   | Represents an attribute that is used to mark action method parameters and controller properties, whose values must be bound using [RouteDataValueProvider][3]. It also instructs the route creation process to add route parameters after the {action} segment for each decorated action method parameter, and after the {controller} segment for each decorated controller property. 
[HttpExtensions][4]       | Extensions methods that provide utility functions for various ASP.NET Web API classes.                                                                                                                                                                                                                                                                                                

[1]: CustomRouteAttribute/README.md
[2]: FromRouteAttribute/README.md
[3]: http://msdn.microsoft.com/en-us/library/hh834976
[4]: HttpExtensions/README.md