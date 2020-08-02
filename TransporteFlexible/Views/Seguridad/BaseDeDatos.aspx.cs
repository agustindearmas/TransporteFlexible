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
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                if (!SecurityHelper.CheckPermissions(42, Session[SV.Permissions.GD()]))
                {
                    RebotarUsuarioSinPermisos("/");
                }
            }
            else
            {
                Response.Redirect(ViewsEnum.SessionExpired.GD());
            }
        }

        private void RebotarUsuarioSinPermisos(string redirect = null)
        {
            Message msj = MessageFactory.GetMessage("MS39", redirect);
            MessageHelper.ProcessMessage(GetType(), msj, Page);
        }

        protected void RestoreBDButton_Click(object sender, EventArgs e)
        {
            if (Session[SV.Permissions.GD()] != null && SecurityHelper.CheckPermissions(13, Session[SV.Permissions.GD()]))
            {
                BDManager _bdMgr = new BDManager();
                Message msj = _bdMgr.MontarBKP(fuRestore.FileName, Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                RebotarUsuarioSinPermisos();
            }
        }

        protected void BackUpBDButton_Click(object sender, EventArgs e)
        {
            if (Session[SV.Permissions.GD()] != null && SecurityHelper.CheckPermissions(12, Session[SV.Permissions.GD()]))
            {
                BDManager _bdMgr = new BDManager();
                Message msj = _bdMgr.GenerarBKP(BkpNameTB.Text, Convert.ToInt32(Session[SV.LoggedUserId.GD()]));
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                RebotarUsuarioSinPermisos();
            }
        }

        protected void RecalculateDigitsBTN_Click(object sender, EventArgs e)
        {
            if (SecurityHelper.CheckPermissions(14, Session[SV.Permissions.GD()]))
            {
                TablaDVVManager _dVerificadorManager = new TablaDVVManager();
                Message msj = _dVerificadorManager.RecalcularDigitosVerificadores();
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                RebotarUsuarioSinPermisos();
            }
        }
    }
}