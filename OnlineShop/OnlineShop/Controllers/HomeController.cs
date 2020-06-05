using Model.DAO;
using Model.Object;
using OnlineShop.Common;
using OnlineShop.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Slides = new SlideDAO().ListAll();
            ViewBag.ListNewProducts = new ProductDAO().ListNewProduct(4);
            ViewBag.ListFeatureProducts = new ProductDAO().ListFeatureProduct(4);

            ViewBag.Title = ConfigurationManager.AppSettings["HomeTitle"];
            ViewBag.Keywords = ConfigurationManager.AppSettings["HomeKeywords"];
            ViewBag.Descriptions = ConfigurationManager.AppSettings["HomeDescriptions"];
            return View();
        }

        [ChildActionOnly] // chỉ để người khác gọi method này
        [OutputCache(Duration = 3600 * 24)] // lần đầu sẽ chạy từ lần sau cứ 1h thì nó mới đc phép chạy vào method này
        public ActionResult MainMenu()
        {
            // 1 2 do mình quy định
            List<Menu> list = new MenuDAO().ListByGroupId(1);
            return PartialView(list);
        }

        [ChildActionOnly] // chỉ để người khác gọi method này
        public ActionResult TopMenu()
        {
            // 1 2 do mình quy định
            List<Menu> list = new MenuDAO().ListByGroupId(2);
            return PartialView(list);
        }

        [ChildActionOnly] // chỉ để người khác gọi method này
        [OutputCache(Duration = 3600 * 24)]
        public ActionResult Footer()
        {
            Footer model = new FooterDAO().GetFooter();
            return PartialView(model);
        }

        [ChildActionOnly]
        public PartialViewResult HeaderCart()
        {
            var cart = Session[CommonConstants.CartSession];
            List<CartItem> list = new List<CartItem>();

            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return PartialView(list);
        }
    }
}