MvcCodeRouting.Web.Mvc Namespace
================================
The MvcCodeRouting.Web.Mvc namespace contains attributes and extension methods for ASP.NET MVC controllers.


Classes
-------

Class                                | Description                                                                                                                                                                                                                                                                                                                                                                           
------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[CustomRouteAttribute][1]            | Represents an attribute that is used to customize the URL for the decorated action method or controller class.                                                                                                                                                                                                                                                                        
[DefaultActionAttribute][2]          | Represents an attribute that is used to specify which action method should be treated as default action, which means making the action segment of the URL optional.                                                                                                                                                                                                                   
[FromRouteAttribute][3]              | Represents an attribute that is used to mark action method parameters and controller properties, whose values must be bound using [RouteDataValueProvider][4]. It also instructs the route creation process to add route parameters after the {action} segment for each decorated action method parameter, and after the {controller} segment for each decorated controller property. 
[MvcExtensions][5]                   | Extensions methods that provide utility functions for various ASP.NET MVC classes.                                                                                                                                                                                                                                                                                                    
[RequireRouteParametersAttribute][6] | An [ActionMethodSelectorAttribute][7] for overloaded action methods, used to help the ASP.NET MVC runtime disambiguate and choose the appropriate overload.                                                                                                                                                                                                                           

[1]: CustomRouteAttribute/README.md
[2]: DefaultActionAttribute/README.md
[3]: FromRouteAttribute/README.md
[4]: http://msdn.microsoft.com/en-us/library/ee703614
[5]: MvcExtensions/README.md
[6]: RequireRouteParametersAttribute/README.md
[7]: http://msdn.microsoft.com/en-us/library/dd470807