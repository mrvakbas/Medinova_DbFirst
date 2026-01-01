using Medinova.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Medinova.Controllers
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            string userName = httpContext.User.Identity.Name;

            using (var context = new MedinovaContext())
            {
                var user = context.Users.Include("Roles").FirstOrDefault(u => u.UserName == userName);

                if (user == null || user.Roles == null || user.Roles.Count == 0)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(Roles))
                {
                    string[] requiredRoles = Roles.Split(',').Select(r => r.Trim()).ToArray();
                    bool hasRole = user.Roles.Any(r => requiredRoles.Contains(r.RoleName));

                    if (!hasRole)
                    {
                        return false;
                    }
                }

                    string[] userRoles = user.Roles.Select(r => r.RoleName).ToArray();
                IIdentity identity = new GenericIdentity(userName);
                IPrincipal principal = new GenericPrincipal(identity, userRoles);
                httpContext.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;

                return true;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "area", "" },
                        { "controller", "Home" },
                        { "action", "AccessDenied" }
                    }
                );
            }
        }
    }
}