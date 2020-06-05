using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*botdetect}", new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            // Default phải luôn ở dưới cùng | param phải giống 100% thì mới mapping qua bên controller đc (here ID) ko quan trọng params
            routes.MapRoute(
              name: "Product Category",
              url: "san-pham/{metatitle_test}-{ID}", // if url ntn thì nó sẽ dẫn đến controller này -> action này
              defaults: new { controller = "Product", action = "Category", id = UrlParameter.Optional },
              namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
             name: "Product Detail",
             url: "chi-tiet/{metatitle}-{ID}",
             defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional },
             namespaces: new[] { "OnlineShop.Controllers" }
            );

            // "url chỉ đc tính đến trước dấu ?" | sau ? là params -> params same 100% thì mới mapping đc
            routes.MapRoute(
                name: "Add Cart",
                url: "them-gio-hang",
                defaults: new { controller = "Cart", action = "AddItem", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
                 name: "Cart",
                 url: "gio-hang",
                 defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                 namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
                name: "Payment",
                url: "thanh-toan",
                defaults: new { controller = "Cart", action = "Payment", id = UrlParameter.Optional }, // Default là HttpGet
                namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
                name: "Payment Success",
                url: "hoan-thanh",
                defaults: new { controller = "Cart", action = "Success", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
                  name: "Contact",
                  url: "lien-he",
                  defaults: new { controller = "Contact", action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
                   name: "Register",
                   url: "dang-ky",
                   defaults: new { controller = "User", action = "Register", id = UrlParameter.Optional }, // Default là HttpGet
                   namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
                name: "Login",
                url: "dang-nhap",
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
               name: "Search",
               url: "tim-kiem",
               defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional },
               namespaces: new[] { "OnlineShop.Controllers" }
            );

            routes.MapRoute(
               name: "News",
               url: "tin-tuc",
               defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "OnlineShop.Controllers" }
            );

            // so sánh url này với params action cái nào có {} mà giống name thì gán value vào đó
            routes.MapRoute(
                 name: "Content Detail",
                 url: "tin-tuc/{metatitle}-{id}",
                 defaults: new { controller = "Content", action = "Detail", id = UrlParameter.Optional },
                 namespaces: new[] { "OnlineShop.Controllers" }
             );

            // nó so sánh url trên tag a chứ ko phải value -> giống -> nhảy vào C/A -> {} -> so sánh params rồi gán value
            routes.MapRoute(
                 name: "Tags",
                 url: "tag/{tagId}",
                 defaults: new { controller = "Content", action = "Tag", id = UrlParameter.Optional },
                 namespaces: new[] { "OnlineShop.Controllers" }
             );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" } // có 2 Home chỉ rõ namespace
            );
        }
    }
}
