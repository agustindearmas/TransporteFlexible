using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using Negocio.Managers.Shared;
using System;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad
{
    public partial class ValidaEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
        }

        protected void BtnValidarCuenta_Click(object sender, EventArgs e)
        {
            string emailId = Request.QueryString["1"];
            string userId = Request.QueryString["2"];
            Message msj;
            
            UserManager _userMgr = new UserManager();
            msj = _userMgr.ValidateAccount(emailId, userId);
            MessageHelper.ProcessMessage(GetType(), msj, Page);
        }
    }
}