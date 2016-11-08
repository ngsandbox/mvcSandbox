using System.Web.Mvc;
using System.Web.Routing;

namespace MySite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Localized",
                url: "{language}-{culture}/{controller}/{action}/{id}",
                defaults: new
                {
                    area = "",
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    language = "en",
                    culture = "us",
                }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    area = "",
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}
