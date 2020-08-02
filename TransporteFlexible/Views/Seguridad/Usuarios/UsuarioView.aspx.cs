using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using Negocio.Managers.Shared;
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
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                if (SecurityHelper.CheckPermissions(5, Session[SV.Permissions.GD()]))
                {
                    if (!IsPostBack)
                    {
                        PopulateGridView();
                    }
                }
                else
                {
                    Message msj = MessageFactory.GetMessage("MS39", "/");
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                }
            }
            else
            {
                Response.Redirect(ViewsEnum.SessionExpired.GD());
            } 
        }

        private void PopulateGridView()
        {
            try
            {
                UserManager _usrMgr = new UserManager();
                string nombreUsuario = _tbNombreUsuario.Text;
                string dni = _tbDni.Text;

                Message msj = _usrMgr.ObtenerUsuariosNegocioDesencriptados(BajaCBX.Checked, nombreUsuario, dni);

                if (msj.CodigoMensaje != "OK")
                {
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                }
                else
                {
                    List<User> usuarios = (List<User>)msj.Resultado;
                    LoadDataGridView(usuarios);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ActivateOrDeactivateUser(int id)
        {
            if (Page.IsValid)
            {
                UserManager _usuarioMgr = new UserManager();
                Message msj = _usuarioMgr.ActivateOrDeactivateUser(id);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
                PopulateGridView();
            }
        }

        private void GoToPermitsView(int id)
        {
            string urlRedirect = string.Concat(ViewsEnum.Permiso.GD(),
                "?id=", id);
            Response.Redirect(urlRedirect);
        }

        private void GoToAddEditUserView(int id)
        {
            string urlRedirect = string.Concat(ViewsEnum.UsuarioAM.GD(),
                "?id=", id);
            Response.Redirect(urlRedirect);
        }

        internal override bool NoNecesaryFieldsInGridView(PropertyDescriptor prop)
        {
            return (prop.Name == "FechaDesde" || prop.Name == "FechaHasta" ||
                     prop.Name == "UsuarioCreacion" || prop.Name == "UsuarioModificacion" ||
                     prop.Name == "FechaModificacion");
        }

        internal override void LoadDataGridView(List<User> entities)
        {
            UserGV.AllowPaging = true;
            DataTable dt = ConvertToDataTable(entities);
            Session["Users"] = dt;
            UserGV.DataSource = dt;
            UserGV.DataBind();
            _lblFechaActualizacion.Text = DateTime.Now.ToString();
        }

        protected void UserGV_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int index = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = UserGV.Rows[index];

            string userId = row.Cells[0].Text;

            Session[SV.EditingUserId.GD()] = userId;

            int userIdInt = Convert.ToInt32(userId);

            switch (commandName)
            {
                case "Permits":
                    GoToPermitsView(userIdInt);
                    break;
                case "Modify":
                    GoToAddEditUserView(userIdInt);
                    break;
                case "DownOrUp":
                    DownOrUpUser(userIdInt);
                    break;
                case "DeActive":
                    ActivateOrDeactivateUser(userIdInt);
                    break;
                default:
                    break;
            }
        }

        private void DownOrUpUser(int userId)
        {
            if (Page.IsValid)
            {
                UserManager _userMgr = new UserManager();
                Message msj = _userMgr.DownOrUpUser(userId, (int)Session[SV.LoggedUserId.GD()]);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
                PopulateGridView();
            }
        }

        protected void SearchUserBTN_Click(object sender, EventArgs e)
        {
            PopulateGridView();
        }

        protected void NewUserButton_Click(object sender, EventArgs e)
        {
            // el cero indica que se quiere crear un usuario nuevo.
            GoToAddEditUserView(0);
        }

        protected void ExportXMLButton_Click(object sender, EventArgs e)
        {
            XMLManager _xmlMgr = new XMLManager();
            DataTable dt = Session["Users"] as DataTable;
            Message msj = _xmlMgr.ExportDataTableToXMLFile(dt, "usuario", "Usuarios.xml");
            MessageHelper.ProcessMessage(GetType(), msj, Page);
        }
    }
}