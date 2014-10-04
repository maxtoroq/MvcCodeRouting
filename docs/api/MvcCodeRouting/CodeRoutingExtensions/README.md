CodeRoutingExtensions Class
===========================
Provides the extension methods to register modules in a host ASP.NET MVC application.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  **MvcCodeRouting.CodeRoutingExtensions**  

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting.Web (in MvcCodeRouting.Web.dll)

Syntax
------

```csharp
public static class CodeRoutingExtensions
```


Methods
-------

Name                                                                   | Description                                                                                                                                                                                                         
---------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[MapCodeRoutes(RouteCollection, Type)][3]                              | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.                                                                        
[MapCodeRoutes(RouteCollection, String, Type)][4]                      | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly, and prepends the provided base route to the URL of each created route. 
[MapCodeRoutes(RouteCollection, Type, CodeRoutingSettings)][5]         | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.                                                                        
[MapCodeRoutes(RouteCollection, String, Type, CodeRoutingSettings)][6] | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly, and prepends the provided base route to the URL of each created route. 


See Also
--------
[MvcCodeRouting Namespace][2]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: ../README.md
[3]: MapCodeRoutes_2.md
[4]: MapCodeRoutes.md
[5]: MapCodeRoutes_3.md
[6]: MapCodeRoutes_1.md