using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using TransporteFlexible.Helper;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Roles
{
    public partial class RolAMView : System.Web.UI.Page
    {
        private readonly RolManager _rolMgr;
        public RolAMView()
        {
            _rolMgr = new RolManager();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                if (SecurityHelper.CheckPermissions(2, Session[SV.Permissions.GD()])
                    && SecurityHelper.CheckPermissions(4, Session[SV.Permissions.GD()]))
                {
                    if (!IsPostBack)
                    {
                        PageLoad();
                    }
                }
            }
            else
            {
                Response.Redirect(ViewsEnum.SessionExpired.GD());
            }
        }

        private void PageLoad()
        {
            if (Request.QueryString.HasKeys() &&
                            int.TryParse(Request.QueryString.Get("id"), out int rolId))
            {
                Session[SV.EdittingRoleId.GD()] = rolId;
                if (rolId != 0)
                {
                    BuildEditView(rolId);
                }
                else
                {
                    BuildAddView();
                }
            }
        }

        private void BuildEditView(int RoleId)
        {
            FillDeallocatedPermissions(RoleId);
            Message msj = _rolMgr.GetRole(RoleId, (int)Session[SV.LoggedUserId.GD()]);
            if (msj.Resultado is Rol role)
            {
                FillAssignedPermissions(role.Permisos);
                RolNameTB.Text = role.Descripcion;
            }
        }

        private void BuildAddView()
        {
            TitleLBL.Text = "<i class='fas fa-id-card-alt'></i> Nuevo Rol";
            FillDeallocatedPermissions(0);
        }

        private void FillAssignedPermissions(List<Permiso> role)
        {
            Session[SV.AssignedPermissions.GD()] = role;
            AssignedListBox.DataSource = role;
            AssignedListBox.DataTextField = "Descripcion";
            AssignedListBox.DataValueField = "Id";
            AssignedListBox.DataBind();
        }

        protected void AsignLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                Message msj = null;
                if (!string.IsNullOrWhiteSpace(DeallocatedListBox.SelectedValue))
                {
                    List<Permiso> permisosAsignados =
                        Session[SV.AssignedPermissions.GD()] is List<Permiso> ?
                        Session[SV.AssignedPermissions.GD()] as List<Permiso>
                        : new List<Permiso>();

                    List<Permiso> permisosDesasignados =
                        Session[SV.DeallocatedPermissions.GD()] is List<Permiso> ?
                        Session[SV.DeallocatedPermissions.GD()] as List<Permiso>
                        : new List<Permiso>();

                    int permisoId = Convert.ToInt32(DeallocatedListBox.SelectedValue);
                    Permiso permiso = permisosDesasignados.Where(per => per.Id == permisoId).Single();

                    permisosAsignados.Add(permiso);
                    permisosDesasignados.Remove(permiso);

                    permisosAsignados =
                       permisosAsignados.OrderBy(x => x.Descripcion).ToList();
                    permisosDesasignados =
                        permisosDesasignados.OrderBy(x => x.Descripcion).ToList();

                    FillAssignedPermissions(permisosAsignados);
                    BindDeallocatedListBox(permisosDesasignados);


                    Session[SV.AssignedPermissions.GD()] = permisosAsignados;
                    Session[SV.DeallocatedPermissions.GD()] = permisosDesasignados;

                }
                else
                {
                    // Se debe seleccionar un permiso para que sea asignado 
                    msj = MessageFactory.GetMessage("MS65");
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                }

            }
            catch (Exception ex)
            {
                LogManager _bitacoraMgr = new LogManager();
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "AsignLinkButton_Click", "Se produjo una excepcion modificndo los permisos del Rol: "
                                      + Session[SV.EdittingRoleId.GD()], Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                }
                catch { }

                Message msj = MessageFactory.GettErrorMessage("ER03", ex);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        protected void UnsignLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                Message msj = null;
                if (!string.IsNullOrWhiteSpace(AssignedListBox.SelectedValue))
                {
                    List<Permiso> permisosAsignados =
                        Session[SV.AssignedPermissions.GD()] is List<Permiso> ?
                        Session[SV.AssignedPermissions.GD()] as List<Permiso>
                        : new List<Permiso>();

                    List<Permiso> permisosDesasignados =
                        Session[SV.DeallocatedPermissions.GD()] is List<Permiso> ?
                        Session[SV.DeallocatedPermissions.GD()] as List<Permiso>
                        : new List<Permiso>();

                    int permisoId = Convert.ToInt32(AssignedListBox.SelectedValue);
                    Permiso permiso = permisosAsignados.Where(per => per.Id == permisoId).Single();

                    PermisoManager _permisoMgr = new PermisoManager();

                    msj = _permisoMgr.ComprobarPermisoAsignadoAOtroUsuario(permiso,
                        Convert.ToInt32(Session[SV.EditingUserId.GD()]));

                    if (msj.CodigoMensaje != "OK")
                    {
                        MessageHelper.ProcessMessage(GetType(), msj, Page);
                        return;
                    }

                    permisosDesasignados.Add(permiso);
                    permisosAsignados.Remove(permiso);

                    permisosAsignados =
                       permisosAsignados.OrderBy(x => x.Descripcion).ToList();
                    permisosDesasignados =
                        permisosDesasignados.OrderBy(x => x.Descripcion).ToList();

                    FillAssignedPermissions(permisosAsignados);
                    BindDeallocatedListBox(permisosDesasignados);

                    Session[SV.AssignedPermissions.GD()] = permisosAsignados;
                    Session[SV.DeallocatedPermissions.GD()] = permisosDesasignados;
                }
                else
                {
                    // Se debe seleccionar un permiso para que sea asignado 
                    msj = MessageFactory.GetMessage("MS66");
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                }
            }
            catch (Exception ex)
            {
                LogManager _bitacoraMgr = new LogManager();
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Baja, "UnsignLinkButton_Click", "Se produjo una excepcion modificando los permisos del Rol: "
                   + Session[SV.EdittingRoleId.GD()], Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                }
                catch { }

                Message msj = MessageFactory.GettErrorMessage("ER03", ex);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        protected void SaveRolButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int roleId = Convert.ToInt32(Session[SV.EdittingRoleId.GD()]);
                List<Permiso> permissions = Session[SV.AssignedPermissions.GD()] as List<Permiso>;
                int loggedUserId =  Convert.ToInt32(Session[SV.LoggedUserId.GD()]);
                string roleName = RolNameTB.Text;
                Message msj;
                if (roleId == 0)
                    msj = _rolMgr.Insert(roleName, permissions, loggedUserId);
                else
                    msj = _rolMgr.Update(roleId, roleName, permissions, loggedUserId);

                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        protected void CancelRolButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewsEnum.Rol.GD());
        }

        private void FillDeallocatedPermissions(int RoleId)
        {
            RolManager _rolMgr = new RolManager();
            Message msj = _rolMgr.GetDeallocatedPermissions(RoleId, (int)Session[SV.LoggedUserId.GD()]);
            if (msj.Resultado is List<Permiso> DeallocatedPermissions)
            {
                BindDeallocatedListBox(DeallocatedPermissions);
            }
        }

        private void BindDeallocatedListBox(List<Permiso> DeallocatedPermissions)
        {
            Session[SV.DeallocatedPermissions.GD()] = DeallocatedPermissions;
            DeallocatedListBox.DataSource = DeallocatedPermissions;
            DeallocatedListBox.DataTextField = "Descripcion";
            DeallocatedListBox.DataValueField = "Id";
            DeallocatedListBox.DataBind();
        }
    }
}