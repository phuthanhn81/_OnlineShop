using OnlineShop.Common;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // Default nó sẽ nhảy vào file Resources ko có .
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session[CommonConstants.CurrentCulture] != null) // qua url khác nó sẽ vô đây
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session[CommonConstants.CurrentCulture].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session[CommonConstants.CurrentCulture].ToString());
            }
            else
            {
                Session[CommonConstants.CurrentCulture] = "vi"; // Session là 1 string
                Thread.CurrentThread.CurrentCulture = new CultureInfo("vi");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi");
            }
        }

        // changing culture
        public ActionResult ChangeCulture(string ddlCulture, string returnUrl)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(ddlCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ddlCulture);
            Session[CommonConstants.CurrentCulture] = ddlCulture;

            return Redirect(returnUrl); // url hiện tại
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Session.Add(CommonConstants.USER_SESSION, UserSession); -> phần login
            // check user đã login hay chưa (gõ thẳng URL Home/Index mà ko cần qua login)
            // Session có name USER_SESSION (login - mình quy định) có object hay chưa
            UserLogin UserSession = (UserLogin)Session[CommonConstants.USER_SESSION];
            if (UserSession == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", Area = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }

        protected void SetAlert(string message, string type)
        {
            // Tương tự ViewBag, TempData cũng truyền data ra View và ngược lại but tồn tại ngắn, can named lại
            // giống Session tồn tại toàn Web sẽ chạy nếu đc gọi ra HTML or Code
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success"; // class Bootstrap
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
    }
}