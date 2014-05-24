RequireRouteParametersAttribute.IsValidForRequest Method
========================================================
Determines whether the action method selection is valid for the specified controller context.

**Namespace:** [MvcCodeRouting][1]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public override bool IsValidForRequest(
	ControllerContext controllerContext,
	MethodInfo methodInfo
)
```

### Parameters

#### *controllerContext*
Type: [System.Web.Mvc.ControllerContext][2]  
The controller context.

#### *methodInfo*
Type: [System.Reflection.MethodInfo][3]  
Information about the action method.

### Return Value
Type: [Boolean][4]  
 true if the [RouteData][5] has values for all parameters decorated with [FromRouteAttribute][6], and if all keys in [RouteData][5] match any of the decorated parameters, excluding controller, action and other route parameters that do not map to action method parameters. 

See Also
--------
[RequireRouteParametersAttribute Class][7]  
[MvcCodeRouting Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/dd492673
[3]: http://msdn.microsoft.com/en-us/library/1wa35kh5
[4]: http://msdn.microsoft.com/en-us/library/a28wyd50
[5]: http://msdn.microsoft.com/en-us/library/dd492908
[6]: ../FromRouteAttribute/README.md
[7]: README.md