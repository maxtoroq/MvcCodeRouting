Controllers and Namespaces
==========================

One of the main goals of MvcCodeRouting is to allow a better organization of large projects. Although ASP.NET MVC has the concept of Areas, they only go one level deeper. With MvcCodeRouting you can use namespaces to organize your controllers. Each namespace segment after the root namespace automatically becomes part or the URL, making it really easy to manage and evolve your application.

Please note that using namespace hierarchies to organize your controllers is completely optional, if you are comfortable having all your controllers in the same namespace that's fine.

Default controllers
-------------------
A default controller is a controller in a sub-namespace that has the same name as the last segment of its namespace. For example, *MyStore.Controllers.Admin.AdminController* is the default controller of the *MyStore.Controllers.Admin* namespace, and its routes start with `Admin/{action}` (note that *Admin* is not repeated twice).

The purpose of default controllers is to allow a logical grouping of controllers and other related types like view models. For example, lets consider these types:

- *MyStore.Controllers.Admin.AdminController*
- *MyStore.Controllers.Admin.Action1ViewModel*
- *MyStore.Controllers.Admin.Action2ViewModel*

All these types are grouped in the *MyStore.Controllers.Admin* namespace because logically they are closely related. However, if the root namespace is *MyStore.Controllers*, without this feature the routes for *AdminController* would start with `Admin/Admin/{action}`, where the first *Admin* segment maps to the namespace and the second to the controller name. To not repeat *Admin* twice in the URLs you would have to move *AdminController* to its parent namespace, consequently separating it from its other related types.

Default controllers allow you to follow an organizational pattern where all controllers live in their own namespace with their view models. This is optional, it's completely OK to have more than one controller in a namespace. However, if your controller and related view models grow you can always move it to a sub-namespace with the same name, without breaking URL generation or views location.

### Ambiguous route detection

Continuing with the previous example, nothings stops you from having these controllers:

- *MyStore.Controllers.AdminController*
- *MyStore.Controllers.Admin.AdminController*

The routes for these controllers would start the same:

- `Admin/{action}`
- `Admin/{action}`

This is valid as long as you don't run into conflicts (e.g. both have an action with the same name). **MvcCodeRouting throws an exception when it detects ambiguous routes**.
