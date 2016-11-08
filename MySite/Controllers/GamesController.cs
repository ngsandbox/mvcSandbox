using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MySite.Core;
using MySite.Models;

namespace MySite.Controllers
{
    public class GamesController : Controller
    {
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult GridMemory()
        {
            return PartialView();
        }

        public ActionResult Mandala()
        {
            return PartialView();
        }

        public async Task<JsonResult> GetCards(int limit = 10)
        {
            var path = "/Content/Images/memory/";
            return Json(await GetCardsAsync(limit, path), JsonRequestBehavior.AllowGet);
        }

        private async static Task<List<CardInfo>> GetCardsAsync(int limit, string path)
        {
            var filesInfo = await GameFactory.GetFilesAsync(path);
            var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            List<CardInfo> result = GameFactory.ConverToCards(limit, path, lang, filesInfo);
            return result;
        }
    }
}