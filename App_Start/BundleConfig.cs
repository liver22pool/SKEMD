using System.Web;
using System.Web.Optimization;

namespace SKEMD_WWW
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/tabs").Include(
                        "~/Scripts/addclasskillclass.js",
                        "~/Scripts/addcss.js",
                        "~/Scripts/attachevent.js",
                        "~/Scripts/tabtastic.js"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство построения на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/Site.css",
                "~/Content/demos.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/jquery.prettyCheckable.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                "~/Content/Site.css",
                "~/Content/login.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));


            bundles.Add(new StyleBundle("~/js/jq").Include(
                "~/Scripts/jquery-1.9.1.js",
                "~/Scripts/jquery.ui.core.js",
                "~/Scripts/jquery.ui.widget.js",
                "~/Scripts/jquery.ui.accordion.js",
                "~/Scripts/jquery.mCustomScrollbar.concat.min.js",
                "~/Scripts/jquery.prettyCheckable.js"));

            bundles.Add(new StyleBundle("~/js/skemd").Include(
                "~/Scripts/skemd.core.js",
                "~/Scripts/skemd.func.js"));

            //datepicker
            bundles.Add(new StyleBundle("~/css/datepicker").Include(
            "~/Content/themes/datepicker/jquery.ui.datepicker.css"));

            bundles.Add(new StyleBundle("~/js/datepicker").Include(
                   "~/Scripts/jquery.ui.datepicker.js",
                   "~/Scripts/DatePicker_Main.js"
                   ));

            bundles.Add(new StyleBundle("~/js/scroll").Include(
                // "~/Scripts/jquery.mCustomScrollbar.concat.min.js",
                   "~/Scripts/mCustomScrollbar.js",
                   "~/Scripts/Scroll_Main.js"
                   ));
        }
    }
}