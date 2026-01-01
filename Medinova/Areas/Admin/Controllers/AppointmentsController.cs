using Medinova.Controllers;
using Medinova.Models;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AppointmentsController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            var appointments = context.Appointments.ToList();
            return View(appointments);
        }
    }
}