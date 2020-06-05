using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}