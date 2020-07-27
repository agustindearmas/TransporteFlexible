using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
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
            if (!PermisosHelper.ValidarPermisos(42, Session[SV.Permisos.GD()]))
            {
                RebotarUsuarioSinPermisos("/");
            }
        }

        protected void btnGenerarRespaldo_Click(object sender, EventArgs e)
        {
            if (Session[SV.Permisos.GD()] != null && PermisosHelper.ValidarPermisos(12, Session[SV.Permisos.GD()]))
            {   
                BDManager _bdMgr = new BDManager();
                Message msj = _bdMgr.GenerarBKP(txtNombreRespaldo.Text, Convert.ToInt32(SV.UsuarioLogueado.GD()));
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                RebotarUsuarioSinPermisos();
            }
        }

        protected void btnRestaurar_Click(object sender, EventArgs e)
        {
            if (Session[SV.Permisos.GD()] != null && PermisosHelper.ValidarPermisos(13, Session[SV.Permisos.GD()]))
            {
                BDManager _bdMgr = new BDManager();
                Message msj = _bdMgr.MontarBKP(fuRestore.FileName, Convert.ToInt32(SV.UsuarioLogueado.GD()));
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                RebotarUsuarioSinPermisos();
            }
        }

        protected void btnRecalDV_Click(object sender, EventArgs e)
        {
            if (PermisosHelper.ValidarPermisos(14, Session[SV.Permisos.GD()]))
            {
                TablaDVVManager _dVerificadorManager = new TablaDVVManager();
                Message msj = _dVerificadorManager.RecalcularDigitosVerificadores();
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                RebotarUsuarioSinPermisos();
            }
        }

        private void RebotarUsuarioSinPermisos(string redirect = null)
        {
            Message msj = MessageFactory.GetMessage("MS39", redirect);
            MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
        }
    }
}