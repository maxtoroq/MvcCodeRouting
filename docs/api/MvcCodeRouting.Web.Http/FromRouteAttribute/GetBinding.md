FromRouteAttribute.GetBinding Method
====================================
Gets the binding for a parameter.

**Namespace:** [MvcCodeRouting.Web.Http][1]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public override HttpParameterBinding GetBinding(
	HttpParameterDescriptor parameter
)
```

### Parameters

#### *parameter*
Type: [System.Web.Http.Controllers.HttpParameterDescriptor][2]  
The parameter to bind.

### Return Value
Type: [HttpParameterBinding][3]  
The [HttpParameterBinding][3] that contains the binding.

See Also
--------
[FromRouteAttribute Class][4]  
[MvcCodeRouting.Web.Http Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/hh834697
[3]: http://msdn.microsoft.com/en-us/library/hh944852
[4]: README.md