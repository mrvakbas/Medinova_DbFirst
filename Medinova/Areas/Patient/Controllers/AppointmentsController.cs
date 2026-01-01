using Medinova.Controllers;
using Medinova.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Areas.Patient.Controllers
{
    [CustomAuthorize(Roles = "Patient")]
    public class AppointmentsController : BaseController
    {
        MedinovaContext _context = new MedinovaContext();

        public ActionResult Index()
        {
            try
            {
                var currentUserName = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == currentUserName);

                if (user == null) return RedirectToAction("Login", "Account");

                if (!user.Roles.Any(r => r.RoleId == 3))
                {
                    return HttpNotFound("Yetkisiz erişim.");
                }

                var myAppointments = _context.Appointments
                    .Where(a => a.UserId == user.UserId)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .ToList();

                // İstatistikler
                ViewBag.TotalAppointments = myAppointments.Count;
                ViewBag.ActiveAppointments = myAppointments.Count(a => a.IsActive == true);
                ViewBag.PassiveAppointments = myAppointments.Count(a => a.IsActive == false);

                return View(myAppointments);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Veriler yüklenirken bir hata oluştu: " + ex.Message;
                return View(new List<Appointment>());
            }
        }

        // Randevu İptal Etme
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            try
            {
                var currentUserName = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == currentUserName);

                if (user == null) return Json(new { success = false, message = "Oturum kapalı." });

                var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id && a.UserId == user.UserId);

                if (appointment == null)
                    return Json(new { success = false, message = "Randevu bulunamadı veya yetkiniz yok." });

                appointment.IsActive = false;
                _context.SaveChanges();
                LogAction("Randevu İptali", $"ID'si {id} olan randevu kullanıcı tarafından iptal edildi.");
                return Json(new { success = true, message = "Randevunuz başarıyla iptal edildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
     
    }
}