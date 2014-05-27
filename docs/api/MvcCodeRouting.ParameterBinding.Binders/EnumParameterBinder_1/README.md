EnumParameterBinder&lt;TEnum> Class
===================================
Binds [Enum][1] route parameters.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [MvcCodeRouting.ParameterBinding.ParameterBinder][3]  
    **MvcCodeRouting.ParameterBinding.Binders.EnumParameterBinder<TEnum>**  

**Namespace:** [MvcCodeRouting.ParameterBinding.Binders][4]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public class EnumParameterBinder<TEnum> : ParameterBinder 
where TEnum : struct, new()
```


Type Parameters
---------------

#### *TEnum*
The enumeration type.

The **EnumParameterBinder<TEnum>** type exposes the following members.


Constructors
------------

Name                               | Description                                                             
---------------------------------- | ----------------------------------------------------------------------- 
[EnumParameterBinder&lt;TEnum>][5] | Initializes a new instance of the **EnumParameterBinder<TEnum>** class. 


Methods
-------

Name         | Description                                                                                                    
------------ | -------------------------------------------------------------------------------------------------------------- 
[TryBind][6] | Attempts to bind a route parameter. (Overrides [ParameterBinder.TryBind(String, IFormatProvider, Object)][7].) 


Properties
----------

Name               | Description                                                                       
------------------ | --------------------------------------------------------------------------------- 
[ParameterType][8] | Returns the [Type][9] for TEnum. (Overrides [ParameterBinder.ParameterType][10].) 


See Also
--------
[MvcCodeRouting.ParameterBinding.Binders Namespace][4]  

[1]: http://msdn.microsoft.com/en-us/library/1zt1ybx4
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: ../../MvcCodeRouting.ParameterBinding/ParameterBinder/README.md
[4]: ../README.md
[5]: _ctor.md
[6]: TryBind.md
[7]: ../../MvcCodeRouting.ParameterBinding/ParameterBinder/TryBind.md
[8]: ParameterType.md
[9]: http://msdn.microsoft.com/en-us/library/42892f65
[10]: ../../MvcCodeRouting.ParameterBinding/ParameterBinder/ParameterType.md