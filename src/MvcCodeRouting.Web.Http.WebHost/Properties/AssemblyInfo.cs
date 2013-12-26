using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using MvcCodeRouting.Web.Http.WebHost;

[assembly: AssemblyTitle("MvcCodeRouting.Web.Http.WebHost.dll")]
[assembly: AssemblyDescription("MvcCodeRouting.Web.Http.WebHost.dll")]
[assembly: ComVisible(false)]
[assembly: CLSCompliantAttribute(true)]
[assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Justification = "Term is recognized.")]
