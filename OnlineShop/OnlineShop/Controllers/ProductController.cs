using Model.DAO;
using Model.Object;
using Model.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult ProductCategory()
        {
            List<ProductCategory> list = new ProductCategoryDAO().ListAll();
            return PartialView(list);
        }

        public ActionResult Category(long ID, int page = 1, int pageSize = 5)
        {
            #region done
            ProductCategory category = new CategoryDAO().ViewDetail(ID);
            ViewBag.Category = category;

            int totalRecord = 0;
            List<ProductViewModel> model = new ProductDAO().ListByCategoryId(ID, ref totalRecord, page, pageSize);

            int totalPage = (totalRecord % pageSize != 0) ? (totalRecord / pageSize) + 1 : (totalRecord / pageSize);

            ViewBag.MaxPage = 3; // có 3 ô page display
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            #endregion
            return View(model);
        }

        // varyByParam="id" 1st vào after id phải khác nhau thì mới đc vào (cái nào chưa có mới vào server còn ko lấy từ cache)
        // Location = OutputCacheLocation.? -> cách thằng nào
        [OutputCache(CacheProfile = "Cache1DayForProduct")]
        public ActionResult Detail(long ID)
        {
            Product product = new ProductDAO().ViewDetail(ID);
            ViewBag.Category = new ProductCategoryDAO().ViewDetail(product.CategoryID.Value);
            ViewBag.RelatedProducts = new ProductDAO().ListRelatedProducts(ID);
            return View(product);
        }

        public JsonResult ListName(string q) // q = keyword
        {
            List<string> data = new ProductDAO().ListName(q);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string keyword, int page = 1, int pageSize = 5)
        {
            #region done
            int totalRecord = 0;
            List<ProductViewModel> model = new ProductDAO().Search(keyword, ref totalRecord, page, pageSize);

            int totalPage = (totalRecord % pageSize != 0) ? (totalRecord / pageSize) + 1 : (totalRecord / pageSize);

            ViewBag.Keyword = keyword;
            ViewBag.MaxPage = 3; // có 3 ô page display
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            #endregion
            return View(model);
        }
    }
}