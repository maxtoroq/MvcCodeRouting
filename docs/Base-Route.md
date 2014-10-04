Base Route
==========
*baseRoute* is another parameter of the *MapCodeRoutes* method. It's used in more advanced scenarios, such as:

- Use a common base route for all controllers
- Split a large application into various projects
- *Rent* a URL space to *Portable Modules*

Common base route
-----------------
For example, let's consider a blog hosting application where each user gets a URL like this one:

```text
http://example.com/blogs/{username}
```

In this case `blogs/{username}` is the base route, and namespace, controller, action and parameter segments come after that. The base route is just prepended to the routes that MvcCodeRouting creates.

This is what the *MapCodeRoutes* call would look like:

```csharp
routes.MapCodeRoutes(
    baseRoute: "blogs/{username}",
    rootController: typeof(CoolBlog.Controllers.HomeController));
```

You can access the base route parameter values like this:

```csharp
public class HomeController : Controller {
    
    public string Username { get; set; }
    
    protected override void Initialize(RequestContext context) {
        this.Username = context.RouteData.GetRequiredString("username");
    }
}
```

Split a large application into various projects
-----------------------------------------------
Another use for base routes is to split a large application into various project, then bringing them all together in the *main* app by calling *MapCodeRoutes* once for each project. For example: 

```csharp
// Register the main (entry) module
routes.MapCodeRoutes(
    rootController: typeof(HomeController),
    settings: new CodeRoutingSettings { 
        EnableEmbeddedViews = true
    });
    
// Register the Catalog module
routes.MapCodeRoutes(
    baseRoute: "catalog",
    rootController: typeof(Catalog.CatalogController),
    settings: new CodeRoutingSettings { 
        EnableEmbeddedViews = true
    });

// Register the Checkout module
routes.MapCodeRoutes(
    baseRoute: "checkout",
    rootController: typeof(Checkout.CheckoutController),
    settings: new CodeRoutingSettings { 
        EnableEmbeddedViews = true
    });

// Register the PayPal module
routes.MapCodeRoutes(
    baseRoute: "checkout/paypal",
    rootController: typeof(Checkout.PayPal.PayPalController),
    settings: new CodeRoutingSettings {
        EnableEmbeddedViews = true
    });

```

Portable Modules
----------------
*Portable Modules* are applications that are self-contained in a single assembly with views embedded as assembly resources. The idea is to easily reuse functionality without having to copy source code. One example of such module is [MvcAccount][1].

See Also
--------
- [Embedded Views][2]

[1]: https://github.com/maxtoroq/MvcAccount/blob/master/docs/Installation-Instructions.md
[2]: Embedded-Views.md
