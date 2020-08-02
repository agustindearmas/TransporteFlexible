using Common.Enums.Seguridad;
using Common.Extensions;
using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace TransporteFlexible
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciar la aplicación
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}