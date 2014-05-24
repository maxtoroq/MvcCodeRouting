CodeRoutingExtensions.MapCodeRoutes Method (RouteCollection, Type, CodeRoutingSettings)
=======================================================================================
Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.

**Namespace:** [MvcCodeRouting][1]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public static ICollection<Route> MapCodeRoutes(
	this RouteCollection routes,
	Type rootController,
	CodeRoutingSettings settings
)
```

### Parameters

#### *routes*
Type: [System.Web.Routing.RouteCollection][2]  
A collection of routes for the application.

#### *rootController*
Type: [System.Type][3]  
The route controller for the application.

#### *settings*
Type: [MvcCodeRouting.CodeRoutingSettings][4]  
A settings object that customizes the route creation process. This parameter can be null.

### Return Value
Type: [ICollection][5]&lt;[Route][6]>  
The created routes.
### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type [RouteCollection][2]. When you use instance method syntax to call this method, omit the first parameter. For more information, see [Extension Methods (Visual Basic)][7] or [Extension Methods (C# Programming Guide)][8].

See Also
--------
[CodeRoutingExtensions Class][9]  
[MvcCodeRouting Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/cc680101
[3]: http://msdn.microsoft.com/en-us/library/42892f65
[4]: ../CodeRoutingSettings/README.md
[5]: http://msdn.microsoft.com/en-us/library/92t2ye13
[6]: http://msdn.microsoft.com/en-us/library/cc680015
[7]: http://msdn.microsoft.com/en-us/library/bb384936.aspx
[8]: http://msdn.microsoft.com/en-us/library/bb383977.aspx
[9]: README.md