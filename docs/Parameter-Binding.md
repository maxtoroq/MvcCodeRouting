Parameter Binding
=================

Motivation
----------
Route parameter constraining and binding are two separate mechanisms. First, Regex constraints are executed when routing tries to find a route that matches. Second, before action invocation, Model Binding converts the route parameter (usually a string) to the appropriate type. Unless there's a custom model binder configured, this usually falls back to [System.Convert][1].

This is not desirable for a number of reasons, specially when dealing with non-string parameters:

- A Regex constraint might not be 100% accurate and could match values that would later be rejected by Model Binding. There's clearly no need for two parsing mechanisms.
- Regex constrains are also executed on URL generation, even if the value provided is already of the same type used in the action method. This is clearly unnecessary and inefficient.

### What about Model Binders as constraints?

If model binders are used to parse route parameters it would seem logical to reuse them for constraints. However, there are several issues associated with this idea:

- Model binders depend on [ControllerContext][2], which depends on [RequestContext][3], which depends on [RouteData][4]. None of these are available when route constraints are executed.
- Model binding is too powerful for simple types. It depends on [ModelMetadata][5], [IValueProvider][6]s, it supports prefixed parameters, etc. This power and complexity is not needed for parsing route parameters.
- MVC and Web API have different model binding APIs, you'd need to write two different binders to support both frameworks.

*Parameter Binding* is introduced to overcome all of these issues.

How it works
------------
This is the definition of the [ParameterBinder][7] class:

```csharp
public abstract class ParameterBinder {

   public abstract Type ParameterType { get; }

   public abstract bool TryBind(string value, IFormatProvider provider, out object result);
}
```

The [ParameterType][8] property declares which type the binder is meant for.

The [TryBind][9] method takes the string value of a route parameter and returns true if the bind is successful, or false if not, while also returning the bound value in the result output parameter. The *value* parameter is always a non-null, non-empty instance. The *provider* parameter is always [CultureInfo.InvariantCulture][10].

How to use it
-------------
There are three ways to configure binders. The first way is by using the [ParameterBinder][11] attribute on the type.

The second way, which overrides the first, is using the [ParameterBinders][12] setting. By default it includes binders for [Boolean][13], [Guid][14], [Decimal][15], [Double][16], [Single][17], [SByte][18], [Int16][19], [Int32][20], [Int64][21], [Byte][22], [UInt16][23], [UInt32][24] and [UInt64][25]. You can add binders for other types, or override the built-in binders.

The third way, which overrides the first and the second, is using `[FromRoute(BinderType)]`, which now can be used to specify either a model binder or parameter binder.

[1]: http://msdn.microsoft.com/en-us/library/system.convert
[2]: http://msdn.microsoft.com/en-us/library/system.web.mvc.controllercontext
[3]: http://msdn.microsoft.com/en-us/library/system.web.routing.requestcontext
[4]: http://msdn.microsoft.com/en-us/library/system.web.routing.routedata
[5]: http://msdn.microsoft.com/en-us/library/system.web.mvc.modelmetadata
[6]: http://msdn.microsoft.com/en-us/library/system.web.mvc.ivalueprovider
[7]: api/MvcCodeRouting.ParameterBinding/ParameterBinder/README.md
[8]: api/MvcCodeRouting.ParameterBinding/ParameterBinder/ParameterType.md
[9]: api/MvcCodeRouting.ParameterBinding/ParameterBinder/TryBind.md
[10]: http://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo.invariantculture
[11]: api/MvcCodeRouting.ParameterBinding/ParameterBinderAttribute/README.md
[12]: api/MvcCodeRouting/CodeRoutingSettings/ParameterBinders.md
[13]: http://msdn.microsoft.com/en-us/library/system.boolean
[14]: http://msdn.microsoft.com/en-us/library/system.guid
[15]: http://msdn.microsoft.com/en-us/library/system.decimal
[16]: http://msdn.microsoft.com/en-us/library/system.double
[17]: http://msdn.microsoft.com/en-us/library/system.single
[18]: http://msdn.microsoft.com/en-us/library/system.sbyte
[19]: http://msdn.microsoft.com/en-us/library/system.int16
[20]: http://msdn.microsoft.com/en-us/library/system.int32
[21]: http://msdn.microsoft.com/en-us/library/system.int64
[22]: http://msdn.microsoft.com/en-us/library/system.byte
[23]: http://msdn.microsoft.com/en-us/library/system.uint16
[24]: http://msdn.microsoft.com/en-us/library/system.uint32
[25]: http://msdn.microsoft.com/en-us/library/system.uint64
