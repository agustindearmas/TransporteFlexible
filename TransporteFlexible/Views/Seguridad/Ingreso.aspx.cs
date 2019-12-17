using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Web.UI;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad
{
    public partial class Ingreso : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void BtnIngresar_Click(Object sender, EventArgs e)
        {
            IngresoManager ingresoMgr = new IngresoManager();
            Mensaje mensaje = ingresoMgr.Ingresar(_tbNombreUsuario.Text, _tbContraseña.Text);
            ProcesarMensajeDeIngreso(mensaje);
        }

        private void ProcesarMensajeDeIngreso(Mensaje msj)
        {
            if (msj.CodigoMensaje == "OK")
            {
                // redirigir a la pagina principal
                // Session
                Sesion ses = (Sesion)msj.Resultado;
                Session["UsuarioLogueado"] = ses.IdUsuario;
                Session["NombreUsuario"] = ses.NombreUsuario;
                Session["Permisos"] = ses.Permisos;
                Response.Redirect("~/Views/Shared/Bienvenida.aspx");
            }
            else
            {
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }
    }
}