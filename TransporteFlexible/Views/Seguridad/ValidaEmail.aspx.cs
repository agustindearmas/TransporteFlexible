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
            string id = Request.QueryString["id"];
            EmailManager _emailMgr = new EmailManager();
            Message msj = _emailMgr.ValidateEmailAccount(id);
            MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
        }
    }
}