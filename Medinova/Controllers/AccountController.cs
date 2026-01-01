using Medinova.DTOs;
using Medinova.Models;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginDto model)
        {
            var user = context.Users.FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            FormsAuthentication.SetAuthCookie(user.UserName, false);
            Session["userName"] = user.UserName;
            Session["fullName"] = user.FirstName + " " + user.LastName;

            string[] roles = user.Roles.Select(r => r.RoleName).ToArray();
            IIdentity identity = new GenericIdentity(user.UserName);
            IPrincipal principal = new GenericPrincipal(identity, roles);
            HttpContext.User = principal;

            LogAction("Sisteme Giriş", $"{user.FirstName} {user.LastName} ({user.UserName}) başarıyla giriş yaptı.");

            if (user.Roles.Any(r => r.RoleName == "Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else if (user.Roles.Any(r => r.RoleName == "Doctor"))
            {
                return RedirectToAction("Index", "Appointment", new { area = "Doctor" });
            }
            else if (user.Roles.Any(r => r.RoleName == "Patient"))
            {
                return RedirectToAction("Index", "Appointments", new { area = "Patient" });
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            var currentUserName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.UserName == currentUserName);

            if (user != null)
            {
                LogAction("Sistemden Çıkış", $"{user.FirstName} {user.LastName} oturumu güvenli bir şekilde sonlandırdı.");
            }

            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Login");
        }
    }
}