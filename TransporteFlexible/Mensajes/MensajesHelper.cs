using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Shared;
using System;
using System.Web.UI;
using TransporteFlexible.Enums;

namespace TransporteFlexible.Mensajes
{
    public static class MensajesHelper
    {
        public static void ProcesarMensajeGenerico(Type type, Mensaje msj, Page page)
        {
            if (msj.EsError && msj.RutaRedireccion == ViewsEnum.Error.GD())
            {
                System.Web.HttpContext.Current.Session["Error"] = ObtenerMensaje(msj.CodigoMensaje);

                if (msj.Resultado is Exception)
                {
                    Exception a = msj.Resultado as Exception;
                    System.Web.HttpContext.Current.Session["Error"] = 
                        string.Concat(System.Web.HttpContext.Current.Session["Error"], " Excepción: ", a.Message);
                }

                System.Web.HttpContext.Current.Response.Redirect(msj.RutaRedireccion);
            }

            if (msj.MuestraMensaje)
                MostrarMensaje(msj, page, type);

            if (!string.IsNullOrWhiteSpace(msj.RutaRedireccion) && !msj.MuestraMensaje)
                System.Web.HttpContext.Current.Response.Redirect(msj.RutaRedireccion);
        }

        private static string ObtenerMensaje(string codMensaje)
        {
            if (Enum.TryParse(codMensaje, out MensajesEnum resultado))
            {

                return resultado.GD();
            }
            else
            {
                return "";
            }
        }

        private static void MostrarMensaje(Mensaje mensaje, Page page, Type type)
        {
            string title = "¡Atención!";
            string body = ObtenerMensaje(mensaje.CodigoMensaje);

            if (!string.IsNullOrWhiteSpace(mensaje.Concatena))
            {
                body = string.Format(body, mensaje.Concatena);
            }

            page.ClientScript.RegisterStartupScript(type, "Popup", "ShowPopup('" + title + "', '" + body + "', '" + mensaje.RutaRedireccion + "');", true);
        }
    }
}