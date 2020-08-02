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

namespace TransporteFlexible.Views.Seguridad.Roles
{
    public partial class RolesView : GridViewExtensions<Rol>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                if (SecurityHelper.CheckPermissions(1, Session[SV.Permissions.GD()]))
                {
                    if (!IsPostBack)
                    {
                        FillDLL();
                        GetGridViewDataSource();
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

        private void FillDLL()
        {
            RolManager _rMgr = new RolManager();
            List<Rol> roles = _rMgr.Retrieve(null);
            RolesDLL.DataSource = roles;
            RolesDLL.DataTextField = "Descripcion";
            RolesDLL.DataValueField = "Id";
            RolesDLL.DataBind();
            ListItem li = new ListItem
            {
                Text = "",
                Value = "0"
            };
            RolesDLL.Items.Add(li);
            RolesDLL.SelectedValue = "0";
        }

        protected void ExportXMLButton_Click(object sender, EventArgs e)
        {
            XMLManager _xmlMgr = new XMLManager();
            DataTable dt = Session["Roles"] as DataTable;
            Message msj = _xmlMgr.ExportDataTableToXMLFile(dt, "rol", "roles.xml");
            MessageHelper.ProcessMessage(GetType(), msj, Page);
        }

        protected void RolesGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            RolesGridView.PageIndex = e.NewPageIndex;
            GetGridViewDataSource();
        }

        protected void NewRolButton_Click(object sender, EventArgs e)
        {
            // EL cero indica q se queire dar de alta un rol
            GoToEditRoleView(0);
        }

        protected void RolesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int index = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = RolesGridView.Rows[index];

            string roleId = row.Cells[0].Text;

            Session[SV.EdittingRoleId.GD()] = roleId;

            int roleIdInt = Convert.ToInt32(roleId);

            switch (commandName)
            {
                case "E":
                    GoToEditRoleView(roleIdInt);
                    break;
                case "D":
                    DeleteRole(roleIdInt);
                    break;

                default:
                    break;
            }
        }

        private void DeleteRole(int roleId)
        {
            List<int> essentialRoles = new List<int> { 1, 2, 3, 4, 5 };
            if (essentialRoles.Contains(roleId))
            {
                //El rol seleccionado es esencial, no puede ser eliminado ni modificado
                Message msj = MessageFactory.GetMessage("MS81");
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                RolManager _rMgr = new RolManager();
                Message msj = _rMgr.DeleteRol(roleId, (int)Session[SV.LoggedUserId.GD()]);
                if (msj.CodigoMensaje == "MS18")
                {
                    GetGridViewDataSource();
                }
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }

        }

        private void GoToEditRoleView(int RoleId)
        {
            List<int> essentialRoles = new List<int> { 1, 2, 3, 4, 5 };
            if (essentialRoles.Contains(RoleId))
            {
                //El rol seleccionado es esencial, no puede ser eliminado ni modificado
                Message msj = MessageFactory.GetMessage("MS81");
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                string urlRedirect = string.Concat(ViewsEnum.RolAM.GD(),
                    "?id=", RoleId);
                Response.Redirect(urlRedirect);
            }
        }

        internal override void LoadDataGridView(List<Rol> entities)
        {
            RolesGridView.AllowPaging = true;
            DataTable dt = ConvertToDataTable(entities);
            Session["Roles"] = dt;
            RolesGridView.DataSource = dt;
            RolesGridView.DataBind();
            ActualizationDateLBL.Text = DateTime.Now.ToString();
        }

        private void GetGridViewDataSource()
        {
            RolManager _rolMgr = new RolManager();
            int roleId = Convert.ToInt32(RolesDLL.SelectedValue);
            Message msj;
            List<Rol> roles = new List<Rol>();

            if (roleId == 0)
                msj = _rolMgr.GetRoles((int)Session[SV.LoggedUserId.GD()]);
            else
                msj = _rolMgr.GetRole(roleId, (int)Session[SV.LoggedUserId.GD()]);



            if (msj.CodigoMensaje != "OK")
            {
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                if (msj.Resultado is Rol rol)
                    roles.Add(rol);

                if (msj.Resultado is List<Rol>)
                    roles = msj.Resultado as List<Rol>;

                LoadDataGridView(roles);
            }
        }

        internal override bool NoNecesaryFieldsInGridView(PropertyDescriptor prop)
        {
            return (prop.Name == "DVH");
        }

        protected void RolesDLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetGridViewDataSource();
        }
    }
}