RequireRouteParametersAttribute Class
=====================================
An [ActionMethodSelectorAttribute][1] for overloaded action methods, used to help the ASP.NET MVC runtime disambiguate and choose the appropriate overload.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Attribute][3]  
    [System.Web.Mvc.ActionMethodSelectorAttribute][1]  
      **MvcCodeRouting.RequireRouteParametersAttribute**  
        [MvcCodeRouting.Web.Mvc.RequireRouteParametersAttribute][4]  

**Namespace:** [MvcCodeRouting][5]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
[ObsoleteAttribute("Please use MvcCodeRouting.Web.Mvc.RequireRouteParametersAttribute instead.")]
public class RequireRouteParametersAttribute : ActionMethodSelectorAttribute
```

The **RequireRouteParametersAttribute** type exposes the following members.


Constructors
------------

Name                                 | Description                                                                 
------------------------------------ | --------------------------------------------------------------------------- 
[RequireRouteParametersAttribute][6] | Initializes a new instance of the **RequireRouteParametersAttribute** class 


Methods
-------

Name                   | Description                                                                                                                                                                                    
---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[IsValidForRequest][7] | Determines whether the action method selection is valid for the specified controller context. (Overrides [ActionMethodSelectorAttribute.IsValidForRequest(ControllerContext, MethodInfo)][8].) 


See Also
--------
[MvcCodeRouting Namespace][5]  

[1]: http://msdn.microsoft.com/en-us/library/dd470807
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/e8kc3626
[4]: ../../MvcCodeRouting.Web.Mvc/RequireRouteParametersAttribute/README.md
[5]: ../README.md
[6]: _ctor.md
[7]: IsValidForRequest.md
[8]: http://msdn.microsoft.com/en-us/library/dd470593