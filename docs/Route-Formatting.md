Route Formatting
================
Route formatting consists in changing parts of a route to suit a particular style. There are three types of segments that can be formatted: namespace, controller and action segments.

To format routes you use the [RouteFormatter][1] setting. In the following example routes are transformed from PascalCased to lowercase-hyphenated:

```csharp
void RegisterRoutes(RouteCollection routes) {
    
    routes.MapCodeRoutes(
        rootController: typeof(Controllers.HomeController),
        settings: new CodeRoutingSettings {
            RouteFormatter = args =>
                Regex.Replace(args.OriginalSegment, @"([a-z])([A-Z])", "$1-$2").ToLowerInvariant()
        }
    );
}
```

You can even change the segment to something completely different. The following example changes the *LogOn* action segment of the *Account* controller to *SignIn*:

```csharp
void RegisterRoutes(RouteCollection routes) {

    routes.MapCodeRoutes(
        rootController: typeof(Controllers.HomeController),
        settings: new CodeRoutingSettings {
            RouteFormatter = args => {
                if (args.ControllerType == typeof(Controllers.AccountController)
                    && args.SegmentType == RouteSegmentType.Action
                    && args.OriginalSegment == "LogOn") {
                    
                    return "SignIn";
                }
            
                return args.OriginalSegment;
            }
        }
    );
}
```

When you format an action or controller segment you don't actually change the action or controller name. For incoming request, the formatted segment is mapped to the original segment. The reverse is done for URL generation, the original segment is mapped to the formatted segment. Therefore, URL generation and views location are not affected by route formatting.

[1]: api/MvcCodeRouting/CodeRoutingSettings/RouteFormatter.md
