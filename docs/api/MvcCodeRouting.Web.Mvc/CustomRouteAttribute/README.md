CustomRouteAttribute Class
==========================
Represents an attribute that is used to customize the URL for the decorated action method or controller class.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  [System.Attribute][2]  
    [MvcCodeRouting.CustomRouteAttribute][3]  
      **MvcCodeRouting.Web.Mvc.CustomRouteAttribute**  

**Namespace:** [MvcCodeRouting.Web.Mvc][4]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public sealed class CustomRouteAttribute : CustomRouteAttribute
```

The **CustomRouteAttribute** type exposes the following members.


Constructors
------------

Name                      | Description                                                                                       
------------------------- | ------------------------------------------------------------------------------------------------- 
[CustomRouteAttribute][5] | Initializes a new instance of the **CustomRouteAttribute** class, using the provided URL pattern. 


Properties
----------

Name     | Description                                                 
-------- | ----------------------------------------------------------- 
[Url][6] | The URL pattern. (Overrides [CustomRouteAttribute.Url][7].) 


See Also
--------
[MvcCodeRouting.Web.Mvc Namespace][4]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: http://msdn.microsoft.com/en-us/library/e8kc3626
[3]: ../../MvcCodeRouting/CustomRouteAttribute/README.md
[4]: ../README.md
[5]: _ctor.md
[6]: Url.md
[7]: ../../MvcCodeRouting/CustomRouteAttribute/Url.md