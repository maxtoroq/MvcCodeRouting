CustomRouteAttribute Constructor
================================
Initializes a new instance of the [CustomRouteAttribute][1] class, using the provided URL pattern.

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public CustomRouteAttribute(
	string url
)
```

### Parameters

#### *url*
Type: [System.String][3]  
The URL pattern. Constraints can be specified using the [FromRouteAttribute][4] on the action method parameters or controller class properties.


See Also
--------
[CustomRouteAttribute Class][1]  
[MvcCodeRouting Namespace][2]  

[1]: README.md
[2]: ../README.md
[3]: http://msdn.microsoft.com/en-us/library/s1wwdcbf
[4]: ../FromRouteAttribute/README.md