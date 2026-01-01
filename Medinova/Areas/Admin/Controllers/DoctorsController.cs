using Medinova.Controllers;
using Medinova.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DoctorModel = Medinova.Models.Doctor;

namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class DoctorsController : Controller
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            var doctors = context.Doctors.Include("Department").ToList();
            return View(doctors);
        }
        public ActionResult CreateDoctor()
        {
            ViewBag.Departments = context.Departments.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateDoctor(DoctorModel doctor)
        {
            var depts = context.Departments.ToList();
            ViewBag.Departments = depts;
            context.Doctors.Add(doctor);
            context.SaveChanges();
            TempData["CreateSuccess"] = doctor.FullName + " başarıyla sisteme eklendi.";
            return RedirectToAction("Index");
        }

        public ActionResult DeleteDoctor(int id)
        {
            var doctor = context.Doctors.Find(id);
            if (doctor != null)
            {
                context.Doctors.Remove(doctor);
                context.SaveChanges();
                TempData["DeleteSuccess"] = "Doktor başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult UpdateDoctor(int id)
        {
            var doctor = context.Doctors.Find(id);
            ViewBag.Departments = context.Departments.ToList();
            return View(doctor);
        }

        [HttpPost]
        public ActionResult UpdateDoctor(Medinova.Models.Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                context.Entry(doctor).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                TempData["EditSuccess"] = "Doktor bilgileri güncellendi.";
                return RedirectToAction("Index");
            }

            ViewBag.Departments = context.Departments.ToList();
            return View(doctor);
        }
    }
}