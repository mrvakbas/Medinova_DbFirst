using Medinova.Models;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Medinova
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalFilters.Filters.Add(new AuthorizeAttribute());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Eðer kullanýcý authenticated ise
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string userName = HttpContext.Current.User.Identity.Name;

                using (var context = new MedinovaContext())
                {
                    // Veritabanýndan kullanýcýyý ve rollerini getir
                    var user = context.Users.FirstOrDefault(u => u.UserName == userName);

                    if (user != null && user.Roles != null && user.Roles.Count > 0)
                    {
                        // Rolleri diziye çevir
                        string[] roles = user.Roles.Select(r => r.RoleName).ToArray();

                        // GenericPrincipal oluþtur ve HttpContext'e ata
                        IIdentity identity = new GenericIdentity(userName);
                        IPrincipal principal = new GenericPrincipal(identity, roles);
                        HttpContext.Current.User = principal;
                        System.Threading.Thread.CurrentPrincipal = principal;
                    }
                }
            }
        }
    }
}
