RouteDebugHandler Class
=======================
Serves representations of the routes in [Routes][1] for visualization and debugging purposes.


Inheritance Hierarchy
---------------------
[System.Object][2]  
  **MvcCodeRouting.RouteDebugHandler**  

**Namespace:** [MvcCodeRouting][3]  
**Assembly:** MvcCodeRouting (in MvcCodeRouting.dll)

Syntax
------

```csharp
public class RouteDebugHandler : IHttpHandler
```

The **RouteDebugHandler** type exposes the following members.


Constructors
------------

Name                   | Description                                                   
---------------------- | ------------------------------------------------------------- 
[RouteDebugHandler][4] | Initializes a new instance of the **RouteDebugHandler** class 


Methods
-------

Name                | Description                                          
------------------- | ---------------------------------------------------- 
[ProcessRequest][5] | Serves representations of the routes in [Routes][1]. 


Properties
----------

Name               | Description                                                                                    
------------------ | ---------------------------------------------------------------------------------------------- 
[DefaultFormat][6] | Gets or sets the default format that the handler should use. Valid values are: "csharp", "vb". 
[IsReusable][7]    | Gets a value indicating whether another request can use the [IHttpHandler][8] instance.        


See Also
--------
[MvcCodeRouting Namespace][3]  

[1]: http://msdn.microsoft.com/en-us/library/cc679803
[2]: http://msdn.microsoft.com/en-us/library/e5kfa45b
[3]: ../README.md
[4]: _ctor.md
[5]: ProcessRequest.md
[6]: DefaultFormat.md
[7]: IsReusable.md
[8]: http://msdn.microsoft.com/en-us/library/7ezc17x8