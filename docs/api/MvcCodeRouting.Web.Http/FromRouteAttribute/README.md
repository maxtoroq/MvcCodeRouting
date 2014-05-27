FromRouteAttribute Class
========================
Represents an attribute that is used to mark action method parameters and controller properties, whose values must be bound using [RouteDataValueProvider][1]. It also instructs the route creation process to add route parameters after the {action} segment for each decorated action method parameter, and after the {controller} segment for each decorated controller property.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Attribute][3]  
    [System.Web.Http.ParameterBindingAttribute][4]  
      [System.Web.Http.ModelBinding.ModelBinderAttribute][5]  
        **MvcCodeRouting.Web.Http.FromRouteAttribute**  

**Namespace:** [MvcCodeRouting.Web.Http][6]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public sealed class FromRouteAttribute : ModelBinderAttribute, 
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

Name                            | Description                                                                                                                                         
------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- 
[BindModel][9]                  | Binds the model to a value by using the specified action context and binding context.                                                               
[GetBinding][10]                | Gets the binding for a parameter. (Overrides [ModelBinderAttribute.GetBinding(HttpParameterDescriptor)][11].)                                       
[GetValueProviderFactories][12] | Gets the value providers that will be fed to the model binder. (Overrides [ModelBinderAttribute.GetValueProviderFactories(HttpConfiguration)][13].) 


Properties
----------

Name             | Description                                                                                                                           
---------------- | ------------------------------------------------------------------------------------------------------------------------------------- 
[CatchAll][14]   | true if the parameter represents a catch-all parameter; otherwise, false. This setting is ignored when used on controller properties. 
[Constraint][15] | Gets or sets a regular expression that specify valid values for the decorated parameter or property.                                  


See Also
--------
[MvcCodeRouting.Web.Http Namespace][6]  

[1]: http://msdn.microsoft.com/en-us/library/hh834976
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/e8kc3626
[4]: http://msdn.microsoft.com/en-us/library/jj127376
[5]: http://msdn.microsoft.com/en-us/library/hh835545
[6]: ../README.md
[7]: _ctor.md
[8]: _ctor_1.md
[9]: BindModel.md
[10]: GetBinding.md
[11]: http://msdn.microsoft.com/en-us/library/jj127369
[12]: GetValueProviderFactories.md
[13]: http://msdn.microsoft.com/en-us/library/hh944649
[14]: CatchAll.md
[15]: Constraint.md