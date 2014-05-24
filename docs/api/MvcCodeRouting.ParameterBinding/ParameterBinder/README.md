ParameterBinder Class
=====================
Parses route parameters to the type expected by the controller.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  **MvcCodeRouting.ParameterBinding.ParameterBinder**  
    [MvcCodeRouting.ParameterBinding.Binders.BooleanParameterBinder][2]  
    [MvcCodeRouting.ParameterBinding.Binders.ByteParameterBinder][3]  
    [MvcCodeRouting.ParameterBinding.Binders.DecimalParameterBinder][4]  
    [MvcCodeRouting.ParameterBinding.Binders.DoubleParameterBinder][5]  
    [MvcCodeRouting.ParameterBinding.Binders.EnumParameterBinder&lt;TEnum>][6]  
    [MvcCodeRouting.ParameterBinding.Binders.GuidParameterBinder][7]  
    [MvcCodeRouting.ParameterBinding.Binders.Int16ParameterBinder][8]  
    [MvcCodeRouting.ParameterBinding.Binders.Int32ParameterBinder][9]  
    [MvcCodeRouting.ParameterBinding.Binders.Int64ParameterBinder][10]  
    [MvcCodeRouting.ParameterBinding.Binders.SByteParameterBinder][11]  
    [MvcCodeRouting.ParameterBinding.Binders.SingleParameterBinder][12]  
    [MvcCodeRouting.ParameterBinding.Binders.UInt16ParameterBinder][13]  
    [MvcCodeRouting.ParameterBinding.Binders.UInt32ParameterBinder][14]  
    [MvcCodeRouting.ParameterBinding.Binders.UInt64ParameterBinder][15]  

**Namespace:** [MvcCodeRouting.ParameterBinding][16]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public abstract class ParameterBinder
```

The **ParameterBinder** type exposes the following members.


Constructors
------------

Name                  | Description                                                  
--------------------- | ------------------------------------------------------------ 
[ParameterBinder][17] | Initializes a new instance of the **ParameterBinder** class. 


Methods
-------

Name          | Description                         
------------- | ----------------------------------- 
[TryBind][18] | Attempts to bind a route parameter. 


Properties
----------

Name                | Description                                               
------------------- | --------------------------------------------------------- 
[ParameterType][19] | The [Type][20] of the instances that this binder creates. 


Remarks
-------
 Implementations should be thread-safe. 

See Also
--------
[MvcCodeRouting.ParameterBinding Namespace][16]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: ../../MvcCodeRouting.ParameterBinding.Binders/BooleanParameterBinder/README.md
[3]: ../../MvcCodeRouting.ParameterBinding.Binders/ByteParameterBinder/README.md
[4]: ../../MvcCodeRouting.ParameterBinding.Binders/DecimalParameterBinder/README.md
[5]: ../../MvcCodeRouting.ParameterBinding.Binders/DoubleParameterBinder/README.md
[6]: ../../MvcCodeRouting.ParameterBinding.Binders/EnumParameterBinder_1/README.md
[7]: ../../MvcCodeRouting.ParameterBinding.Binders/GuidParameterBinder/README.md
[8]: ../../MvcCodeRouting.ParameterBinding.Binders/Int16ParameterBinder/README.md
[9]: ../../MvcCodeRouting.ParameterBinding.Binders/Int32ParameterBinder/README.md
[10]: ../../MvcCodeRouting.ParameterBinding.Binders/Int64ParameterBinder/README.md
[11]: ../../MvcCodeRouting.ParameterBinding.Binders/SByteParameterBinder/README.md
[12]: ../../MvcCodeRouting.ParameterBinding.Binders/SingleParameterBinder/README.md
[13]: ../../MvcCodeRouting.ParameterBinding.Binders/UInt16ParameterBinder/README.md
[14]: ../../MvcCodeRouting.ParameterBinding.Binders/UInt32ParameterBinder/README.md
[15]: ../../MvcCodeRouting.ParameterBinding.Binders/UInt64ParameterBinder/README.md
[16]: ../README.md
[17]: _ctor.md
[18]: TryBind.md
[19]: ParameterType.md
[20]: http://msdn.microsoft.com/en-us/library/42892f65