using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using TransporteFlexible.Helper;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad
{
    public partial class BaseDeDatos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Permisos"] == null || !PermisosHelper.ValidarPermisos(42, Session["Permisos"]))
            {
                // REBOTARLO
            }
        }

        protected void btnGenerarRespaldo_Click(object sender, EventArgs e)
        {
            if (Session["Permisos"] != null && PermisosHelper.ValidarPermisos(12, Session["Permisos"]))
            {   
                BDManager _bdMgr = new BDManager();
                Mensaje msj = _bdMgr.GenerarBKP(txtNombreRespaldo.Text, Convert.ToInt32(Session["UsuarioLogueado"]));
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                Response.Redirect("/");
            }
        }

        protected void btnRestaurar_Click(object sender, EventArgs e)
        {
            if (Session["Permisos"] != null && PermisosHelper.ValidarPermisos(12, Session["Permisos"]))
            {
                BDManager _bdMgr = new BDManager();
                Mensaje msj = _bdMgr.MontarBKP(fuRestore.FileName, Convert.ToInt32(Session["UsuarioLogueado"]));
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                Response.Redirect("/");
            }
        }

        protected void btnRecalDV_Click(object sender, EventArgs e)
        {
            if (Session["Permisos"] != null && PermisosHelper.ValidarPermisos(12, Session["Permisos"]))
            {
                TablaDVVManager _dVerificadorManager = new TablaDVVManager();
                Mensaje msj = _dVerificadorManager.RecalcularDigitosVerificadores();
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                Response.Redirect("/");
            }
        }
    }
}