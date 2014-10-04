CodeRoutingMvcExtensions Class
==============================
Provides the extension methods to configure an ASP.NET MVC application.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  **MvcCodeRouting.CodeRoutingMvcExtensions**  

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting.Web.Mvc (in MvcCodeRouting.Web.Mvc.dll)

Syntax
------

```csharp
public static class CodeRoutingMvcExtensions
```


Methods
-------

Name                                         | Description                                                                                                                                                                                                                                                        
-------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
[EnableCodeRouting(ControllerBuilder)][3]    | Sets a custom [DefaultControllerFactory][4] implementation that provides a more direct access to the controller types for routes created by MvcCodeRouting. It enables a scenario where routes are created for controllers that are dynamically loaded at runtime. 
[EnableCodeRouting(ViewEngineCollection)][5] | Enables namespace-aware views location. Always call after you are done adding view engines.                                                                                                                                                                        


See Also
--------
[MvcCodeRouting Namespace][2]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: ../README.md
[3]: EnableCodeRouting.md
[4]: http://msdn.microsoft.com/en-us/library/dd470766
[5]: EnableCodeRouting_1.md