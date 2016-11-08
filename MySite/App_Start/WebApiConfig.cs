using System.Web.Http;
using Microsoft.Owin.Security.OAuth;

namespace MySite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "LocalizedApi",
                routeTemplate: "{language}-{culture}/api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    language = "en",
                    culture = "us",
                }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
