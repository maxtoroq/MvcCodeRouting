Embedded Views
==============
MvcCodeRouting includes a [VirtualPathProvider][1] implementation that can load views embedded as assembly resources. Use the [EnableEmbeddedViews][2] setting to enable this feature, like in the following example:

```csharp
void RegisterRoutes(RouteCollection routes) {
   
    routes.MapCodeRoutes(
        baseRoute: "Account",
        rootController: typeof(MvcAccount.AccountController),
        settings: new CodeRoutingSettings { 
            EnableEmbeddedViews = true
        });
}
```

You also need to call [EnableCodeRouting][3] on the view engine collection.

```csharp
void RegisterViewEngines(ViewEngineCollection engines) {
    engines.EnableCodeRouting();
}
```

You can override embedded views using file views in your application. The *VirtualPathProvider* implementation presents a merged view (no pun intended) of file and embedded views. When a view exists in both file system and the assembly, the file view takes precedence.

How to embed views
------------------
View resources must be named using the following convention:

```text
{assemblyName}.Views.{viewPath}
```

Here are some examples:

```text
MvcAccount.Views._ViewStart.cshtml
MvcAccount.Views.Account.Index.cshtml
MvcAccount.Views.Authentication.EditorTemplates.Object.cshtml
```

In Visual Studio follow these steps:

1. Create a *Views* folder at the root of your class library project.
2. Put the view files there, or in any subfolder under it. 
3. Change the *Build Action* to *Embedded Resource* for each view file.

General considerations when using embedded views
------------------------------------------------
- Don't use dots (`.`) in the file name except for the extension. When attempting to map a virtual path to an assembly resource, dots are interpreted as path separators (`/`).
- MvcCodeRouting cannot load embedded Web.config files, as the framework always load these from the file system.
- Visual Studio uses the root namespace as the first segment, not the assembly name. These are equal by default, but if you change the root namespace MvcCodeRouting won't be able to load views from that assembly.

Extracting views
----------------
MvcCodeRouting's NuGet package includes an *Extract-Views* PowerShell command you can use to copy embedded views to your project. You can invoke this command from the *Package Manager Console*, for example:

```powershell
PM> Extract-Views MvcAccount Account
```

Usage:

```text
Extract-Views [-AssemblyName] <String> [[-ViewsDirectory] <String>] [[-ProjectName] <String>] [-Culture <String>]
```

Parameters:

- **AssemblyName**: Specifies the assembly that contains embedded views.
- **ViewsDirectory**: Specifies the directory relative to the *Views* directory where you want to save the views. e.g. if *Foo\Bar* views are saved in *Views\Foo\Bar*. If omitted, views are saved directly in *Views*.
- **ProjectName**: Specifies the project to use as context. If omitted, the default project is chosen.
- **Culture**: Specifies the culture of the satellite assembly to extract views from. If omitted, views are extracted from the main assembly.

[1]: http://msdn.microsoft.com/en-us/library/system.web.hosting.virtualpathprovider
[2]: api/MvcCodeRouting/CodeRoutingSettings/EnableEmbeddedViews.md
[3]: api/MvcCodeRouting/CodeRoutingExtensions/EnableCodeRouting_1.md
