ParameterBinder.TryBind Method
==============================
Attempts to bind a route parameter.

**Namespace:** [MvcCodeRouting.ParameterBinding][1]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public abstract bool TryBind(
	string value,
	IFormatProvider provider,
	out Object result
)
```

### Parameters

#### *value*
Type: [System.String][2]  
The value of the route parameter.

#### *provider*
Type: [System.IFormatProvider][3]  
The format provider to be used.

#### *result*
Type: [System.Object][4]  
The bound value, an instance of the [Type][5] specified in [ParameterType][6].

### Return Value
Type: [Boolean][7]  
true if the parameter is successfully bound; else, false.

See Also
--------
[ParameterBinder Class][8]  
[MvcCodeRouting.ParameterBinding Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/s1wwdcbf
[3]: http://msdn.microsoft.com/en-us/library/efh2ww9y
[4]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[5]: http://msdn.microsoft.com/en-us/library/42892f65
[6]: ParameterType.md
[7]: http://msdn.microsoft.com/en-us/library/a28wyd50
[8]: README.md