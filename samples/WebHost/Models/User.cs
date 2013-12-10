using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Samples.Models {
   
   public class User {

      public int Id { get; set; }
      public string Name { get; set; }

      public static IEnumerable<User> All() {
         yield return new User { Id = 1, Name = "Mark" };
         yield return new User { Id = 2, Name = "Don" };
         yield return new User { Id = 3, Name = "Mel" };
         yield return new User { Id = 4, Name = "Craig" };
      }
   }
}