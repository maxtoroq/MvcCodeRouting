Namespace-based Modularity for ASP.NET MVC and Web API
======================================================

- Break your application into as much small pieces as you want, using namespaces and projects
- URL generation and Views location is relative to the module
- Embed your views which can be overridden by files views in the host application
- Create reusable portable modules

Convention over configuration Automatic Routing
-----------------------------------------------
- Routes are automatically created for you using convention over configuration
- Break away from the convention using attribute routes
- Default constraints for primitive types that can be overridden on a per-parameter or per-module basis
- Intelligent grouping of similar routes for efficient matching
- Formatting of routes (e.g. to lowercase, hyphen-separated, underscore-separated, etc)

MvcCodeRouting is an alternative to
-----------------------------------
- Conventional routing
- Custom routing
- Attribute routing
- Areas

Get it now! using NuGet
-----------------------
```powershell
# MVC
Install-Package MvcCodeRouting

# Web API
Install-Package MvcCodeRouting.Web.Http.WebHost
```

Motivation
----------
- [Why ASP.NET MVC Routing sucks](http://maxtoroq.github.io/2014/02/why-aspnet-mvc-routing-sucks.html)
- [Rethinking ASP.NET MVC: Workflow per Controller](http://maxtoroq.github.io/2013/02/aspnet-mvc-workflow-per-controller.html)

Resources
---------
- [Documentation](https://mvccoderouting.codeplex.com/documentation)
- [Ask for help](https://mvccoderouting.codeplex.com/discussions)
- [Report an issue](https://mvccoderouting.codeplex.com/workitem/list/basic)

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif)](http://maxtoroq.github.io/p/donate.html) [![Flattr this](https://api.flattr.com/button/flattr-badge-large.png)](http://flattr.com/thing/1761230/MvcCodeRouting)
