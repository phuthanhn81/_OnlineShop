using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Configuration;
using Model.Object;
using Model.DAO;
using Common;
using OnlineShop.Common;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        public ActionResult Index()
        {
            var cart = Session[CommonConstants.CartSession];
            List<CartItem> list = new List<CartItem>();

            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        public ActionResult AddItem(long productId, int quantity)
        {
            Product product = new ProductDAO().ViewDetail(productId);
            var cart = Session[CommonConstants.CartSession]; // tạo Session với key

            if (cart != null)
            {
                List<CartItem> list = (List<CartItem>)cart;
                if (list.Exists(x => x.Product.ID == productId)) // Exists product này rồi -> tăng số lượng
                {
                    foreach (CartItem item in list)
                    {
                        if (item.Product.ID == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    CartItem item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }

                Session[CommonConstants.CartSession] = list; // update giỏ hàng
            }
            else
            {
                CartItem item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                List<CartItem> list = new List<CartItem>();
                list.Add(item);

                Session[CommonConstants.CartSession] = list; // Session here là 1 list giỏ hàng
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Update(string cartModel)
        {
            List<CartItem> jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel); // convert json -> list (convert sai ngừng hàm luôn)
            List<CartItem> sessionCart = (List<CartItem>)Session[CommonConstants.CartSession]; // current list of session

            foreach (CartItem item in sessionCart) // current list could add, update, delete from user -> so sánh sessionCart với jsonCart
            {
                CartItem jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    if (jsonItem.Quantity > 0) // 1
                    {
                        item.Quantity = jsonItem.Quantity; // luôn đè data
                    }
                }
            }
            Session[CommonConstants.CartSession] = sessionCart; // refresh
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DeleteAll()
        {
            Session[CommonConstants.CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(long id)
        {
            List<CartItem> sessionCart = (List<CartItem>)Session[CommonConstants.CartSession];
            sessionCart.Remove(sessionCart.First(n => n.Product.ID == id));
            Session[CommonConstants.CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session[CommonConstants.CartSession];
            List<CartItem> list = new List<CartItem>();

            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult Payment(string shipName, string mobile, string address, string email)
        {
            try
            {
                #region add order
                Order order = new Order();
                order.CreatedDate = DateTime.Now;
                order.ShipAddress = address;
                order.ShipMobile = mobile;
                order.ShipName = shipName;
                order.ShipEmail = email;

                long id = new OrderDAO().Insert(order);
                #endregion

                List<CartItem> cart = (List<CartItem>)Session[CommonConstants.CartSession];
                OrderDetailDAO detailDao = new OrderDetailDAO();
                decimal total = 0;
                foreach (CartItem item in cart)
                {
                    #region add orderDetail (products)
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.ProductID = item.Product.ID;
                    orderDetail.OrderID = id; // order là bao gồm all orderDetail
                    orderDetail.Price = item.Product.Price.GetValueOrDefault(0) * item.Quantity;
                    orderDetail.Quantity = item.Quantity;
                    detailDao.Insert(orderDetail);
                    #endregion

                    total += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
                }
                #region nội dung order
                string content = System.IO.File.ReadAllText(Server.MapPath("~/assets/client/template/neworder.html"));
                content = content.Replace("{{CustomerName}}", shipName);
                content = content.Replace("{{Phone}}", mobile);
                content = content.Replace("{{Email}}", email);
                content = content.Replace("{{Address}}", address);
                content = content.Replace("{{Total}}", total.ToString("N0"));
                #endregion

                string toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                new MailHelper().SendMail(email, "Đơn hàng mới từ OnlineShop", content); // người order
                new MailHelper().SendMail(toEmail, "Đơn hàng mới từ OnlineShop", content); // người sử lí order
            }
            catch (Exception)
            {
                return Redirect("/loi-thanh-toan");
            }
            return Redirect("/hoan-thanh");
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}