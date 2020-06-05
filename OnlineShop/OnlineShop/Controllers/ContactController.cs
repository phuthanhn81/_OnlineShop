using Model.DAO;
using Model.Object;
using System;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Index()
        {
            Contact model = new ContactDAO().GetActiveContact();
            return View(model);
        }

        [HttpPost]
        public JsonResult Send(string name, string mobile, string address, string email, string content)
        {
            Feedback feedback = new Feedback();
            feedback.Name = name;
            feedback.Email = email;
            feedback.CreatedDate = DateTime.Now;
            feedback.Phone = mobile;
            feedback.Content = content;
            feedback.Address = address;

            int id = new ContactDAO().InsertFeedBack(feedback);
            if (id > 0)
            {
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }
    }
}