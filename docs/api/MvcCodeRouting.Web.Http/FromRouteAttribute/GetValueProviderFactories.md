FromRouteAttribute.GetValueProviderFactories Method
===================================================
Gets the value providers that will be fed to the model binder.

**Namespace:** [MvcCodeRouting.Web.Http][1]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public override IEnumerable<ValueProviderFactory> GetValueProviderFactories(
	HttpConfiguration configuration
)
```

### Parameters

#### *configuration*
Type: [System.Web.Http.HttpConfiguration][2]  
The [HttpConfiguration][2] configuration object.

### Return Value
Type: [IEnumerable][3]&lt;[ValueProviderFactory][4]>  
A collection of [ValueProviderFactory][4] instances.

See Also
--------
[FromRouteAttribute Class][5]  
[MvcCodeRouting.Web.Http Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/hh833997
[3]: http://msdn.microsoft.com/en-us/library/9eekhta0
[4]: http://msdn.microsoft.com/en-us/library/hh835931
[5]: README.md