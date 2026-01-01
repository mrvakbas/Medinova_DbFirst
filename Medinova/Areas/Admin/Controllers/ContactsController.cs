using Medinova.Controllers;
using Medinova.Models;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ContactsController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            var contact = context.Contacts.ToList();
            return View(contact);
        }
    }
}