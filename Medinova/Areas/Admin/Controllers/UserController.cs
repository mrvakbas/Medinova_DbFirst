using Medinova.Controllers;
using Medinova.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using static Medinova.Areas.Admin.Data.UserRoleViewModel;

namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class UserController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            var user = context.Users.Include(x=>x.Roles).ToList();
            return View(user);
        }
        public ActionResult AssignRole(int id)
        {
            var user = context.Users.Find(id);
            var allRoles = context.Roles.ToList();

            var model = new AssignRoleViewModel
            {
                UserId = user.UserId,
                FullName = user.FirstName + " " + user.LastName,
                Roles = allRoles.Select(r => new RoleSelection
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    IsSelected = user.Roles.Any(ur => ur.RoleId == r.RoleId)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignRole(AssignRoleViewModel model)
        {
            var user = context.Users.Include("Roles").FirstOrDefault(u => u.UserId == model.UserId);

            if (user != null)
            {
                user.Roles.Clear();

                foreach (var role in model.Roles.Where(x => x.IsSelected))
                {
                    var dbRole = context.Roles.Find(role.RoleId);
                    user.Roles.Add(dbRole);
                }

                context.SaveChanges();
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View(model);
        }
    }
}