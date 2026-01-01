using Medinova.Models;
using System.Web.Mvc;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    public class ContactController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Contact contact)
        {
            context.Contacts.Add(contact);
            context.SaveChanges();
            return RedirectToAction("Index", "Default");
        }
    }
}