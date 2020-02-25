using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using TransporteFlexible.Helper;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios
{
    public partial class Usuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisosHelper.ValidarPermisos(16, Session["Permisos"]))
            {
                if (!IsPostBack)
                {
                    PopularTabla();
                }
            }
            else
            {
                Mensaje msj = Mensaje.CrearMensaje("MS39", false, true, null, "/");
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }

        private void PopularTabla()
        {
            try
            {
                UsuarioManager _usrMgr = new UsuarioManager();
                int idUser = string.IsNullOrWhiteSpace(_tbIdUsuario.Text) ? 0 : int.Parse(_tbIdUsuario.Text);
                string nombreUsuario = _tbNombreUsuario.Text;
                string dni = _tbDni.Text;

                Mensaje msj = _usrMgr.ObtenerUsuariosNegocioDesencriptados(idUser, nombreUsuario, dni);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}