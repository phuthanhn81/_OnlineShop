using Model.DAO;
using Model.Object;
using OnlineShop.Common;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        public ActionResult Index(string SearchString, int Page = 1)
        {
            CategoryDAO CDAO = new CategoryDAO();
            IEnumerable<Category> list = CDAO.GetList(SearchString, Page, 10);

            // bug -> value same thì phân trang sai -> can't same
            ViewBag.SearchString = SearchString;
            return View(list);
        }

        [HttpPost]
        public JsonResult ChangeStatus(long ID)
        {
            bool result = new CategoryDAO().ChangeStatus(ID);
            return Json(new
            {
                Status = result // return Json phải gõ đúng Attribute vì "Status" : value
            });
        }

        public ActionResult Delete(int ID)
        {
            bool flag = new CategoryDAO().Delete(ID);
            if (!flag)
            {
                ModelState.AddModelError("", "Failed to Delete user");
            }
            //View 1 đối số String là View mới -> object có thể bị null
            //RedirectToAction là return Method
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var currentCulture = Session[CommonConstants.CurrentCulture];
                category.Language = currentCulture.ToString();
                long id = new CategoryDAO().Insert(category);
                if (id > 0)
                {
                    SetAlert("Succeed to add new user", "success");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", StaticResources.Resources.InsertCategoryFailed);
                }
            }
            return View(category);
        }

        public ActionResult Edit(long ID)
        {
            return View(new CategoryDAO().Get(ID));
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            bool result = new CategoryDAO().Update(category);
            if (result)
            {
                SetAlert("Succeed to update user", "success");
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update user");
            }
            return View();
        }
    }
}

// CurrentCulture