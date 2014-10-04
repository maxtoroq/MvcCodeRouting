Known Issues
============

Html.Action() fails when using the special controller reference syntax, and the application has at least one Area
-----------------------------------------------------------------------------------------------------------------
Example:

```csharp
@Html.Action("LogOnStatus", "~~Account")
```

The above code works fine if the application has no Areas. If you are using Areas, the workaround is to specify the route context using the *__routecontext* route value, like this:

```csharp
@Html.Action("LogOnStatus", "Account", new { __routecontext = "" })
```

Embedded Views do not work in ASP.NET MVC 5.0
---------------------------------------------
This is a [known issue][1] in MVC 5.0. You need to either downgrade to 4.x or upgrade to 5.1

[1]: http://aspnetwebstack.codeplex.com/workitem/1362
