ParameterBinderCollection.TryGetItem Method
===========================================
Gets the item associated with the specified key.

**Namespace:** [MvcCodeRouting.ParameterBinding][1]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public bool TryGetItem(
	Type key,
	out ParameterBinder item
)
```

### Parameters

#### *key*
Type: [System.Type][2]  
The key whose item to get.

#### *item*
Type: [MvcCodeRouting.ParameterBinding.ParameterBinder][3]  
When this method returns, the item associated with the specified key, if the key is found; otherwise, the default value for the type of the *item* parameter. This parameter is passed uninitialized.

### Return Value
Type: [Boolean][4]  
true if an item with the specified key is found; otherwise, false.

See Also
--------
[ParameterBinderCollection Class][5]  
[MvcCodeRouting.ParameterBinding Namespace][1]  

[1]: ../README.md
[2]: http://msdn.microsoft.com/en-us/library/42892f65
[3]: ../ParameterBinder/README.md
[4]: http://msdn.microsoft.com/en-us/library/a28wyd50
[5]: README.md