CodeRoutingSettings Class
=========================
Specifies a set of features that affect the way modules are exposed in a host application.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  **MvcCodeRouting.CodeRoutingSettings**  

**Namespace:** [MvcCodeRouting][2]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public class CodeRoutingSettings
```

The **CodeRoutingSettings** type exposes the following members.


Constructors
------------

Name                                          | Description                                                                                                            
--------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- 
[CodeRoutingSettings()][3]                    | Initializes a new instance of the **CodeRoutingSettings** class, using the values from the [Defaults][4] property.     
[CodeRoutingSettings(CodeRoutingSettings)][5] | Initializes a new instance of the **CodeRoutingSettings** class, using the values from the provided settings instance. 


Methods
-------

Name       | Description                                                                                                                                                  
---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ 
[Reset][6] | Resets the members of the settings class to their original default values, that is, the values from the [Defaults][4] property before any changes were made. 


Properties
----------

Name                     | Description                                                                                                                                                                                            
------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
[Configuration][7]       | Gets or sets an object that is associated to each created route as a data token named 'Configuration'. Use to provide configuration settings to controllers.                                           
[DefaultConstraints][8]  | Gets default constraints used for route parameters that represents action parameters and controller properties.                                                                                        
[Defaults][4]            | The settings that all new **CodeRoutingSettings** instances inherit. Use this property to affect the behavior of the `MapCodeRoutes` methods without having to pass a settings instance for each call. 
[EnableEmbeddedViews][9] | true to look for views embedded in assemblies.                                                                                                                                                         
[IgnoredControllers][10] | Gets a collection of controller types that must be ignored by the route creation process.                                                                                                              
[ParameterBinders][11]   | Gets the default binders used for route parameters that represent action parameters and controller properties.                                                                                         
[RootOnly][12]           | true to create routes for the root controller only.                                                                                                                                                    
[RouteFormatter][13]     | Gets or sets a delegate for custom route formatting.                                                                                                                                                   
[UseImplicitIdToken][14] | true to include an {id} route parameter for actions with a parameter named id.                                                                                                                         


See Also
--------
[MvcCodeRouting Namespace][2]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: ../README.md
[3]: _ctor.md
[4]: Defaults.md
[5]: _ctor_1.md
[6]: Reset.md
[7]: Configuration.md
[8]: DefaultConstraints.md
[9]: EnableEmbeddedViews.md
[10]: IgnoredControllers.md
[11]: ParameterBinders.md
[12]: RootOnly.md
[13]: RouteFormatter.md
[14]: UseImplicitIdToken.md