MvcCodeRouting.ParameterBinding Namespace
=========================================
The MvcCodeRouting.ParameterBinding namespace contains classes that customize how route parameters are bound.


Classes
-------

Class                          | Description                                                                                                                          
------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------ 
[ParameterBinder][1]           | Parses route parameters to the type expected by the controller.                                                                      
[ParameterBinderAttribute][2]  | Represents an attribute that is used to associate a route parameter type to a [ParameterBinder][1] implementation that can parse it. 
[ParameterBinderCollection][3] | Represents a collection of parameter binders. Each item in this collection must have a unique [ParameterType][4].                    

[1]: ParameterBinder/README.md
[2]: ParameterBinderAttribute/README.md
[3]: ParameterBinderCollection/README.md
[4]: ParameterBinder/ParameterType.md