using System.Web.Optimization;

namespace OnlineShop
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/core").Include(
                     "~/Assets/Client/css/bootstrap-theme.css",
                     "~/Assets/Client/css/font-awesome.min.css",
                     "~/Assets/Client/css/bootstrap.css",
                     "~/Assets/Client/css/style.css",
                     "~/Assets/Client/css/slider.css",
                     "~/Assets/Client/css/jquery-ui.css"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/jscore").Include(
                      "~/Assets/Client/js/jquery-1.11.3.min.js",
                      "~/Assets/Client/js/jquery-ui.js",
                      "~/Assets/Client/js/bootstrap.min.js",
                      "~/assets/client/js/move-top.js",
                      "~/assets/client/js/easing.js",
                      "~/assets/client/js/startstop-slider.js"
                      ));
        }
    }
}
