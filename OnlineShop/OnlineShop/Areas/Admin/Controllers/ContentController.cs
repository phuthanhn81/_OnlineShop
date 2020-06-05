using Model.DAO;
using Model.Object;
using OnlineShop.Common;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {
        public ActionResult Index(string searchString, int page = 1)
        {
            ContentDAO CDAO = new ContentDAO();
            IEnumerable<Content> list = CDAO.ListAllPaging(searchString, page, 10);
            ViewBag.SearchString = searchString;
            return View(list);
        }

        [HttpPost]
        public JsonResult ChangeStatus(long ID)
        {
            bool result = new ContentDAO().ChangeStatus(ID);
            return Json(new
            {
                Status = result // return Json phải gõ đúng Attribute vì "Status" : value
            });
        }

        public ActionResult Delete(long ID)
        {
            bool flag = new ContentDAO().Delete(ID);
            if (!flag)
            {
                ModelState.AddModelError("", "Failed to Delete user");
            }
            return RedirectToAction("Index");
        }

        public void SetViewBag(long? selectedId = null)
        {
            // selectedId -> chọn sẵn theo ID (Edit), (Create) thì ko cần truyền đối số
            // ViewBag.CategoryID chỉ đúng property thì tự binding vào model
            CategoryDAO DAO = new CategoryDAO();
            ViewBag.CategoryID = new SelectList(DAO.ListAll(), "ID", "Name", selectedId);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Content content)
        {
            if (ModelState.IsValid)
            {
                UserLogin session = (UserLogin)Session[CommonConstants.USER_SESSION];
                content.CreatedBy = session.UserName;
                var culture = Session[CommonConstants.CurrentCulture];
                content.Language = culture.ToString();
                new ContentDAO().Create(content);
                return RedirectToAction("Index");
            }
            SetViewBag(); // ViewBag chỉ sài đc 1 lần
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpGet]
        public ActionResult Edit(long ID)
        {
            ContentDAO CDAO = new ContentDAO();
            Content content = CDAO.GetByID(ID);
            SetViewBag(content.CategoryID);
            return View(content);
        }

        [HttpPost, ValidateInput(false)] // ValidateInput cho phép nhập kí tự đặc biệt
        public ActionResult Edit(Content model)
        {
            if (ModelState.IsValid)
            {
                long ID = new ContentDAO().Edit(model);
                if (ID > 0)
                {
                    SetAlert("Succeed to update user", "success");
                    return RedirectToAction("Index", "Content");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update user");
                }
            }
            SetViewBag(model.CategoryID);
            return View();
        }
    }
}

// CurrentCulture