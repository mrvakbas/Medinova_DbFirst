using Medinova.Controllers;
using Medinova.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class LogController : Controller
    {
        MedinovaContext _context = new MedinovaContext();

        public ActionResult Index()
        {
            var logs = _context.Logs
                .OrderByDescending(l => l.LogDate)
                .ToList();

            return View(logs);
        }
        [HttpPost]
        public JsonResult DeleteLog(int id)
        {
            try
            {
                var log = _context.Logs.Find(id);
                if (log != null)
                {
                    _context.Logs.Remove(log);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Log kaydı bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteAllLogs()
        {
            try
            {
                var allLogs = _context.Logs.ToList();
                _context.Logs.RemoveRange(allLogs);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}