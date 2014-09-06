using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MvcCodeRouting.Tests.Routing {
   
   [TestClass]
   public class ControllerReferenceBehavior {

      readonly RouteCollection routes;

      public ControllerReferenceBehavior() {
         routes = TestUtil.GetRouteCollection();
      }

      [TestMethod]
      public void Self() {

         var controller = typeof(ControllerReference.Self.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/Foo");

         Assert.AreEqual("/Bar", Url.Action("Bar", controller));
      }

      [TestMethod]
      public void Sibling() {

         var controller = typeof(ControllerReference.Sibling.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/Foo");

         Assert.AreEqual("/B/Foo", Url.Action("Foo", "B"));
      }

      [TestMethod]
      public void ChildOfSibling() {

         var controller = typeof(ControllerReference.ChildOfSibling.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/Foo");

         Assert.AreEqual("/B/C/Foo", Url.Action("Foo", "B.C"));
      }

      [TestMethod]
      public void Child() {

         var controller = typeof(ControllerReference.Child.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/Foo");

         Assert.AreEqual("/A/C/Foo", Url.Action("Foo", "+C"));
      }

      [TestMethod]
      public void GrandChild() {

         var controller = typeof(ControllerReference.GrandChild.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/Foo");

         Assert.AreEqual("/A/B/C/Foo", Url.Action("Foo", "+B.C"));
      }

      [TestMethod]
      public void SiblingOfParent() {

         var controller = typeof(ControllerReference.SiblingOfParent.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/A/C/Foo");

         Assert.AreEqual("/B/Foo", Url.Action("Foo", "..B"));
      }

      [TestMethod]
      public void Parent() {

         var controller = typeof(ControllerReference.Parent.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/A/C/Foo");

         Assert.AreEqual("/Foo", Url.Action("Foo", ".."));
      }

      [TestMethod]
      public void ParentOfDefaultController() {

         var controller = typeof(ControllerReference.Parent.AController);

         routes.Clear();
         routes.MapCodeRoutes(controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/A/D/Foo");

         Assert.AreEqual("/Foo", Url.Action("Foo", ".."));
      }

      [TestMethod]
      public void BaseRouteRelative() {

         routes.Clear();
         routes.MapCodeRoutes(typeof(ControllerReference.BaseRouteRelative.A.AController));

         var controller = typeof(ControllerReference.BaseRouteRelative.B.BController);

         routes.MapCodeRoutes("B", controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/B/Foo");

         Assert.AreEqual("/B/C/Foo", Url.Action("Foo", "~C"));
      }

      [TestMethod]
      public void ApplicationRelative() {

         routes.Clear();
         routes.MapCodeRoutes(typeof(ControllerReference.ApplicationRelative.A.AController));

         var controller = typeof(ControllerReference.ApplicationRelative.B.BController);

         routes.MapCodeRoutes("B", controller);

         var Url = TestUtil.CreateUrlHelper(routes, "~/B/Foo");

         Assert.AreEqual("/C/Foo", Url.Action("Foo", "~~C"));
      }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.Self {

   public class AController : Controller {

      public void Foo() { }

      public void Bar() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.Sibling {

   public class AController : Controller {

      public void Foo() { }
   }

   public class BController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.ChildOfSibling {

   public class AController : Controller {

      public void Foo() { }
   }

   public class BController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.ChildOfSibling.B {

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.Child {

   public class AController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.Child.A {

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.GrandChild {

   public class AController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.GrandChild.A.B {

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.SiblingOfParent {

   public class AController : Controller {

      public void Foo() { }
   }

   public class BController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.SiblingOfParent.A {

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.Parent {

   public class AController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.Parent.A {

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.Parent.A.D {

   public class DController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.BaseRouteRelative.A {

   public class AController : Controller {

      public void Foo() { }
   }

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.BaseRouteRelative.B {

   public class BController : Controller {

      public void Foo() { }
   }

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.ApplicationRelative.A {

   public class AController : Controller {

      public void Foo() { }
   }

   public class CController : Controller {

      public void Foo() { }
   }
}

namespace MvcCodeRouting.Tests.Routing.ControllerReference.ApplicationRelative.B {

   public class BController : Controller {

      public void Foo() { }
   }

   public class CController : Controller {

      public void Foo() { }
   }
}