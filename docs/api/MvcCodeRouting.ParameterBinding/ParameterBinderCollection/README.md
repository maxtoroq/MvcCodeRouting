ParameterBinderCollection Class
===============================
Represents a collection of parameter binders. Each item in this collection must have a unique [ParameterType][1].


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Collections.ObjectModel.Collection][3]&lt;[ParameterBinder][4]>  
    [System.Collections.ObjectModel.KeyedCollection][5]&lt;[Type][6], [ParameterBinder][4]>  
      **MvcCodeRouting.ParameterBinding.ParameterBinderCollection**  

**Namespace:** [MvcCodeRouting.ParameterBinding][7]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public class ParameterBinderCollection : KeyedCollection<Type, ParameterBinder>
```

The **ParameterBinderCollection** type exposes the following members.


Constructors
------------

Name                           | Description                                                           
------------------------------ | --------------------------------------------------------------------- 
[ParameterBinderCollection][8] | Initializes a new instance of the **ParameterBinderCollection** class 


Methods
-------

Name               | Description                                                                                                         
------------------ | ------------------------------------------------------------------------------------------------------------------- 
[GetKeyForItem][9] | Extracts the key from the specified *item*. (Overrides [KeyedCollection&lt;TKey, TItem>.GetKeyForItem(TItem)][10].) 
[TryGetItem][11]   | Gets the item associated with the specified key.                                                                    


See Also
--------
[MvcCodeRouting.ParameterBinding Namespace][7]  

[1]: ../ParameterBinder/ParameterType.md
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/ms132397
[4]: ../ParameterBinder/README.md
[5]: http://msdn.microsoft.com/en-us/library/ms132438
[6]: http://msdn.microsoft.com/en-us/library/42892f65
[7]: ../README.md
[8]: _ctor.md
[9]: GetKeyForItem.md
[10]: http://msdn.microsoft.com/en-us/library/ms132454
[11]: TryGetItem.md