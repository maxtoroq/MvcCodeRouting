CustomRouteAttribute Class
==========================
Represents an attribute that is used to customize the URL for the decorated action method or controller class.


Inheritance Hierarchy
---------------------
[System.Object][1]  
  [System.Attribute][2]  
    **MvcCodeRouting.Web.Http.CustomRouteAttribute**  

**Namespace:** [MvcCodeRouting.Web.Http][3]  
**Assembly:** MvcCodeRouting.Web.Http (in MvcCodeRouting.Web.Http.dll)

Syntax
------

```csharp
public sealed class CustomRouteAttribute : Attribute, 
	ICustomRouteAttribute
```

The **CustomRouteAttribute** type exposes the following members.


Constructors
------------

Name                      | Description                                                                                       
------------------------- | ------------------------------------------------------------------------------------------------- 
[CustomRouteAttribute][4] | Initializes a new instance of the **CustomRouteAttribute** class, using the provided URL pattern. 


Properties
----------

Name     | Description      
-------- | ---------------- 
[Url][5] | The URL pattern. 


See Also
--------
[MvcCodeRouting.Web.Http Namespace][3]  

[1]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[2]: http://msdn.microsoft.com/en-us/library/e8kc3626
[3]: ../README.md
[4]: _ctor.md
[5]: Url.md