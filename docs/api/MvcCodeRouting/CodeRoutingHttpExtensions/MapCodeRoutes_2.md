CodeRoutingHttpExtensions.MapCodeRoutes Method (HttpConfiguration, Type)
========================================================================
Creates routes for the specified root controller and all other controllers in the same namespace or any sub-namespace, in the same assembly.

**Namespace:** [MvcCodeRouting][1]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public static ICollection<IHttpRoute> MapCodeRoutes(
	this HttpConfiguration configuration,
	Type rootController
)
```

### Parameters

#### *configuration*
Type: [System.Web.Http.HttpConfiguration][2]  
The [HttpConfiguration][2] configuration object.

#### *rootController*
Type: [System.Type][3]  
The route controller for the application.

### Return Value
Type: [ICollection][4]&lt;[IHttpRoute][5]>  
The created routes.
### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type [HttpConfiguration][2]. When you use instance method syntax to call this method, omit the first parameter. For more information, see [Extension Methods (Visual Basic)][6] or [Extension Methods (C# Programming Guide)][7].

See Also
--------
[CodeRoutingHttpExtensions Class][8]  
[MvcCodeRouting Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/hh833997
[3]: http://msdn.microsoft.com/en-us/library/42892f65
[4]: http://msdn.microsoft.com/en-us/library/92t2ye13
[5]: http://msdn.microsoft.com/en-us/library/hh835899
[6]: http://msdn.microsoft.com/en-us/library/bb384936.aspx
[7]: http://msdn.microsoft.com/en-us/library/bb383977.aspx
[8]: README.md