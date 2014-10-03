Links and Controller Reference Syntax
=====================================
With MvcCodeRouting, URL generation is relative to the current route (the route that matched the current HTTP request). That means that when you do `Url.Action("Foo", "Bar")`, the *Bar* controller must be in the same *Route Context* as the current executing controller.

Route Context
-------------
*Route Context* is the concept that MvcCodeRouting uses to define a hierarchy of routes, and in consequence a hierarchy of controllers. Controllers that share the same route context are considered siblings when it comes to matching routes for URL generation. This means that no special syntax is required to refer to other sibling controllers. Usually, controllers in the same namespace are also in the same route context, except for [default controllers][1], which are in the route context of their parent namespace. For example:

Controller                                             | Route Context
------------------------------------------------------ | ------------------------------
*MyStore.Controllers.HomeController*                   | (empty)
*MyStore.Controllers.UserController*                   | (empty)
*MyStore.Controllers.Admin.AdminController*            | (empty, default controller)
*MyStore.Controllers.Admin.UserController*             | `Admin`
*MyStore.Controllers.Admin.ProductController*          | `Admin`
*MyStore.Controllers.Admin.Product.CategoryController* | `Admin/Product`

Asuming *MyStore.Controllers.HomeController* is the root controller, what controller does `Url.Action("", "User")` link to? it depends on the current executing controller; if it's *HomeController* then it links to *UserController*; if it's *Admin.ProductController* then it links to *Admin.UserController*.

Controller Reference Syntax
---------------------------
MvcCodeRouting recognizes a special controller reference syntax that you can use to link to controllers in a different route context than the current executing controller.

Reference name            | Example code                          | Current controller        | Returns
------------------------- | ------------------------------------- | ------------------------- | -------------------------
Sibling or self           | `Url.Action("", "User")`              | *HomeController*          | `/User`
Child or child of sibling | `Url.Action("", "Admin.User")`        | *HomeController*          | `/Admin/User`
Child                     | `Url.Action("", "+User")`             | *Admin.AdminController*   | `/Admin/User`
Grandchild                | `Url.Action("", "+Product.Category")` | *Admin.AdminController*   | `/Admin/Product/Category`
Parent                    | `Url.Action("", "..User")`            | *Admin.ProductController* | `/User`
baseRoute-relative \*     | `Url.Action("", "~User")`             | *Admin.ProductController* | `/User`
Application-relative      | `Url.Action("", "~~User")`            | *Admin.ProductController* | `/User`

\* If you provide a *baseRoute* when calling the *MapCodeRoutes* method, that base route is also used as base route context. If *baseRoute* is null then this syntax yields the same results as the application-relative reference.

See Also
--------
- [Controllers and Namespaces][2]

[1]: Controllers-and-Namespaces.md#default-controllers
[2]: Controllers-and-Namespaces.md
