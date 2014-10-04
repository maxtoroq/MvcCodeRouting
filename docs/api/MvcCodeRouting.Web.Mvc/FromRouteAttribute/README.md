FromRouteAttribute Class
========================
Represents an attribute that is used to mark action method parameters and controller properties, whose values must be bound using [RouteDataValueProvider][1]. It also instructs the route creation process to add route parameters after the {action} segment for each decorated action method parameter, and after the {controller} segment for each decorated controller property.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Attribute][3]  
    [System.Web.Mvc.CustomModelBinderAttribute][4]  
      **MvcCodeRouting.Web.Mvc.FromRouteAttribute**  

**Namespace:** [MvcCodeRouting.Web.Mvc][5]  
**Assembly:** MvcCodeRouting.Web.Mvc (in MvcCodeRouting.Web.Mvc.dll)

Syntax
------

```csharp
public sealed class FromRouteAttribute : CustomModelBinderAttribute, 
	IModelBinder, IFromRouteAttribute
```

The **FromRouteAttribute** type exposes the following members.


Constructors
------------

Name                            | Description                                                                              
------------------------------- | ---------------------------------------------------------------------------------------- 
[FromRouteAttribute()][6]       | Initializes a new instance of the **FromRouteAttribute** class.                          
[FromRouteAttribute(String)][7] | Initializes a new instance of the **FromRouteAttribute** class using the specified name. 


Methods
-------

Name           | Description                                                                                                                       
-------------- | --------------------------------------------------------------------------------------------------------------------------------- 
[BindModel][8] | Binds the decorated parameter or property to a value by using the specified controller context and binding context.               
[GetBinder][9] | Gets the model binder used to bind the decorated parameter or property. (Overrides [CustomModelBinderAttribute.GetBinder()][10].) 


Properties
----------

Name             | Description                                                                                                                           
---------------- | ------------------------------------------------------------------------------------------------------------------------------------- 
[BinderType][11] | Gets or sets the type of the binder.                                                                                                  
[CatchAll][12]   | true if the parameter represents a catch-all parameter; otherwise, false. This setting is ignored when used on controller properties. 
[Constraint][13] | Gets or sets a regular expression that specify valid values for the decorated parameter or property.                                  
[Name][14]       | Gets or sets the route parameter name. The default name used is the parameter or property name.                                       


See Also
--------
[MvcCodeRouting.Web.Mvc Namespace][5]  

[1]: http://msdn.microsoft.com/en-us/library/ee703614
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/e8kc3626
[4]: http://msdn.microsoft.com/en-us/library/dd492121
[5]: ../README.md
[6]: _ctor.md
[7]: _ctor_1.md
[8]: BindModel.md
[9]: GetBinder.md
[10]: http://msdn.microsoft.com/en-us/library/dd470595
[11]: BinderType.md
[12]: CatchAll.md
[13]: Constraint.md
[14]: Name.md