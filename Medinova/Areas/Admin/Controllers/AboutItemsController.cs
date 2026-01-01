using Medinova.Controllers;
using Medinova.Models;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AboutItemsController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            var aboutItems = context.AboutItems.ToList();
            return View(aboutItems);
        }
    }
}