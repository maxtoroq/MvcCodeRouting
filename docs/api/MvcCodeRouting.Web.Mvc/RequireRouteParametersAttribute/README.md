RequireRouteParametersAttribute Class
=====================================
An [ActionMethodSelectorAttribute][1] for overloaded action methods, used to help the ASP.NET MVC runtime disambiguate and choose the appropriate overload.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  [System.Attribute][3]  
    [System.Web.Mvc.ActionMethodSelectorAttribute][1]  
      **MvcCodeRouting.Web.Mvc.RequireRouteParametersAttribute**  

**Namespace:** [MvcCodeRouting.Web.Mvc][4]  
**Assembly:** MvcCodeRouting.Web.Mvc (in MvcCodeRouting.Web.Mvc.dll)

Syntax
------

```csharp
public sealed class RequireRouteParametersAttribute : ActionMethodSelectorAttribute
```

The **RequireRouteParametersAttribute** type exposes the following members.


Constructors
------------

Name                                 | Description                                                                 
------------------------------------ | --------------------------------------------------------------------------- 
[RequireRouteParametersAttribute][5] | Initializes a new instance of the **RequireRouteParametersAttribute** class 


Methods
-------

Name                   | Description                                                                                                                                                                                    
---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
[IsValidForRequest][6] | Determines whether the action method selection is valid for the specified controller context. (Overrides [ActionMethodSelectorAttribute.IsValidForRequest(ControllerContext, MethodInfo)][7].) 


See Also
--------
[MvcCodeRouting.Web.Mvc Namespace][4]  

[1]: http://msdn.microsoft.com/en-us/library/dd470807
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: http://msdn.microsoft.com/en-us/library/e8kc3626
[4]: ../README.md
[5]: _ctor.md
[6]: IsValidForRequest.md
[7]: http://msdn.microsoft.com/en-us/library/dd470593