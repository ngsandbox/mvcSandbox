using System.Web.Mvc;

namespace MySite.Controllers
{
    public class PreviewController : Controller
    {
        public ActionResult GridCards()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
    }
}
