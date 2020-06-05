using Model.DAO;
using Model.Object;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ContentController : Controller
    {
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            int totalRecord = 0;
            IEnumerable<Content> model = new ContentDAO().ListAllPaging(page, pageSize, ref totalRecord);

            int totalPage = (totalRecord % pageSize != 0) ? (totalRecord / pageSize) + 1 : (totalRecord / pageSize);

            ViewBag.MaxPage = 3; // có 3 ô page display
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            return View(model);
        }

        public ActionResult Detail(long id, string metatitle)
        {
            Content model = new ContentDAO().GetByID(id); // detail content
            ViewBag.Tags = new ContentDAO().ListTag(id); // all tags đi kèm
            return View(model);
        }

        public ActionResult Tag(string tagId, int page = 1, int pageSize = 5)
        {
            int totalRecord = 0;
            IEnumerable<Content> model = new ContentDAO().ListAllByTag(tagId, page, pageSize, ref totalRecord);

            int totalPage = (totalRecord % pageSize != 0) ? (totalRecord / pageSize) + 1 : (totalRecord / pageSize);

            ViewBag.Tag = new ContentDAO().GetTag(tagId);
            ViewBag.MaxPage = 3; // có 3 ô page display
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            return View(model);
        }
    }
}