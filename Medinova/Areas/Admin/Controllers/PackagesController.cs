using Medinova.Controllers;
using Medinova.Models;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class PackagesController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            var package = context.Packages.ToList();
            return View(package);
        }
    }
}