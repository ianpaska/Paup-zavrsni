using GameShop3.Models;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.Web.Optimization;

namespace GameShop3
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // admin
            using (var context = new ApplicationDbContext())
            {
                IdentitySeed.SeedAdmin(context);
            }
        }
    }
}
