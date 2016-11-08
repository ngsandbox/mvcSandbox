using System.Threading;
using System.Web.Mvc;

namespace MySite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return View();
        }
    }
}
