CodeRoutingExtensions.BindRouteProperties Method
================================================
Binds controller properties decorated with [FromRouteAttribute][1] using the current route values.

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
[ObsoleteAttribute("Please use MvcCodeRouting.Web.Mvc.MvcExtensions.BindRouteProperties(ControllerBase) instead.")]
public static void BindRouteProperties(
	this ControllerBase controller
)
```

### Parameters

#### *controller*
Type: [System.Web.Mvc.ControllerBase][3]  
The controller to bind.

### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type [ControllerBase][3]. When you use instance method syntax to call this method, omit the first parameter. For more information, see [Extension Methods (Visual Basic)][4] or [Extension Methods (C# Programming Guide)][5].

Remarks
-------
You can call this method from [Initialize(RequestContext)][6].

See Also
--------
[CodeRoutingExtensions Class][7]  
[MvcCodeRouting Namespace][2]  

[1]: ../FromRouteAttribute/README.md
[2]: ../README.md
[3]: http://msdn.microsoft.com/en-us/library/dd504950
[4]: http://msdn.microsoft.com/en-us/library/bb384936.aspx
[5]: http://msdn.microsoft.com/en-us/library/bb383977.aspx
[6]: http://msdn.microsoft.com/en-us/library/dd470361
[7]: README.md