using Model.DAO;
using Model.Object;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Common;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        public LoginController()
        {
            // khởi tạo ở Runtime
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(LoginModel Model)
        {
            if (ModelState.IsValid) // ko hợp lệ báo lỗi đề cập ở Object
            {
                UserDAO UDAO = new UserDAO();
                int result = UDAO.Login(Model.UserName, Encryptor.MD5Hash(Model.Password), true); // return Object hay hơn ?
                if (result == 1)
                {
                    User user = UDAO.Get(Model.UserName);

                    // lưu user login này lại (data user này làm gì, mua gì, ... gửi lên server)
                    UserLogin UserSession = new UserLogin();
                    UserSession.UserName = user.UserName;
                    UserSession.UserID = user.ID;
                    UserSession.GroupID = user.GroupID;
                    List<string> listCredentials = UDAO.GetListCredential(Model.UserName);

                    // Session dùng để lưu data client lên server
                    Session.Add(CommonConstants.USER_SESSION, UserSession); // object
                    Session.Add(CommonConstants.SESSION_CREDENTIALS, listCredentials); // list

                    // login thành công đưa về trang chủ home
                    return RedirectToAction("Index", "Home");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("Wrong", "You don't have permission to access");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("Wrong", "Your Account Has Been Disabled");
                }
                else
                {
                    ModelState.AddModelError("Wrong", "Wrong Username Or Password");
                }
            }
            return View("Index"); // sai show lại (view mới nhưng object mất = reset value) 
        }
    }
}