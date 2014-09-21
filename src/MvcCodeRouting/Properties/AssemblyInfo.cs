using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("MvcCodeRouting.dll")]
[assembly: AssemblyDescription("MvcCodeRouting.dll")]
[assembly: ComVisible(false)]
[assembly: CLSCompliantAttribute(true)]
[assembly: InternalsVisibleTo("MvcCodeRouting.Web")]
[assembly: InternalsVisibleTo("MvcCodeRouting.Web.Mvc")]
[assembly: InternalsVisibleTo("MvcCodeRouting.Web.Http")]
[assembly: InternalsVisibleTo("MvcCodeRouting.Web.Http.WebHost")]
[assembly: InternalsVisibleTo("MvcCodeRouting.AspNet.Mvc")]

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Justification = "Term is recognized.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "MvcCodeRouting", MessageId = "Mvc", Justification = "Term is recognized.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "MvcCodeRouting.Web.Mvc", MessageId = "Mvc", Justification = "Term is recognized.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "MvcCodeRouting.ParameterBinding", MessageId = "Mvc", Justification = "Term is recognized.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "MvcCodeRouting.ParameterBinding.Binders", MessageId = "Mvc", Justification = "Term is recognized.")]