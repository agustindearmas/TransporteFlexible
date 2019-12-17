using System.Web.Optimization;

namespace TransporteFlexible
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {  
            //// El orden es muy importante para el funcionamiento de estos archivos ya que tienen dependencias explícitas
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/MsAjax/MicrosoftAjaxWebForms.js"
                    ));

            // Use la versión de desarrollo de Modernizr para desarrollar y aprender. Luego, cuando esté listo
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));
        }
    }
}