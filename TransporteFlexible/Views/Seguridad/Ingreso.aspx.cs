using Common.Enums.Seguridad;
using Common.Extensions;
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
            LogInManager ingresoMgr = new LogInManager();
            Message mensaje = ingresoMgr.LogIn(_tbNombreUsuario.Text, _tbContraseña.Text);
            ProcesarMensajeDeIngreso(mensaje);
        }

        private void ProcesarMensajeDeIngreso(Message msj)
        {
            if (msj.CodigoMensaje == "OK")
            {
                // redirigir a la pagina principal
                // Session
                Sesion ses = (Sesion)msj.Resultado;
                Session[SV.UsuarioLogueado.GD()] = ses.IdUsuario;
                Session[SV.NombreUsuario.GD()] = ses.NombreUsuario;
                Session[SV.Permisos.GD()] = ses.Permisos;
                ;
                Response.Redirect("~" + ViewsEnum.Bienvenida.GD());
            }
            else
            {
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }
    }
}