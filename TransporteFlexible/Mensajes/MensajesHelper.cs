using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Shared;
using System;
using System.Threading;
using System.Web.UI;
using TransporteFlexible.Enums;

namespace TransporteFlexible.Mensajes
{
    public static class MensajesHelper
    {
        private static string ObtenerMensaje(string codMensaje)
        {
            if (Enum.TryParse(codMensaje, out MensajesEnum resultado))
            {
                return resultado.GetDescription();
            }
            else
            {
                return "";
            }
        }

        public static void ProcesarMensajeGenerico(Type type, Mensaje msj, Page page)
        {
            if (msj.EsError && msj.RutaRedireccion == Common.Enums.Seguridad.ViewsEnum.Error.GetDescription())
            {
                System.Web.HttpContext.Current.Session["Error"] = ObtenerMensaje(msj.CodigoMensaje);
                System.Web.HttpContext.Current.Response.Redirect(msj.RutaRedireccion);
            }

            if (msj.MuestraMensaje)
                MostrarMensaje(msj.CodigoMensaje, page, type, msj.RutaRedireccion);

            if (!string.IsNullOrWhiteSpace(msj.RutaRedireccion) && !msj.MuestraMensaje)            
                System.Web.HttpContext.Current.Response.Redirect(msj.RutaRedireccion);
        }

        private static void MostrarMensaje(string codigoMensaje, Page page, Type type, string redireccion)
        {
            string title = "¡Atención!";
            string body = ObtenerMensaje(codigoMensaje);            
            page.ClientScript.RegisterStartupScript(type, "Popup", "ShowPopup('" + title + "', '" + body + "', '" + redireccion + "');", true);
        }
    }
}