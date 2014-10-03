Hierarchical (a.k.a. RESTful) Routes
====================================
Hierarchical routes are routes where one or more parameters go before the action parameter, for example:

```text
Book/{id}
Book/{id}/Chapter/{chapterId}/{page}
```

There are 2 ways to achieve this. One, is by decorating controller properties with the *FromRoute* attribute, for example:

```csharp
public class BookController : Controller {
    
    [FromRoute]
    public int id { get; set; }
    
    protected override void Initialize(RequestContext requestContext) {
    
        base.Initialize(requestContext);
        this.BindRouteProperties();
    }
    
    public ActionResult Index() {
        ...
    }
    
    public ActionResult Chapter([FromRoute]int chapterId, [FromRoute]int page = 1) {
        ...
    }
}
```

Properties decorated with the *FromRoute* attribute are **always** required.

The other way is by using the *CustomRoute* attribute:

```csharp
public class BookController : Controller {
    
    [CustomRoute("{id}")]
    public ActionResult Details([FromRoute]int id) {
        ...
    }
    
    [CustomRoute("{id}/{action}/{chapterId}/{page}")]
    public ActionResult Chapter([FromRoute]int id, [FromRoute]int chapterId, [FromRoute]int page = 1) {
        ...
    }
}
```

The advantage of using the *CustomRoute* attribute is that you can implement `Book` and `Book/{id}` on the same controller.

See Also
--------
- [Custom Routes][1]
 
[1]: Custom-Routes.md
