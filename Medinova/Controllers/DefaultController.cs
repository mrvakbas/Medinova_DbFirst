using Medinova.DTOs;
using Medinova.Enums;
using Medinova.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    public class DefaultController : BaseController
    {
        MedinovaContext context = new MedinovaContext();
        public ActionResult Index()
        {
            return View();
        }

        private const string GEMINI_API_KEY = "AIzaSyCFB3DyFK0imrvWfU8xyNFktflgfyOC7Jc";
        private const string GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        [HttpPost]
        public async Task<ActionResult> GetHealthAdvice(HealthQuestion question)
        {
            try
            {
                if (question == null || string.IsNullOrWhiteSpace(question.Message))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Lütfen bir soru veya belirtiniz girin."
                    });
                }

                var systemInstruction = @"Sen bir AI sağlık asistanısın. Kullanıcının belirtilerini dinle ve:
1. Hangi doktor bölümüne gitmesi gerektiğini öner
2. Kısaca bu belirtiler hakkında bilgi ver
3. Ne zaman acil servise gitmesi gerektiğini belirt
4. Her zaman hatırla: Senin tavsiyelerine dayanarak tıbbi kararlar alma, mutlaka bir doktor ile görüş.

Cevaplarını Türkçe ver ve samimi bir dille iletişim kur.";

                var prompt = $"{systemInstruction}\n\nKullanıcı: {question.Message}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                using (var client = new HttpClient())
                {
                    // Timeout ekle
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var jsonContent = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var url = $"{GEMINI_API_URL}?key={GEMINI_API_KEY}";
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(responseContent);

                        string assistantResponse = result.candidates[0].content.parts[0].text;

                        return Json(new
                        {
                            success = true,
                            response = assistantResponse
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            error = $"API Hatası: {response.StatusCode} - {errorContent}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = $"Bir hata oluştu: {ex.Message} | {ex.InnerException?.Message}"
                });
            }
        }



        [HttpGet]
        public PartialViewResult DefaultAppointment()
        {
            var departments = context.Departments.ToList();
            ViewBag.departments = (from department in departments
                                   select new SelectListItem
                                   {
                                       Text = department.Name,
                                       Value = department.DepartmentId.ToString()
                                   }).ToList();

            var dateList = new List<SelectListItem>();

            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Now.AddDays(i);

                dateList.Add(new SelectListItem
                {
                    Text = date.ToString("dd MMMM dddd"),
                    Value = date.ToString("yyyy-MM-dd")
                });
            }

            ViewBag.dateList = dateList;

            return PartialView();
        }

        [HttpPost]
        [Authorize] // Sadece giriş yapmış kullanıcıların işlem yapmasını sağlar
        public ActionResult MakeAppointment(Appointment appointment)
        {
            // 1. Giriş yapan kullanıcının UserName bilgisini Identity üzerinden alıyoruz
            var currentUserName = User.Identity.Name;

            // 2. Users tablosundan bu kullanıcıyı buluyoruz
            var user = context.Users.FirstOrDefault(u => u.UserName == currentUserName);

            if (user != null)
            {
                // 3. KRİTİK ADIM: Randevu nesnesine kullanıcının ID'sini atıyoruz
                appointment.UserId = user.UserId;

                // 4. Diğer varsayılan değerleri set ediyoruz
                appointment.IsActive = true;

                // Formda ad-soyad boşsa kullanıcı tablosundan dolduruyoruz
                if (string.IsNullOrEmpty(appointment.FullName))
                {
                    appointment.FullName = user.FirstName + " " + user.LastName;
                }

                context.Appointments.Add(appointment);
                context.SaveChanges();
                LogAction("Yeni Randevu", $"{appointment.FullName} için {appointment.AppointmentDate:dd.MM.yyyy}{appointment.AppointmentTime} tarihine randevu oluşturuldu.");
                return RedirectToAction("Index");

            }

            // Kullanıcı bulunamazsa hata mesajı ekleyip formu geri döndür
            ModelState.AddModelError("", "Kullanıcı kimliği doğrulanamadı.");
            return View(appointment);
        }

        public JsonResult GetDoctorsByDepartmentId(int departmentId)
        {
            var doctors = context.Doctors.Where(x => x.DepartmentId == departmentId)
                                           .Select(doctor => new SelectListItem
                                           {
                                               Text = doctor.FullName,
                                               Value = doctor.DoctorId.ToString()
                                           }).ToList();
            return Json(doctors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAvailableHours(DateTime selectedDate, int doctorId)
        {
            var bookedTimes = context.Appointments.Where(x => x.DoctorId == doctorId && x.AppointmentDate == selectedDate).Select(x => x.AppointmentTime).ToList();

            var dtoList = new List<AppointmentAvailabilityDto>();

            foreach (var hour in Times.AppointmentHours)
            {
                var dto = new AppointmentAvailabilityDto();
                dto.Time = hour;

                if (bookedTimes.Contains(hour))
                {
                    dto.IsBooked = true;
                }
                else
                {
                    dto.IsBooked = false;
                }

                dtoList.Add(dto);
            }

            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult HeroSection()
        {
            var banner = context.Banners.ToList();
            return PartialView(banner);
        } 
        public PartialViewResult AboutSection()
        {
            var about = context.Abouts.ToList();
            return PartialView(about);
        } 
        public PartialViewResult AboutItemSection()
        {
            var aboutItem = context.AboutItems.ToList();
            return PartialView(aboutItem);
        } 
        public PartialViewResult FeatureSection()
        {
            var feature = context.Features.ToList();
            return PartialView(feature);
        } 
        public PartialViewResult PackageSection()
        {
            var packages = context.Packages.ToList();
            return PartialView(packages);
        } 
        public PartialViewResult DoctorListSection()
        {
            var doctor = context.Doctors.Take(5).ToList();
            return PartialView(doctor);
        } 
        public PartialViewResult TestimonialSection()
        {
            var testimonials = context.Testimonials.ToList();
            return PartialView(testimonials);
        } 

    }
}