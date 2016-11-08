using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace MySite.Filters
{
    public class InternationalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string language = (string)filterContext.RouteData.Values["language"];
            string culture = (string)filterContext.RouteData.Values["culture"];
            language = language ?? "en";
            culture = culture ?? "us";
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo($"{language}-{culture}");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo($"{language}-{culture}");
        }
    }
}