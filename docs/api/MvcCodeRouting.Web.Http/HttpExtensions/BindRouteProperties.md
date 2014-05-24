HttpExtensions.BindRouteProperties Method
=========================================
Binds controller properties decorated with [FromRouteAttribute][1] using the current route values.

**Namespace:** [MvcCodeRouting.Web.Http][2]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public static void BindRouteProperties(
	this ApiController controller
)
```

### Parameters

#### *controller*
Type: [System.Web.Http.ApiController][3]  
The controller to bind.

### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type [ApiController][3]. When you use instance method syntax to call this method, omit the first parameter. For more information, see [Extension Methods (Visual Basic)][4] or [Extension Methods (C# Programming Guide)][5].

Remarks
-------
You can call this method from [Initialize(HttpControllerContext)][6].

See Also
--------
[HttpExtensions Class][7]  
[MvcCodeRouting.Web.Http Namespace][2]  

[1]: ../FromRouteAttribute/README.md
[2]: ../README.md
[3]: http://msdn.microsoft.com/en-us/library/hh834453
[4]: http://msdn.microsoft.com/en-us/library/bb384936.aspx
[5]: http://msdn.microsoft.com/en-us/library/bb383977.aspx
[6]: http://msdn.microsoft.com/en-us/library/hh834971
[7]: README.md