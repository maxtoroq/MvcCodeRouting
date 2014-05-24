CodeRoutingHttpExtensions Class
===============================
Provides the extension methods to register and configure modules in a host ASP.NET Web API application.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  **MvcCodeRouting.CodeRoutingHttpExtensions**  

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public static class CodeRoutingHttpExtensions
```


Methods
-------

Name                                                                     | Description                                                                                                                                                                                                         
------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[MapCodeRoutes(HttpConfiguration, Type)][3]                              | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.                                                                        
[MapCodeRoutes(HttpConfiguration, String, Type)][4]                      | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly, and prepends the provided base route to the URL of each created route. 
[MapCodeRoutes(HttpConfiguration, Type, CodeRoutingSettings)][5]         | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.                                                                        
[MapCodeRoutes(HttpConfiguration, String, Type, CodeRoutingSettings)][6] | Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly, and prepends the provided base route to the URL of each created route. 


See Also
--------
[MvcCodeRouting Namespace][2]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: ../README.md
[3]: MapCodeRoutes_2.md
[4]: MapCodeRoutes.md
[5]: MapCodeRoutes_3.md
[6]: MapCodeRoutes_1.md