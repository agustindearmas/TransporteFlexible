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

namespace TransporteFlexible.Views.Seguridad.Usuarios.Permisos
{
    public partial class PermisoView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                if (SecurityHelper.CheckPermissions(9, Session[SV.Permissions.GD()]))
                {
                    if (!IsPostBack)
                    {
                        PageLoad();
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

        private void PageLoad()
        {
            if (Request.QueryString.HasKeys() &&
                            int.TryParse(Request.QueryString.Get("id"), out int userId))
            {
                PopularNombreUsuario(userId);
                PopularPermisos(userId);
            }
        }

        private void PopularNombreUsuario(int userId)
        {
            UserManager _userMgr = new UserManager();
            Message msj = _userMgr.GetUserById(userId);

            if (msj.CodigoMensaje != "OK")
            {
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                if (msj.Resultado is User user)
                {
                    lblUser.Text = user.NombreUsuario;
                }
            }
        }

        private void PopularPermisos(int userId)
        {
            try
            {
                PermisoManager _permisoMgr = new PermisoManager();
                Message msj = _permisoMgr.ObtenerPermisosPorUsuarioId(userId);
                Message msj1 = _permisoMgr.ObtenerPermisosDesasignados(userId);

                if (msj.CodigoMensaje != "OK" && msj1.CodigoMensaje != "OK")
                {
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                }
                else
                {
                    List<Permiso> permisosAsignados = (List<Permiso>)msj.Resultado;
                    List<Permiso> permisosDesasignados = (List<Permiso>)msj1.Resultado;

                    if (permisosAsignados != null)
                    {
                        permisosAsignados =
                        permisosAsignados.OrderBy(x => x.Descripcion).ToList();
                    }
                    

                    permisosDesasignados =
                        permisosDesasignados.OrderBy(x => x.Descripcion).ToList();

                    BindAsignados(permisosAsignados);
                    BindDesasignados(permisosDesasignados);

                    Session[SV.AssignedPermissions.GD()] = permisosAsignados;
                    Session[SV.DeallocatedPermissions.GD()] = permisosDesasignados;
                }
            }
            catch (Exception ex)
            {
                LogManager _bitacoraMgr = new LogManager();
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Modificando Permisos del User", "Se produjo una excepcion modificndo los permisos del User: "
                   + Session[SV.EditingUserId.GD()], Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                }
                catch { }
                
               
                Message msj = MessageFactory.GettErrorMessage("ER03", ex);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        protected void lbAsignar_Click(object sender, EventArgs e)
        {
            try
            {
                Message msj = null;
                if (!string.IsNullOrWhiteSpace(lbxDesasignados.SelectedValue))
                {
                    List<Permiso> permisosAsignados =
                        Session[SV.AssignedPermissions.GD()] is List<Permiso> ?
                        Session[SV.AssignedPermissions.GD()] as List<Permiso>
                        : new List<Permiso>();

                    List<Permiso> permisosDesasignados = (List<Permiso>)Session[SV.DeallocatedPermissions.GD()];

                    int permisoId = Convert.ToInt32(lbxDesasignados.SelectedValue);
                    Permiso permiso = permisosDesasignados.Where(per => per.Id == permisoId).Single();

                    permisosAsignados.Add(permiso);
                    permisosDesasignados.Remove(permiso);

                    permisosAsignados =
                       permisosAsignados.OrderBy(x => x.Descripcion).ToList();
                    permisosDesasignados =
                        permisosDesasignados.OrderBy(x => x.Descripcion).ToList();

                    BindAsignados(permisosAsignados);
                    BindDesasignados(permisosDesasignados);
                  
                    
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "Modificando Permisos del User", "Se produjo una excepcion modificndo los permisos del User: "
                                      + Session[SV.EditingUserId.GD()], Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                }
                catch {}
              
                Message msj = MessageFactory.GettErrorMessage("ER03", ex);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        protected void lbDesasignar_Click(object sender, EventArgs e)
        {
            try
            {
                Message msj = null;
                if (!string.IsNullOrWhiteSpace(lbxAsignados.SelectedValue))
                {
                    List<Permiso> permisosAsignados = (List<Permiso>)Session[SV.AssignedPermissions.GD()];
                    List<Permiso> permisosDesasignados = (List<Permiso>)Session[SV.DeallocatedPermissions.GD()];

                    int permisoId = Convert.ToInt32(lbxAsignados.SelectedValue);
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

                    BindAsignados(permisosAsignados);
                    BindDesasignados(permisosDesasignados);

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
                    _bitacoraMgr.Create(LogCriticality.Alta, "Modificando Permisos del User", "Se produjo una excepcion modificndo los permisos del User: "
                   + Session[SV.EditingUserId.GD()], Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                }
                catch{}
               
                Message msj = MessageFactory.GettErrorMessage("ER03", ex);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Message msj = null;
                UserManager _usuarioMgr = new UserManager();
                if (ValidarFormulario())
                {
                    int userId = Convert.ToInt32(Session[SV.EditingUserId.GD()]);
                    List<Permiso> permisosAsignadosSession =
                        (List<Permiso>)Session[SV.AssignedPermissions.GD()];
                    msj = _usuarioMgr.AsignPermits(userId, permisosAsignadosSession, Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                    MessageHelper.ProcessMessage(GetType(), msj, Page);

                }
                // ELSE FALLO LA VALIDACION NO HAC 
            }
            catch (Exception ex)
            {
                LogManager _bitacoraMgr = new LogManager();
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Modificando Permisos del User", "Se produjo una excepcion modificando los permisos del User: "
                 + Session[SV.EditingUserId.GD()], Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                }
                catch{}
             
                Message msj = MessageFactory.GettErrorMessage("ER03", ex);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            PageLoad();
        }

        private void BindDesasignados(List<Permiso> permisosDesasignados)
        {
            lbxDesasignados.DataSource = permisosDesasignados;
            lbxDesasignados.DataTextField = "Descripcion";
            lbxDesasignados.DataValueField = "Id";
            lbxDesasignados.DataBind();
        }

        private void BindAsignados(List<Permiso> permisosAsignados)
        {
            lbxAsignados.DataSource = permisosAsignados;
            lbxAsignados.DataTextField = "Descripcion";
            lbxAsignados.DataValueField = "Id";
            lbxAsignados.DataBind();
        }

        private bool ValidarFormulario()
        {
            try
            {
                PermisoManager _permisoMgr = new PermisoManager();
                int userId = Convert.ToInt32(Session[SV.EditingUserId.GD()]);
                Message msj = _permisoMgr.ObtenerPermisosPorUsuarioId(userId);

                if (msj.CodigoMensaje != "OK")
                {
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                    return false;
                }
                else
                {
                    List<int> permisosAsignados = new List<int>();
                    List<int> permisosAsignadosSession = new List<int>();

                    if (msj.Resultado is List<Permiso>)
                    {
                        permisosAsignados = (msj.Resultado as List<Permiso>).Select(x => x.Id).ToList();
                    }

                    if (Session[SV.AssignedPermissions.GD()] is List<Permiso>)
                    {
                        permisosAsignadosSession = (Session[SV.AssignedPermissions.GD()] as List<Permiso>).Select(x => x.Id).ToList();
                    }

                    var bandera = new HashSet<int>(permisosAsignados).SetEquals(permisosAsignadosSession);
                    return !bandera;
                    
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}