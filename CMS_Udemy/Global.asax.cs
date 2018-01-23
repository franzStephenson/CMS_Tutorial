using CMS_Udemy.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CMS_Udemy
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            //Check if user is logged in
            if(User == null)
            {
                return;
            }

            //Get username
            string username = Context.User.Identity.Name;

            //declare array of roles
            string[] roles = null;

            using(db DB = new db())
            {
                //Populate roles
                UsersDTO dto = DB.Users.FirstOrDefault(x => x.UserName == username);

                roles = DB.UserRoles.Where(x => x.UserId == dto.Id).Select(x => x.Role.Name).ToArray();
            }

            //Build IPrincipal object
            IIdentity userIdentity = new GenericIdentity(username);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            //Update context.user
            Context.User = newUserObj;
        }
    }
}
