ParameterBinderAttribute Class
==============================
Represents an attribute that is used to associate a route parameter type to a [ParameterBinder][1] implementation that can parse it.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Attribute][3]  
    **MvcCodeRouting.ParameterBinding.ParameterBinderAttribute**  

**Namespace:** [MvcCodeRouting.ParameterBinding][4]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public sealed class ParameterBinderAttribute : Attribute
```

The **ParameterBinderAttribute** type exposes the following members.


Constructors
------------

Name                          | Description                                                                                            
----------------------------- | ------------------------------------------------------------------------------------------------------ 
[ParameterBinderAttribute][5] | Initializes a new instance of the **ParameterBinderAttribute** class, using the provided *binderType*. 


Properties
----------

Name            | Description                  
--------------- | ---------------------------- 
[BinderType][6] | Gets the type of the binder. 


See Also
--------
[MvcCodeRouting.ParameterBinding Namespace][4]  

[1]: ../ParameterBinder/README.md
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/e8kc3626
[4]: ../README.md
[5]: _ctor.md
[6]: BinderType.md