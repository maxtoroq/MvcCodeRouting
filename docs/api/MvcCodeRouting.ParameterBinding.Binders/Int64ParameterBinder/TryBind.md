Int64ParameterBinder.TryBind Method
===================================
Attempts to bind a route parameter.

**Namespace:** [MvcCodeRouting.ParameterBinding.Binders][1]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public override bool TryBind(
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
The bound value, an instance of [Int64][5].

### Return Value
Type: [Boolean][6]  
true if the parameter is successfully bound; else, false.

See Also
--------
[Int64ParameterBinder Class][7]  
[MvcCodeRouting.ParameterBinding.Binders Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/s1wwdcbf
[3]: http://msdn.microsoft.com/en-us/library/efh2ww9y
[4]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[5]: http://msdn.microsoft.com/en-us/library/6yy583ek
[6]: http://msdn.microsoft.com/en-us/library/a28wyd50
[7]: README.md