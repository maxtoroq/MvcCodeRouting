Getting Started
===============

Convention over configuration Routing
-------------------------------------
Suppose you want a URL like `Store/Products/Browse/{category}/{page}`, the *Store* segment can be mapped to a namespace, *Products* to a controller, *Browse* to an action, and the rest to parameters. So, your code should look like this:

```csharp
using System.Web.Mvc;
using MvcCodeRouting.Web.Mvc;

namespace MyStore.Controllers.Store {
   
    public class ProductsController : Controller {
      
        public ActionResult Browse([FromRoute]string category, [FromRoute]int page = 1) { 
            ...
        }
    }
}
```

Note that the *page* parameter has a default value, that makes the route parameter optional.

> #### Importing the correct namespace
> The above example imports the *MvcCodeRouting.Web.Mvc* namespace, which contains the attributes and extension methods for ASP.NET MVC controllers. For ASP.NET Web API controllers you should import the *MvcCodeRouting.Web.Http* namespace. For more information about MvcCodeRouting's namespaces see the [API reference][1].

### Default actions

By convention, an action named *Index* (with zero route parameters) is used as the default action. To override the convention decorate your action with the [DefaultAction][2] attribute.

### Action-based vs. verb-based routing

Web API supports both action-based and verb-based routing. For action-based routing you must use an HTTP verb attribute, such as `[HttpGet]`, for example:

```csharp
[HttpGet]
public string Foo() {
   return "Foo";
}
```

If an HTTP verb attribute is **not** used then it is asumed that the action name **is** the HTTP method and verb-based routing will be used:

```csharp
public string Get() {
   return "Foo";
}
```

Although Web API also supports a hybrid approach (e.g. `GetFoo()`), this doesn't work with MvcCodeRouting, unless you actually want *Get* to be part of the action name.

You can use both action-based and verb-based routing in the same controller.

Registration
------------
Use the [MapCodeRoutes][3] extension method to register your controllers:

```csharp
using System.Web.Routing;
using MvcCodeRouting;

void RegisterRoutes(RouteCollection routes) {

    routes.MapCodeRoutes(typeof(Controllers.HomeController));
}
```

The *rootController* is used as the starting point for your controllers and routes, controllers must be defined in the same namespace or any sub-namespace beneath it, in the same assembly. Routes for the root controller do not include the controller segment, e.g. `About` instead of `Home/About`. Imagine a bunch of files in the C:\ drive, they do not belong to a folder. In MVC of course, actions must belong to a controller, so we simply designate a controller to act as the root controller. 

> #### Web API
> Unlike *MapRoute* vs. *MapHttpRoute*, with MvcCodeRouting you don't need to call a different method to create routes for Web API controllers. Both MVC and Web API controllers can coexist side by side in the same namespace. Just make sure you install the [MvcCodeRouting.Web.Http.WebHost][5] package, otherwise your Web API controllers will be ignored.
> 
> This also means that in the above example, instead of using an MVC controller as root you can use a Web API controller.
> 
> However, if you are using a different kind of hosting, e.g. OWIN, then you should call the [MapCodeRoutes][4] method that extends [HttpConfiguration][6].

Namespace-aware views location
------------------------------
To enable namespace-aware views location call the [EnableCodeRouting][7] extension method:

```csharp
void Application_Start() {
    RegisterRoutes(RouteTable.Routes);
    RegisterViewEngines(ViewEngines.Engines);
}

void RegisterViewEngines(ViewEngineCollection viewEngines) {
     
    // Call AFTER you are done making changes to viewEngines
    viewEngines.EnableCodeRouting();
}
```

Visualize your routes
---------------------
After installing MvcCodeRouting using NuGet you should have the following code in your Web.config:

```xml
<system.web>
    <httpHandlers>
        <add path="routes.axd" verb="GET,HEAD" type="MvcCodeRouting.RouteDebugHandler, MvcCodeRouting"/>
    </httpHandlers>
</system.web>
<system.webServer>
    <validation validateIntegratedModeConfiguration="false"/>
    <handlers>
        <add name="MvcCodeRouting.RouteDebugHandler" path="routes.axd" verb="GET,HEAD" type="MvcCodeRouting.RouteDebugHandler, MvcCodeRouting"/>
    </handlers>
</system.webServer>
```

Navigate to `~/routes.axd` to visualize the routes that MvcCodeRouting created for you. `~/routes.axd` only works for [local requests][8].

[1]: api/README.md
[2]: api/MvcCodeRouting.Web.Mvc/DefaultActionAttribute/README.md
[3]: api/MvcCodeRouting/CodeRoutingExtensions/README.md#methods
[4]: api/MvcCodeRouting/CodeRoutingHttpExtensions/README.md#methods
[5]: Installing.md
[6]: http://msdn.microsoft.com/en-us/library/system.web.http.httpconfiguration
[7]: api/MvcCodeRouting/CodeRoutingExtensions/EnableCodeRouting_1.md
[8]: http://msdn.microsoft.com/en-us/library/system.web.httprequest.islocal
