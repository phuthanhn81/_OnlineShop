using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Common;
using Common;

namespace OnlineShop
{
    public class HasCredentialAttribute : AuthorizeAttribute
    {
        // [HasCredential(RoleID = "VIEW_USER")] khi ấn vào 1 method nó sẽ truyền VIEW_USER vào RoleID
        public string RoleID { set; get; }

        // chạy this 1st -> BaseController
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            UserLogin session = (UserLogin)HttpContext.Current.Session[CommonConstants.USER_SESSION];
            if (session == null) // chưa login
            {
                return false;
            }

            // vừa phân theo role + nhóm
            List<string> privilegeLevels = GetCredentialByLoggedInUser(session.UserName);
            if (privilegeLevels.Contains(RoleID) || session.GroupID == _CommonConstants.MOD_GROUP)
            {
                return true; // cho phép vào method
            }
            else
            {
                return false; // nhảy xuống HandleUnauthorizedRequest
            }
        }

        private List<string> GetCredentialByLoggedInUser(string userName)
        {
            List<string> credentials = (List<string>)HttpContext.Current.Session[CommonConstants.SESSION_CREDENTIALS];
            return credentials;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Areas/Admin/Views/Shared/401.cshtml"
            };
        }
    }
}