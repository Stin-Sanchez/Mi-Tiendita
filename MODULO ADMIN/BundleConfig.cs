using System.Web;
using System.Web.Optimization;

namespace MODULO_ADMIN
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // jQuery primero (requerido por sb-admin-2 y datatables)
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.4.1.min.js"));

            // Bootstrap 4 + easing + sb-admin-2 (orden importa)
            bundles.Add(new Bundle("~/bundles/sbadmin").Include(
                "~/Scripts/bootstrap4.bundle.min.js",
                "~/Scripts/jquery.easing.min.js",
                "~/Scripts/sb-admin-2.min.js"));

            // Complementos: FontAwesome, DataTables, SweetAlert, otros
            bundles.Add(new Bundle("~/bundles/complementos").Include(
                "~/Scripts/fontawesome/all.min.js",
                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/DataTables/dataTables.responsive.js",
                "~/Scripts/loadingoverlay/loadingoverlay.min.js",
                "~/Scripts/sweetalert.min.js",
                "~/Scripts/jquery-ui-1.14.1.min.js"));

            // Chart.js (dashboard)
            bundles.Add(new Bundle("~/bundles/charts").Include(
                "~/Scripts/Chart.min.js"));

            // CSS: SB Admin 2 base + tema indigo + DataTables + extras
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/sb-admin-2.min.css",
                "~/Content/admin-indigo.css",
                "~/Content/DataTables/css/jquery.dataTables.css",
                "~/Content/DataTables/css/responsive.dataTables.css",
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/sweetalert.css"));
        }
    }
}
