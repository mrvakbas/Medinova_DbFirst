using Medinova.Models;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    [Route("/About")]
    public class AboutsController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            var about = context.Abouts.ToList();
            return View(about);
        }
        public PartialViewResult AboutItemSection()
        {
            var aboutItem = context.AboutItems.ToList();
            return PartialView(aboutItem);
        }
    }
}