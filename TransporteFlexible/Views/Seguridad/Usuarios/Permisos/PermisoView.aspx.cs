using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using TransporteFlexible.Helper.GridView;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios.Permisos
{
    public partial class PermisoView : System.Web.UI.Page
    {
        public int UserId { get; set; }
        protected void Page_Load(object sender, EventArgs e)
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
                    string item = "";
                    foreach (var pa in permisosAsignados)
                    {
                        item = string.Concat(pa.Id, "-", pa.Descripcion);
                        lbxAsignados.Items.Add(item);
                    }
                    foreach (var pd in permisosDesasignados)
                    {
                        item = string.Concat(pd.Id, "-", pd.Descripcion);
                        lbxDesasignados.Items.Add(item);
                    }
                    
                }

            }
            catch (Exception e)
            {
                // revisar este flujo
                throw e;
            }
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {

        }
    }
}