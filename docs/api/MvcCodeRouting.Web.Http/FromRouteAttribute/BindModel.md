FromRouteAttribute.BindModel Method
===================================
Binds the model to a value by using the specified action context and binding context.

**Namespace:** [MvcCodeRouting.Web.Http][1]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public bool BindModel(
	HttpActionContext actionContext,
	ModelBindingContext bindingContext
)
```

### Parameters

#### *actionContext*
Type: [System.Web.Http.Controllers.HttpActionContext][2]  
The action context.

#### *bindingContext*
Type: [System.Web.Http.ModelBinding.ModelBindingContext][3]  
The binding context.

### Return Value
Type: [Boolean][4]  
true if the model is successfully bound; otherwise, false.
### Implements
[IModelBinder.BindModel(HttpActionContext, ModelBindingContext)][5]  


See Also
--------
[FromRouteAttribute Class][6]  
[MvcCodeRouting.Web.Http Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/hh834934
[3]: http://msdn.microsoft.com/en-us/library/hh835191
[4]: http://msdn.microsoft.com/en-us/library/a28wyd50
[5]: http://msdn.microsoft.com/en-us/library/hh834117
[6]: README.md