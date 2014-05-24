FromRouteAttribute.BindModel Method
===================================
Binds the decorated parameter or property to a value by using the specified controller context and binding context.

**Namespace:** [MvcCodeRouting][1]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public Object BindModel(
	ControllerContext controllerContext,
	ModelBindingContext bindingContext
)
```

### Parameters

#### *controllerContext*
Type: [System.Web.Mvc.ControllerContext][2]  
The controller context.

#### *bindingContext*
Type: [System.Web.Mvc.ModelBindingContext][3]  
The binding context.

### Return Value
Type: [Object][4]  
The bound value.
### Implements
[IModelBinder.BindModel(ControllerContext, ModelBindingContext)][5]  


See Also
--------
[FromRouteAttribute Class][6]  
[MvcCodeRouting Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/dd492673
[3]: http://msdn.microsoft.com/en-us/library/dd492718
[4]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[5]: http://msdn.microsoft.com/en-us/library/dd505073
[6]: README.md