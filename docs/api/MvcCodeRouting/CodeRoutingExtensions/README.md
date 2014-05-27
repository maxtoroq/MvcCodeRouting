CodeRoutingExtensions Class
===========================
Provides the extension methods to register and configure modules in a host ASP.NET MVC application.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  **MvcCodeRouting.CodeRoutingExtensions**  

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public static class CodeRoutingExtensions
```

The **CodeRoutingExtensions** type exposes the following members.


Methods
-------

Name                                                                   | Description                                                                                                                                                                                                         
---------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[BindRouteProperties][3]                                               | **Obsolete.** Binds controller properties decorated with [FromRouteAttribute][4] using the current route values.                                                                                                    
[EnableCodeRouting][5]                                                 | Enables namespace-aware views location. Always call after you are done adding view engines.                                                                                                                         
[MapCodeRoutes(RouteCollection, Type)][6]                              | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.                                                                        
[MapCodeRoutes(RouteCollection, String, Type)][7]                      | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly, and prepends the provided base route to the URL of each created route. 
[MapCodeRoutes(RouteCollection, Type, CodeRoutingSettings)][8]         | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.                                                                        
[MapCodeRoutes(RouteCollection, String, Type, CodeRoutingSettings)][9] | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly, and prepends the provided base route to the URL of each created route. 


See Also
--------
[MvcCodeRouting Namespace][2]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: ../README.md
[3]: BindRouteProperties.md
[4]: ../FromRouteAttribute/README.md
[5]: EnableCodeRouting.md
[6]: MapCodeRoutes_2.md
[7]: MapCodeRoutes.md
[8]: MapCodeRoutes_3.md
[9]: MapCodeRoutes_1.md