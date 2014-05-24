FromRouteAttribute Class
========================
Represents an attribute that is used to mark action method parameters and controller properties, whose values must be bound using [RouteDataValueProvider][1]. It also instructs the route creation process to add route parameters after the {action} segment for each decorated action method parameter, and after the {controller} segment for each decorated controller property.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Attribute][3]  
    [System.Web.Mvc.CustomModelBinderAttribute][4]  
      [MvcCodeRouting.FromRouteAttribute][5]  
        **MvcCodeRouting.Web.Mvc.FromRouteAttribute**  

**Namespace:** [MvcCodeRouting.Web.Mvc][6]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public sealed class FromRouteAttribute : FromRouteAttribute
```

The **FromRouteAttribute** type exposes the following members.


Constructors
------------

Name                            | Description                                                                              
------------------------------- | ---------------------------------------------------------------------------------------- 
[FromRouteAttribute()][7]       | Initializes a new instance of the **FromRouteAttribute** class.                          
[FromRouteAttribute(String)][8] | Initializes a new instance of the **FromRouteAttribute** class using the specified name. 


Methods
-------

Name            | Description                                                                                                                                                   
--------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[BindModel][9]  | Binds the decorated parameter or property to a value by using the specified controller context and binding context. (Inherited from [FromRouteAttribute][5].) 
[GetBinder][10] | Gets the model binder used to bind the decorated parameter or property. (Inherited from [FromRouteAttribute][5].)                                             


Properties
----------

Name             | Description                                                                                                                                                                          
---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
[BinderType][11] | Gets or sets the type of the binder. (Overrides [FromRouteAttribute.BinderType][12].)                                                                                                
[CatchAll][13]   | true if the parameter represents a catch-all parameter; otherwise, false. This setting is ignored when used on controller properties. (Overrides [FromRouteAttribute.CatchAll][14].) 
[Constraint][15] | Gets or sets a regular expression that specify valid values for the decorated parameter or property. (Overrides [FromRouteAttribute.Constraint][16].)                                
[Name][17]       | Gets or sets the route parameter name. The default name used is the parameter or property name. (Overrides [FromRouteAttribute.Name][18].)                                           
[TokenName][19]  | **Obsolete.** Gets or sets the route parameter name. The default name used is the parameter or property name. (Inherited from [FromRouteAttribute][5].)                              


See Also
--------
[MvcCodeRouting.Web.Mvc Namespace][6]  

[1]: http://msdn.microsoft.com/en-us/library/ee703614
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/e8kc3626
[4]: http://msdn.microsoft.com/en-us/library/dd492121
[5]: ../../MvcCodeRouting/FromRouteAttribute/README.md
[6]: ../README.md
[7]: _ctor.md
[8]: _ctor_1.md
[9]: ../../MvcCodeRouting/FromRouteAttribute/BindModel.md
[10]: ../../MvcCodeRouting/FromRouteAttribute/GetBinder.md
[11]: BinderType.md
[12]: ../../MvcCodeRouting/FromRouteAttribute/BinderType.md
[13]: CatchAll.md
[14]: ../../MvcCodeRouting/FromRouteAttribute/CatchAll.md
[15]: Constraint.md
[16]: ../../MvcCodeRouting/FromRouteAttribute/Constraint.md
[17]: Name.md
[18]: ../../MvcCodeRouting/FromRouteAttribute/Name.md
[19]: ../../MvcCodeRouting/FromRouteAttribute/TokenName.md