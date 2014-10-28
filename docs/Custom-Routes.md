Custom Routes
=============
The namespace/controller/action/parameters convention that MvcCodeRouting uses for creating routes will suit your needs on most cases. For those other cases where a more specialized route is required you can use the *CustomRoute* attribute, e.g.:

```csharp
namespace CoolBlog.Controllers {
    
    public class BlogController : Controller {
      
        [CustomRoute("{year}/{month}/{slug}")]
        public ActionResult Post(
            [FromRoute(Constraint = "[0-9]{4}")]int year, 
            [FromRoute(Constraint = "[0-9]{2}")]int month, 
            [FromRoute]string slug) {
            ...
        }
    }
}
```

The route created for the above action is:

```csharp
routes.MapRoute(null, "Blog/{year}/{month}/{slug}", 
    new { controller = "Blog", action = "Post" }, 
    new { year = new RegexRouteConstraint(@"[0-9]{4}"), month = new RegexRouteConstraint(@"[0-9]{2}") }, 
    new[] { "CoolBlog.Controllers" });
```

Note that the route is still relative to the controller and namespace. For an absolute route use the `~/` prefix:

```csharp
[CustomRoute("~/{year}/{month}/{slug}")]
public ActionResult Post(
    [FromRoute(Constraint = "[0-9]{4}")]int year, 
    [FromRoute(Constraint = "[0-9]{2}")]int month, 
    [FromRoute]string slug) {
    ...
}
```

Which produces:

```csharp
routes.MapRoute(null, "{year}/{month}/{slug}", 
    new { controller = "Blog", action = "Post" }, 
    new { year = new RegexRouteConstraint(@"[0-9]{4}"), month = new RegexRouteConstraint(@"[0-9]{2}") }, 
    new[] { "CoolBlog.Controllers" });
```

Decorating controllers
----------------------
The *CustomRoute* attribute can also be used to decorate controllers, e.g.:

```csharp
namespace CoolBlog.Controllers {
    
    [CustomRoute("~/{year}/{month}/{slug}")]
    public class PostController : Controller {
          
        [FromRoute(Constraint = "[0-9]{4}")]
        public int Year { get; set; }

        [FromRoute(Constraint = "[0-9]{2}")]
        public int Month { get; set; }
        
        [FromRoute]
        public string Slug { get; set; }
        
        protected override void Initialize(RequestContext requestContext) {
            
            base.Initialize(requestContext);
            this.BindRouteProperties();
        }
        
        public ActionResult Index() {
            ...
        }
        
        public ActionResult Comments() {
            ...
        }
    }
}
```

Which produces:

```csharp
routes.MapRoute(null, "{year}/{month}/{slug}/{action}", 
    new { controller = "Post", action = "Index" }, 
    new { action = new SetRouteConstraint("Index", "Comments"), year = new RegexRouteConstraint(@"[0-9]{4}"), month = new RegexRouteConstraint(@"[0-9]{2}") }, 
    new[] { "CoolBlog.Controllers" });
```

Other uses for decorating controllers with *CustomRoute* include:

- Renaming a controller without breaking URL generation and views location.
- Ignoring namespace segments

See Also
--------
- [Hierarchical Routes][1]
- [Route Formatting][2]

[1]: Hierarchical-Routes.md
[2]: Route-Formatting.md
