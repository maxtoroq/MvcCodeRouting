FromRouteAttribute Class
========================
Represents an attribute that is used to mark action method parameters and controller properties, whose values must be bound using [RouteDataValueProvider][1]. It also instructs the route creation process to add route parameters after the {action} segment for each decorated action method parameter, and after the {controller} segment for each decorated controller property.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Attribute][3]  
    [System.Web.Mvc.CustomModelBinderAttribute][4]  
      **MvcCodeRouting.FromRouteAttribute**  
        [MvcCodeRouting.Web.Mvc.FromRouteAttribute][5]  

**Namespace:** [MvcCodeRouting][6]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
[ObsoleteAttribute("Please use MvcCodeRouting.Web.Mvc.FromRouteAttribute instead.")]
public class FromRouteAttribute : CustomModelBinderAttribute, 
	IModelBinder, IFromRouteAttribute
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
--------------- | --------------------------------------------------------------------------------------------------------------------------------- 
[BindModel][9]  | Binds the decorated parameter or property to a value by using the specified controller context and binding context.               
[GetBinder][10] | Gets the model binder used to bind the decorated parameter or property. (Overrides [CustomModelBinderAttribute.GetBinder()][11].) 


Properties
----------

Name             | Description                                                                                                                           
---------------- | ------------------------------------------------------------------------------------------------------------------------------------- 
[BinderType][12] | Gets or sets the type of the binder.                                                                                                  
[CatchAll][13]   | true if the parameter represents a catch-all parameter; otherwise, false. This setting is ignored when used on controller properties. 
[Constraint][14] | Gets or sets a regular expression that specify valid values for the decorated parameter or property.                                  
[Name][15]       | Gets or sets the route parameter name. The default name used is the parameter or property name.                                       
[TokenName][16]  | **Obsolete.** Gets or sets the route parameter name. The default name used is the parameter or property name.                         


See Also
--------
[MvcCodeRouting Namespace][6]  

[1]: http://msdn.microsoft.com/en-us/library/ee703614
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/e8kc3626
[4]: http://msdn.microsoft.com/en-us/library/dd492121
[5]: ../../MvcCodeRouting.Web.Mvc/FromRouteAttribute/README.md
[6]: ../README.md
[7]: _ctor.md
[8]: _ctor_1.md
[9]: BindModel.md
[10]: GetBinder.md
[11]: http://msdn.microsoft.com/en-us/library/dd470595
[12]: BinderType.md
[13]: CatchAll.md
[14]: Constraint.md
[15]: Name.md
[16]: TokenName.md