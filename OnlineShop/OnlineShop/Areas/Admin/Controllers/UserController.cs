using Model.DAO;
using Model.Object;
using OnlineShop.Common;
using System.Web.Mvc;
using System.Collections.Generic;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        [HasCredential(RoleID = "VIEW_USER")]
        public ActionResult Index(string SearchString, int Page = 1)
        {
            UserDAO UDAO = new UserDAO();
            IEnumerable<User> list = UDAO.GetList(SearchString, Page, 10);
            // Get 10 dựa trên Page but vẫn đếm all để biết có bao nhiêu Page lên View

            ViewBag.SearchString = SearchString; // save lại để View nó phân trang theo keyword
            return View(list);
        }

        [HasCredential(RoleID = "ADD_USER")]
        public ActionResult Post(User Entity)
        {
            if (ModelState.IsValid)
            {
                UserDAO UDAO = new UserDAO();
                Entity.Password = Encryptor.MD5Hash(Entity.Password);
                long flag = UDAO.Post(Entity);
                if (flag == 1)
                {
                    SetAlert("Succeed to add new user", "success");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add new user");
                }
            }
            return View();
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult Edit(int ID)
        {
            //Index sẽ run here (View Get Data)
            User user = new UserDAO().Get(ID);
            //tự Mapping (PasswordFor nó sẽ ko Mapping qua)
            return View(user);
        }

        // chung 1 Method -> chung 1 View -> chung 1 model -> giữ nguyên value của model
        [HttpPost]
        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult Edit(User Entity)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Entity.Password)) //muốn đổi Password | còn ko giữ nguyên Password
                {
                    Entity.Password = Encryptor.MD5Hash(Entity.Password);
                }

                UserDAO UDAO = new UserDAO();
                bool result = UDAO.Update(Entity);
                if (result)
                {
                    SetAlert("Succeed to update user", "Success");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update user");
                }
            }
            //View 0 đối số là View hiện tại -> object hiện tại -> giữ nguyên value
            return View();
        }

        [HasCredential(RoleID = "DELETE_USER")]
        public ActionResult Delete(int ID)
        {
            bool flag = new UserDAO().Delete(ID);
            if (!flag)
            {
                ModelState.AddModelError("", "Failed to Delete user");
            }
            // View 1 đối số String là View mới -> object có thể bị null
            // RedirectToAction là return Method
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ChangeStatus(long ID)
        {
            bool result = new UserDAO().ChangeStatus(ID);
            return Json(new
            {
                Status = result // return Json phải gõ đúng Attribute vì "Status" : value
            });
        }
    }
}