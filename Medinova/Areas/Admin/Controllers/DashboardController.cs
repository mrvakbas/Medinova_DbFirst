using Medinova.Areas.Admin.Data;
using Medinova.Controllers;
using Medinova.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;


namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        MedinovaContext context = new MedinovaContext();

        public ActionResult Index()
        {
            ViewBag.DoctorCount = context.Doctors.Count();
            ViewBag.DepartmentCount = context.Departments.Count();
            ViewBag.UserCount = context.Users.Count();
            ViewBag.AppointmentCount = context.Appointments.Count();

            var viewModel = new DashboardViewModel()
            {
                LastDoctors = context.Doctors
                                     .OrderByDescending(x => x.DoctorId)
                                     .Take(5)
                                     .ToList(),
                AllDepartments = context.Departments
                                        .OrderByDescending(d => d.DepartmentId)
                                        .Take(5)
                                        .ToList()
            };

            var weeklyData = GetWeeklyAppointmentsTrend();
            ViewBag.WeeklyAppointments = weeklyData;

            var departmentData = GetDepartmentAppointments();
            ViewBag.AppointmentsByDepartment = departmentData;

            System.Diagnostics.Debug.WriteLine("\n🚀 ===== ML TAHMİN BAŞLIYOR ===== 🚀");

            try
            {
                var predictionResult = PredictNext90Days();

                if (predictionResult == null || predictionResult.ForecastedCount == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ predictionResult null döndü!");
                    ViewBag.ForecastLabels = "[]";
                    ViewBag.ForecastData = "[]";
                }
                else
                {
                    var forecastLabels = Enumerable.Range(1, 90)
                        .Select(i => DateTime.Now.AddDays(i).ToString("dd MMM", new System.Globalization.CultureInfo("tr-TR")))
                        .ToList();

                    ViewBag.ForecastLabels = JsonConvert.SerializeObject(forecastLabels);
                    ViewBag.ForecastData = JsonConvert.SerializeObject(predictionResult.ForecastedCount);

                    System.Diagnostics.Debug.WriteLine($"✅ ViewBag.ForecastLabels atandı: {forecastLabels.Count} eleman");
                    System.Diagnostics.Debug.WriteLine($"✅ ViewBag.ForecastData atandı: {predictionResult.ForecastedCount.Length} eleman");
                    System.Diagnostics.Debug.WriteLine($"📝 İlk label: {forecastLabels.FirstOrDefault()}");
                    System.Diagnostics.Debug.WriteLine($"📝 İlk değer: {predictionResult.ForecastedCount.FirstOrDefault():F2}");
                    System.Diagnostics.Debug.WriteLine($"📋 JSON Preview (Labels): {ViewBag.ForecastLabels.ToString().Substring(0, Math.Min(100, ViewBag.ForecastLabels.ToString().Length))}...");
                    System.Diagnostics.Debug.WriteLine($"📋 JSON Preview (Data): {ViewBag.ForecastData.ToString().Substring(0, Math.Min(100, ViewBag.ForecastData.ToString().Length))}...");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌❌❌ Index Metodu - ML Tahmin Hatası ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");

                ViewBag.ForecastLabels = "[]";
                ViewBag.ForecastData = "[]";
            }

            System.Diagnostics.Debug.WriteLine("🏁 ===== ML TAHMİN BİTTİ ===== 🏁\n");

            return View(viewModel);
        }

        private AppointmentPrediction PredictNext90Days()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("===== PREDICT DEBUG BAŞLANGIÇ =====");

                MLContext mlContext = new MLContext(seed: 1);

                var allAppointments = context.Appointments
                    .Where(x => x.AppointmentDate != null)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"📊 Toplam Randevu: {allAppointments.Count}");

                if (allAppointments.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ HİÇ RANDEVU YOK! Varsayılan değerler döndürülüyor.");
                    System.Diagnostics.Debug.WriteLine("===== PREDICT DEBUG BİTİŞ =====\n");

                    return new AppointmentPrediction
                    {
                        ForecastedCount = Enumerable.Range(1, 90)
                            .Select(i => 5f + (float)(Math.Sin(i * 0.1) * 2))
                            .ToArray()
                    };
                }

                var groupedData = new Dictionary<DateTime, int>();

                foreach (var appointment in allAppointments)
                {
                    var dateOnly = appointment.AppointmentDate.Value.Date;
                    if (groupedData.ContainsKey(dateOnly))
                        groupedData[dateOnly]++;
                    else
                        groupedData[dateOnly] = 1;
                }

                var processedData = groupedData
                    .Select(kvp => new AppointmentData
                    {
                        Date = kvp.Key,
                        Count = (float)kvp.Value
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"📅 Farklı Gün Sayısı: {processedData.Count}");

                if (processedData.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("📋 İlk 5 gün:");
                    foreach (var item in processedData.Take(5))
                    {
                        System.Diagnostics.Debug.WriteLine($"  {item.Date:yyyy-MM-dd} -> {item.Count} randevu");
                    }
                }

                if (processedData.Count < 7)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Sadece {processedData.Count} gün var (minimum 7 gerekli)");
                    System.Diagnostics.Debug.WriteLine("📈 Basit tahmin yapılıyor (ortalama bazlı)");

                    float avgCount = processedData.Any() ? processedData.Average(x => x.Count) : 5f;

                    System.Diagnostics.Debug.WriteLine($"💡 Ortalama randevu: {avgCount:F2}");
                    System.Diagnostics.Debug.WriteLine("===== PREDICT DEBUG BİTİŞ =====\n");

                    return new AppointmentPrediction
                    {
                        ForecastedCount = Enumerable.Range(1, 90)
                            .Select(i => avgCount + (float)(Math.Sin(i * 0.1) * avgCount * 0.3))
                            .ToArray()
                    };
                }

                System.Diagnostics.Debug.WriteLine("🤖 ML.NET modeli eğitiliyor...");

                IDataView dataView = mlContext.Data.LoadFromEnumerable(processedData);

                int windowSize = Math.Min(Math.Max(processedData.Count / 3, 2), 7);
                System.Diagnostics.Debug.WriteLine($"🪟 Window Size: {windowSize}");

                var pipeline = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedCount",
                    inputColumnName: "Count",
                    windowSize: windowSize,
                    seriesLength: processedData.Count,
                    trainSize: processedData.Count,
                    horizon: 90,
                    confidenceLevel: 0.95f,
                    confidenceLowerBoundColumn: "LowerBound",
                    confidenceUpperBoundColumn: "UpperBound"
                );

                System.Diagnostics.Debug.WriteLine("⚙️ Model eğitiliyor...");
                var model = pipeline.Fit(dataView);

                System.Diagnostics.Debug.WriteLine("🔮 Tahmin yapılıyor...");
                var engine = model.CreateTimeSeriesEngine<AppointmentData, AppointmentPrediction>(mlContext);
                var result = engine.Predict();

                if (result == null || result.ForecastedCount == null || result.ForecastedCount.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Model tahmin döndürmedi!");
                    System.Diagnostics.Debug.WriteLine("===== PREDICT DEBUG BİTİŞ =====\n");

                    return new AppointmentPrediction
                    {
                        ForecastedCount = Enumerable.Range(1, 90).Select(i => 5f).ToArray()
                    };
                }

                System.Diagnostics.Debug.WriteLine($"✅ Tahmin tamamlandı! {result.ForecastedCount.Length} değer");
                System.Diagnostics.Debug.WriteLine("📊 İlk 5 tahmin değeri:");

                for (int i = 0; i < Math.Min(5, result.ForecastedCount.Length); i++)
                {
                    System.Diagnostics.Debug.WriteLine($"  Gün {i + 1}: {result.ForecastedCount[i]:F2} randevu");
                }

                for (int i = 0; i < result.ForecastedCount.Length; i++)
                {
                    if (result.ForecastedCount[i] < 0)
                    {
                        result.ForecastedCount[i] = 0;
                    }
                }

                System.Diagnostics.Debug.WriteLine("===== PREDICT DEBUG BİTİŞ =====\n");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌❌❌ PredictNext90Days HATA ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Hata Mesajı: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Hata Tipi: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner Stack: {ex.InnerException.StackTrace}");
                }

                System.Diagnostics.Debug.WriteLine("🔄 Fallback değerler döndürülüyor...");
                System.Diagnostics.Debug.WriteLine("===== PREDICT DEBUG BİTİŞ =====\n");

                return new AppointmentPrediction
                {
                    ForecastedCount = Enumerable.Range(1, 90)
                        .Select(i => 5f + (float)(Math.Sin(i * 0.1) * 2))
                        .ToArray()
                };
            }
        }

        private List<WeeklyAppointmentDto> GetWeeklyAppointmentsTrend()
        {
            var allAppointments = context.Appointments.ToList();

            if (allAppointments.Count == 0)
                return new List<WeeklyAppointmentDto>();

            var dayNameMap = new Dictionary<int, string>
            {
                { 1, "Pazartesi" },
                { 2, "Salı" },
                { 3, "Çarşamba" },
                { 4, "Perşembe" },
                { 5, "Cuma" },
                { 6, "Cumartesi" },
                { 0, "Pazar" }
            };

            var result = new List<WeeklyAppointmentDto>();
            var today = DateTime.Now.Date;
            var startDate = today;

            for (int i = 0; i < 7; i++)
            {
                DateTime dayDate = startDate.AddDays(i);

                var dayAppointments = allAppointments
                    .Where(a => ParseDate(a.AppointmentDate) == dayDate)
                    .ToList();

                int dayOfWeek = (int)dayDate.DayOfWeek;

                result.Add(new WeeklyAppointmentDto
                {
                    Day = dayNameMap[dayOfWeek],
                    Date = dayDate.ToString("dd.MM.yyyy"),
                    Count = dayAppointments.Count(),
                    Completed = dayAppointments.Count(a => ParseDate(a.AppointmentDate) < today),
                    Pending = dayAppointments.Count(a => ParseDate(a.AppointmentDate) >= today)
                });
            }

            return result;
        }

        private List<DepartmentAppointmentDto> GetDepartmentAppointments()
        {
            string[] colors = new[]
            {
                "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
                "#FF9F40", "#C9CBCF", "#FF6B6B", "#4ECDC4", "#45B7D1"
            };

            var appointments = context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.Department)
                .ToList();

            int colorIndex = 0;

            var result = appointments
                .Where(a => a.Doctor != null && a.Doctor.Department != null)
                .GroupBy(a => a.Doctor.Department.Name)
                .Select(g => new DepartmentAppointmentDto
                {
                    DepartmentName = g.Key,
                    Count = g.Count(),
                    Color = colors[colorIndex++ % colors.Length]
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return result;
        }
        private DateTime ParseDate(object dateObj)
        {
            if (dateObj == null)
                return DateTime.Now.Date;

            if (DateTime.TryParse(dateObj.ToString().Trim(), out DateTime result))
                return result.Date;

            return DateTime.Now.Date;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                context.Dispose();

            base.Dispose(disposing);
        }
    }

    public class WeeklyAppointmentDto
    {
        public string Day { get; set; }
        public string Date { get; set; }
        public int Count { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
    }

    public class DepartmentAppointmentDto
    {
        public string DepartmentName { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }
    }

    public class AppointmentData
    {
        public DateTime Date { get; set; }
        public float Count { get; set; }
    }

    public class AppointmentPrediction
    {
        public float[] ForecastedCount { get; set; }
    }

}