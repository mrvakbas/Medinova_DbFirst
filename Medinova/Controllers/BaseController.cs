using Medinova.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Controllers
{
    public class BaseController : Controller
    {

        protected MedinovaContext context = new MedinovaContext();

        protected void LogAction(string action, string details)
        {
            var currentUserName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.UserName == currentUserName);

            if (user != null)
            {
                Log newLog = new Log
                {
                    UserId = user.UserId,
                    Action = action,
                    Details = details,
                    LogDate = DateTime.Now
                };
                context.Logs.Add(newLog);
                context.SaveChanges();
            }
        }
    }
}