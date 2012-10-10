using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using MvcCodeRouting.Web.Http.WebHost;

[assembly: AssemblyTitle("MvcCodeRouting.Web.Http.dll")]
[assembly: AssemblyDescription("MvcCodeRouting.Web.Http.dll")]
[assembly: ComVisible(false)]
[assembly: CLSCompliantAttribute(true)]
[assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "MvcCodeRouting", MessageId = "Mvc", Justification = "Term is recognized.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Justification = "Term is recognized.")]