using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using TransporteFlexible.Helper;
using TransporteFlexible.Helper.GridView;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios
{
    public partial class UsuarioView : GridViewExtensions<User>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisosHelper.ValidarPermisos(16, Session[SV.Permisos.GD()]))
            {
                if (!IsPostBack)
                {
                    PopularTabla();
                }
            }
            else
            {
                Message msj = MessageFactory.GetMessage("MS39", "/");
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }

        private void PopularTabla()
        {
            try
            {
                UserManager _usrMgr = new UserManager();
                int idUser = string.IsNullOrWhiteSpace(_tbIdUsuario.Text) ? 0 : int.Parse(_tbIdUsuario.Text);
                string nombreUsuario = _tbNombreUsuario.Text;
                string dni = _tbDni.Text;

                Message msj = _usrMgr.ObtenerUsuariosNegocioDesencriptados(idUser, nombreUsuario, dni);

                if (msj.CodigoMensaje != "OK")
                {
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                }
                else
                {
                    List<User> usuarios = (List<User>)msj.Resultado;
                    BuildDataGridView(usuarios);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected void _usuariosGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int index = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = _usuariosGridView.Rows[index];

            string userId = row.Cells[0].Text;

            Session[SV.UsuarioModificado.GD()] = userId;

            int userIdInt = Convert.ToInt32(userId);

            switch (commandName)
            {
                case "_verPermisos":
                    VerPermisos(userIdInt);
                    break;
                case "_modificar":
                    VerEditarUsuario(userIdInt);
                    break;
                case "_eliminar":
                    break;
                case "_habdes":
                    ModificarHabilitado(userIdInt);
                    break;
                default:
                    break;
            }
        }

        private void ModificarHabilitado(int id)
        {
            UserManager _usuarioMgr = new UserManager();
            Message msj = _usuarioMgr.EnableDisableUser(id);
            MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            PopularTabla();
        }

        private void VerPermisos(int id)
        {
            string urlRedirect = string.Concat(ViewsEnum.Permiso.GD(),
                "?id=", id);
            Response.Redirect(urlRedirect);
        }

        private void VerEditarUsuario(int id)
        {
            string urlRedirect = string.Concat(ViewsEnum.UsuarioAM.GD(),
                "?id=", id);
            Response.Redirect(urlRedirect);
        }

        internal override bool EsCampoNoNecesario(PropertyDescriptor prop)
        {
            return (prop.Name == "FechaDesde" || prop.Name == "FechaHasta" ||
                     prop.Name == "UsuarioCreacion" || prop.Name == "UsuarioModificacion" ||
                     prop.Name == "FechaModificacion");
        }

        internal override void BuildDataGridView(List<User> entities)
        {
            _usuariosGridView.AllowPaging = true;
            DataTable dt = ConvertToDataTable(entities);
            _usuariosGridView.DataSource = dt;
            _usuariosGridView.DataBind();
            _lblFechaActualizacion.Text = DateTime.Now.ToString();
        }

        protected void btnFiltrarUsuarios_Click(object sender, EventArgs e)
        {
            PopularTabla();
        }
    }
}