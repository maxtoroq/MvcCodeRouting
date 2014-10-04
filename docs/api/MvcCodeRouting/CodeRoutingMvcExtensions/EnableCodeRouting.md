CodeRoutingMvcExtensions.EnableCodeRouting Method (ControllerBuilder)
=====================================================================
Sets a custom [DefaultControllerFactory][1] implementation that provides a more direct access to the controller types for routes created by MvcCodeRouting. It enables a scenario where routes are created for controllers that are dynamically loaded at runtime.

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting.Web.Mvc (in MvcCodeRouting.Web.Mvc.dll)

Syntax
------

```csharp
public static void EnableCodeRouting(
	this ControllerBuilder controllerBuilder
)
```

### Parameters

#### *controllerBuilder*
Type: [System.Web.Mvc.ControllerBuilder][3]  
The controller builder.

### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type [ControllerBuilder][3]. When you use instance method syntax to call this method, omit the first parameter. For more information, see [Extension Methods (Visual Basic)][4] or [Extension Methods (C# Programming Guide)][5].

See Also
--------
[CodeRoutingMvcExtensions Class][6]  
[MvcCodeRouting Namespace][2]  

[1]: http://msdn.microsoft.com/en-us/library/dd470766
[2]: ../README.md
[3]: http://msdn.microsoft.com/en-us/library/dd460483
[4]: http://msdn.microsoft.com/en-us/library/bb384936.aspx
[5]: http://msdn.microsoft.com/en-us/library/bb383977.aspx
[6]: README.md