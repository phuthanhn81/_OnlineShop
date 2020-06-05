using Model.DAO;
using Model.Object;
using OnlineShop.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        public ActionResult Index(string searchString, int page = 1)
        {
            ProductDAO PDAO = new ProductDAO();
            IEnumerable<Product> list = PDAO.ListAllPaging(searchString, page, 10);
            ViewBag.SearchString = searchString;
            return View(list);
        }

        public void SetViewBag(long? selectedId = null)
        {
            ProductCategoryDAO PCDAO = new ProductCategoryDAO();
            ViewBag.CategoryID = new SelectList(PCDAO.ListAll(), "ID", "Name", selectedId);
        }

        [HttpPost]
        public JsonResult ChangeStatus(long ID)
        {
            bool result = new ProductDAO().ChangeStatus(ID);
            return Json(new
            {
                Status = result // return Json phải gõ đúng Attribute vì "Status" : value
            });
        }

        public ActionResult Delete(long ID)
        {
            bool flag = new ProductDAO().Delete(ID);
            if (!flag)
            {
                ModelState.AddModelError("", "Failed to Delete user");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost, ValidateInput(false)] // ValidateInput cho phép nhập kí tự đặc biệt
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                UserLogin session = (UserLogin)Session[CommonConstants.USER_SESSION];
                product.CreatedBy = session.UserName;
                bool result = new ProductDAO().Create(product);
                if (result)
                {
                    SetAlert("Succeed to add new user", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add new user");
                }
            }
            SetViewBag(); // ViewBag chỉ sài đc 1 lần
            return View();
        }

        [HttpGet]
        public ActionResult Edit(long ID)
        {
            ProductDAO PDAO = new ProductDAO();
            Product product = PDAO.GetByID(ID);
            SetViewBag(product.CategoryID);
            return View(product);
        }

        [HttpPost, ValidateInput(false)] // ValidateInput cho phép nhập kí tự đặc biệt
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                long ID = new ProductDAO().Edit(product);
                if (ID > 0)
                {
                    SetAlert("Succeed to update user", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update user");
                }
            }
            SetViewBag(product.CategoryID);
            return View();
        }

        public JsonResult LoadImages(long id)
        {
            Product product = new ProductDAO().ViewDetail(id);
            XElement xImages = XElement.Parse(product.MoreImages); // convert - > XML
            List<string> listImagesReturn = new List<string>();

            foreach (XElement element in xImages.Elements()) // 0 đối số -> lấy all child
            {
                listImagesReturn.Add(element.Value); // mid XElement là Value
            }
            return Json(new
            {
                data = listImagesReturn
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveImages(long id, string images)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<string> listImages = serializer.Deserialize<List<string>>(images);
            XElement xElement = new XElement("Images"); // tạo root xml

            foreach (string item in listImages)
            {
                string subStringItem = item.Substring(21); // chỉ nên lấy "/ + folder chứa images" thôi
                xElement.Add(new XElement("Image", subStringItem)); // ở giữa là Value | XAttribute("name" , "value")
            }
            ProductDAO PDAO = new ProductDAO();
            try
            {
                PDAO.UpdateImages(id, xElement.ToString()); // xml là 1 string
                return Json(new
                {
                    status = true
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    status = false
                });
            }
        }
    }
}