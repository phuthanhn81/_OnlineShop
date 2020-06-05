using BotDetect.Web.Mvc;
using Facebook;
using Model.DAO;
using Model.Object;
using OnlineShop.Common;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace OnlineShop.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidation("code", "captcha", "Mã xác nhận không đúng!")]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                UserDAO UDAO = new UserDAO();

                if (UDAO.CheckUserName(model.UserName))
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại"); // thoát if + kèm lỗi chung
                }
                else if (UDAO.CheckEmail(model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                }
                else
                {
                    // vẫn sẽ check Validation 1 lần nữa khi add vào CSDL do code first
                    User user = new User();
                    user.UserName = model.UserName;
                    user.Name = model.Name;
                    user.Password = Encryptor.MD5Hash(model.Password);
                    user.Phone = model.Phone;
                    user.Email = model.Email;
                    user.Address = model.Address;
                    user.CreatedDate = DateTime.Now;
                    user.Status = true;
                    if (!string.IsNullOrEmpty(model.ProvinceID))
                    {
                        user.ProvinceID = int.Parse(model.ProvinceID);
                    }
                    if (!string.IsNullOrEmpty(model.DistrictID))
                    {
                        user.DistrictID = int.Parse(model.DistrictID);
                    }

                    long result = UDAO.Insert(user);
                    if (result > 0)
                    {
                        ViewBag.Success = "Đăng ký thành công";
                        RegisterModel new_model = new RegisterModel();
                        ModelState.Clear();
                        return View(new_model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng ký không thành công.");
                    }
                }
            }
            return View(model); // binding lại data vào view này
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel Model)
        {
            if (ModelState.IsValid) // ko hợp lệ báo lỗi đề cập ở Object
            {
                UserDAO UDAO = new UserDAO();
                int result = UDAO.Login(Model.UserName, Encryptor.MD5Hash(Model.Password)); // return Object hay hơn ?
                if (result == 1)
                {
                    User user = UDAO.Get(Model.UserName);

                    // lưu user login này lại (data user này làm gì, mua gì, ... gửi lên server)
                    UserLogin UserSession = new UserLogin();
                    UserSession.UserName = user.UserName;
                    UserSession.UserID = user.ID;

                    // Session dùng để lưu data client lên server
                    Session.Add(CommonConstants.USER_SESSION, UserSession);

                    // login thành công đưa về trang chủ home
                    return Redirect("/");
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
            return View(Model); // sai show lại (view mới nhưng object mất) 
        }

        public ActionResult Logout()
        {
            Session[CommonConstants.USER_SESSION] = null;
            return Redirect("/");
        }

        private Uri RedirectUri
        {
            get
            {
                UriBuilder uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        public ActionResult LoginFacebook()
        {
            FacebookClient fb = new FacebookClient();
            Uri loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            FacebookClient fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // lấy thêm -> me?fields=first_name,middle_name,last_name,id,email,birthday,phone
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;

                User user = new User();
                user.Email = email;
                user.UserName = email;
                user.Password = Encryptor.MD5Hash(email);
                user.Status = true;
                user.Name = firstname + " " + middlename + " " + lastname;
                user.CreatedDate = DateTime.Now;
                long resultInsert = new UserDAO().InsertForFacebook(user);
                if (resultInsert > 0)
                {
                    UserLogin userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                }
            }
            return Redirect("/");
        }

        public JsonResult LoadProvince()
        {
            XDocument xmlDoc = XDocument.Load(Server.MapPath(@"~/assets/client/data/Provinces_Data.xml"));
            IEnumerable<XElement> xElements = xmlDoc.Element("Root").Elements("Item").Where(x => x.Attribute("type").Value == "province");
            List<ProvinceModel> list = new List<ProvinceModel>();
            ProvinceModel province = null;
            foreach (XElement item in xElements)
            {
                province = new ProvinceModel();
                province.ID = int.Parse(item.Attribute("id").Value);
                province.Name = item.Attribute("value").Value;
                list.Add(province);
            }
            return Json(new
            {
                data = list,
                status = true
            });
        }

        public JsonResult LoadDistrict(int ProvinceID)
        {
            XDocument xmlDoc = XDocument.Load(Server.MapPath(@"~/assets/client/data/Provinces_Data.xml"));
            XElement xElement = xmlDoc.Element("Root").Elements("Item")
                .Single(x => x.Attribute("type").Value == "province" && int.Parse(x.Attribute("id").Value) == ProvinceID);
            List<DistrictModel> list = new List<DistrictModel>();
            DistrictModel district = null;
            foreach (XElement item in xElement.Elements("Item").Where(x => x.Attribute("type").Value == "district"))
            {
                district = new DistrictModel();
                district.ID = int.Parse(item.Attribute("id").Value);
                district.Name = item.Attribute("value").Value;
                district.ProvinceID = int.Parse(xElement.Attribute("id").Value); // all district này thuộc quận nào
                list.Add(district);
            }
            return Json(new
            {
                data = list,
                status = true
            });
        }
    }
}
// developers.facebook.com test thì vào lại cái trang này chứ ko thì ko sài đc fb login
// xElement trừ ban đầu phải duyệt Root thì từ lần sau xElement sẽ duyệt ở giữa các thẻ xml