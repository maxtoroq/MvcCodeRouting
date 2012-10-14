using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Samples.Models;

namespace Samples.Controllers.Admin {
   
   public class UserController : CrudController<User, int> {

      protected override IQueryable<User> GetAll() {
         return Samples.Models.User.All().AsQueryable();
      }

      protected override User GetById(int id) {
         return GetAll().SingleOrDefault(u => u.Id == id);
      }
   }
}