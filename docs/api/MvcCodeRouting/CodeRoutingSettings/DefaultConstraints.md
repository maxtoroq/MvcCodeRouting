CodeRoutingSettings.DefaultConstraints Property
===============================================
Gets default constraints used for route parameters that represents action parameters and controller properties.

**Namespace:** [MvcCodeRouting][1]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public IDictionary<Type, string> DefaultConstraints { get; }
```

### Property Value
Type: [IDictionary][2]&lt;[Type][3], [String][4]>

Remarks
-------
 Consider using [ParameterBinders][5] instead, for a more flexible constraining mechanism. 

See Also
--------
[CodeRoutingSettings Class][6]  
[MvcCodeRouting Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/s4ys34ea
[3]: http://msdn.microsoft.com/en-us/library/42892f65
[4]: http://msdn.microsoft.com/en-us/library/s1wwdcbf
[5]: ParameterBinders.md
[6]: README.md