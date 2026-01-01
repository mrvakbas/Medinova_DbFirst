using Medinova.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Medinova.Models;

namespace Medinova.Areas.Doctor.Controllers
{
    [CustomAuthorize(Roles = "Doctor")]
    public class AppointmentController : BaseController
    {
        MedinovaContext _context = new MedinovaContext();

        public ActionResult Index()
        {
            try
            {
                int doctorId = GetCurrentDoctorId();

                if (doctorId == 0)
                {
                    ViewBag.Error = "Doktor bilgisi bulunamadı";
                    return View(new List<Appointment>());
                }

                var appointments = _context.Appointments
                    .Where(a => a.DoctorId == doctorId)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .ToList();

                ViewBag.TotalAppointments = appointments.Count;
                ViewBag.ActiveAppointments = appointments.Where(a => a.IsActive == true).Count();
                ViewBag.CompletedAppointments = appointments.Where(a => a.IsActive == false).Count();

                return View(appointments);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Randevular yüklenirken hata oluştu: " + ex.Message;
                return View(new List<Appointment>());
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                int doctorId = GetCurrentDoctorId();
                var appointment = _context.Appointments
                    .FirstOrDefault(a => a.AppointmentId == id && a.DoctorId == doctorId);

                if (appointment == null)
                {
                    return HttpNotFound();
                }

                return View(appointment);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Randevu detayı yüklenirken hata oluştu: " + ex.Message;
                return View();
            }
        }

        public ActionResult UpcomingAppointments()
        {
            try
            {
                int doctorId = GetCurrentDoctorId();
                var today = DateTime.Now.Date;

                var appointments = _context.Appointments
                    .Where(a => a.DoctorId == doctorId && a.IsActive == true && a.AppointmentDate >= today)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToList();

                return View(appointments);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Yaklaşan randevular yüklenirken hata oluştu: " + ex.Message;
                return View(new List<Appointment>());
            }
        }

        public ActionResult CompletedAppointments()
        {
            try
            {
                int doctorId = GetCurrentDoctorId();

                var appointments = _context.Appointments
                    .Where(a => a.DoctorId == doctorId && a.IsActive == false)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .ToList();

                return View(appointments);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Tamamlanan randevular yüklenirken hata oluştu: " + ex.Message;
                return View(new List<Appointment>());
            }
        }

        [HttpPost]
        public ActionResult MarkAsCompleted(int id)
        {
            try
            {
                int doctorId = GetCurrentDoctorId();
                var appointment = _context.Appointments
                    .FirstOrDefault(a => a.AppointmentId == id && a.DoctorId == doctorId);

                if (appointment == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı" });
                }

                appointment.IsActive = false;
                _context.SaveChanges();

                return Json(new { success = true, message = "Randevu tamamlandı olarak işaretlendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            try
            {
                int doctorId = GetCurrentDoctorId();
                var appointment = _context.Appointments
                    .FirstOrDefault(a => a.AppointmentId == id && a.DoctorId == doctorId);

                if (appointment == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı" });
                }

                // Silme yerine pasif yap
                appointment.IsActive = false;
                _context.SaveChanges();
                LogAction("Randevu İptali", $"ID'si {id} olan randevu kullanıcı tarafından iptal edildi.");
                return Json(new { success = true, message = "Randevu pasif yapıldı" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult GetAppointmentsByDoctorId(int doctorId)
        {
            try
            {
                int currentDoctorId = GetCurrentDoctorId();

                // Sadece kendi randevularını görebilir
                if (doctorId != currentDoctorId)
                {
                    return Json(new { success = false, message = "Yetkiniz yok" }, JsonRequestBehavior.AllowGet);
                }

                var appointments = _context.Appointments
                    .Where(a => a.DoctorId == doctorId)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .Select(a => new
                    {
                        a.AppointmentId,
                        a.FullName,
                        a.PhoneNumber,
                        a.Email,
                        a.AppointmentDate,
                        a.AppointmentTime,
                    })
                    .ToList();

                return Json(new { success = true, data = appointments }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private int GetCurrentDoctorId()
        {
            try
            {
                // Session'dan doktor ID'sini al
                if (Session["DoctorId"] != null)
                {
                    return Convert.ToInt32(Session["DoctorId"]);
                }

                // Username'den doktor bul
                if (User.Identity.IsAuthenticated)
                {
                    var username = User.Identity.Name;
                    var user = _context.Users.FirstOrDefault(u => u.UserName == username);

                    if (user != null)
                    {
                        // Users tablosunda DoctorId varsa onu döndür
                        // Eğer yoksa, doctor tablosundan ad ile ara
                        var doctor = _context.Doctors.FirstOrDefault(d => d.FullName.Contains(user.FirstName));

                        if (doctor != null)
                        {
                            Session["DoctorId"] = doctor.DoctorId;
                            return doctor.DoctorId;
                        }
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}