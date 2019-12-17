using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad
{
    public partial class ValidaEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
        }

        protected void btnValidarCuenta_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            UsuarioManager _usuarioMgr = new UsuarioManager();
            Mensaje msj = _usuarioMgr.ValidarUsuario(id);
            MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
        }
    }
}