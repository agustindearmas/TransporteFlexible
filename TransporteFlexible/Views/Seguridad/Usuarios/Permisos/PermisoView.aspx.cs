using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios.Permisos
{
    public partial class PermisoView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageLoad();
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
            try
            {
                UsuarioManager _usuarioMgr = new UsuarioManager();
                Mensaje msj = _usuarioMgr.ObtenerUsuariosNegocioDesencriptados(userId, null, null);
                if (msj.CodigoMensaje != "OK")
                {
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                }
                else
                {
                    List<Usuario> usuarios = (List<Usuario>)msj.Resultado;
                    lblUser.Text = usuarios.Single().NombreUsuario;
                }

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private void PopularPermisos(int userId)
        {
            try
            {
                PermisoManager _permisoMgr = new PermisoManager();
                Mensaje msj = _permisoMgr.ObtenerPermisosPorUsuarioId(userId);
                Mensaje msj1 = _permisoMgr.ObtenerPermisosDesasignados(userId);

                if (msj.CodigoMensaje != "OK" && msj1.CodigoMensaje != "OK")
                {
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
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

                    Session[SV.PermisosAsignados.GD()] = permisosAsignados;
                    Session[SV.PermisosDesasignados.GD()] = permisosDesasignados;
                }
            }
            catch (Exception ex)
            {
                BitacoraManager _bitacoraMgr = new BitacoraManager();
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Modificando Permisos del Usuario", "Se produjo una excepcion modificndo los permisos del Usuario: "
                   + Session[SV.UsuarioModificado.GD()], Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));
                }
                catch { }
                
               
                Mensaje msj = MessageFactory.CrearMensajeError("ER03", ex);
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }

        protected void lbAsignar_Click(object sender, EventArgs e)
        {
            try
            {
                Mensaje msj = null;
                if (!string.IsNullOrWhiteSpace(lbxDesasignados.SelectedValue))
                {
                    List<Permiso> permisosAsignados =
                        Session[SV.PermisosAsignados.GD()] is List<Permiso> ?
                        Session[SV.PermisosAsignados.GD()] as List<Permiso>
                        : new List<Permiso>();

                    List<Permiso> permisosDesasignados = (List<Permiso>)Session[SV.PermisosDesasignados.GD()];

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
                  
                    
                    Session[SV.PermisosAsignados.GD()] = permisosAsignados;
                    Session[SV.PermisosDesasignados.GD()] = permisosDesasignados;

                }
                else
                {
                    // Se debe seleccionar un permiso para que sea asignado 
                    msj = MessageFactory.CrearMensaje("MS65");
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                }

            }
            catch (Exception ex)
            {
                BitacoraManager _bitacoraMgr = new BitacoraManager();
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Modificando Permisos del Usuario", "Se produjo una excepcion modificndo los permisos del Usuario: "
                                      + Session[SV.UsuarioModificado.GD()], Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));
                }
                catch {}
              
                Mensaje msj = MessageFactory.CrearMensajeError("ER03", ex);
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }

        }

        protected void lbDesasignar_Click(object sender, EventArgs e)
        {
            try
            {
                Mensaje msj = null;
                if (!string.IsNullOrWhiteSpace(lbxAsignados.SelectedValue))
                {
                    List<Permiso> permisosAsignados = (List<Permiso>)Session[SV.PermisosAsignados.GD()];
                    List<Permiso> permisosDesasignados = (List<Permiso>)Session[SV.PermisosDesasignados.GD()];

                    int permisoId = Convert.ToInt32(lbxAsignados.SelectedValue);
                    Permiso permiso = permisosAsignados.Where(per => per.Id == permisoId).Single();

                    PermisoManager _permisoMgr = new PermisoManager();

                    msj = _permisoMgr.ComprobarPermisoAsignadoAOtroUsuario(permiso, 
                        Convert.ToInt32(Session[SV.UsuarioModificado.GD()]));

                    if (msj.CodigoMensaje != "OK")
                    {
                        MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
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

                    Session[SV.PermisosAsignados.GD()] = permisosAsignados;
                    Session[SV.PermisosDesasignados.GD()] = permisosDesasignados;
                }
                else
                {
                    // Se debe seleccionar un permiso para que sea asignado 
                    msj = MessageFactory.CrearMensaje("MS66");
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                }
            }
            catch (Exception ex)
            {
                BitacoraManager _bitacoraMgr = new BitacoraManager();
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Modificando Permisos del Usuario", "Se produjo una excepcion modificndo los permisos del Usuario: "
                   + Session[SV.UsuarioModificado.GD()], Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));
                }
                catch{}
               
                Mensaje msj = MessageFactory.CrearMensajeError("ER03", ex);
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Mensaje msj = null;
                UsuarioManager _usuarioMgr = new UsuarioManager();
                if (ValidarFormulario())
                {
                    int userId = Convert.ToInt32(Session[SV.UsuarioModificado.GD()]);
                    List<Permiso> permisosAsignadosSession =
                        (List<Permiso>)Session[SV.PermisosAsignados.GD()];
                    msj = _usuarioMgr.AsignarPermisos(userId, permisosAsignadosSession, Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);

                }
                // ELSE FALLO LA VALIDACION NO HAC 
            }
            catch (Exception ex)
            {
                BitacoraManager _bitacoraMgr = new BitacoraManager();
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Modificando Permisos del Usuario", "Se produjo una excepcion modificando los permisos del Usuario: "
                 + Session[SV.UsuarioModificado.GD()], Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));
                }
                catch{}
             
                Mensaje msj = MessageFactory.CrearMensajeError("ER03", ex);
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
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
                int userId = Convert.ToInt32(Session[SV.UsuarioModificado.GD()]);
                Mensaje msj = _permisoMgr.ObtenerPermisosPorUsuarioId(userId);

                if (msj.CodigoMensaje != "OK")
                {
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
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

                    if (Session[SV.PermisosAsignados.GD()] is List<Permiso>)
                    {
                        permisosAsignadosSession = (Session[SV.PermisosAsignados.GD()] as List<Permiso>).Select(x => x.Id).ToList();
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