using Common.Enums.Seguridad;
using Common.Extensions;
using System;

namespace TransporteFlexible.Views.Seguridad
{
    public partial class SessionExpired : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session[SV.LoggedUserId.GD()] = null;
            Session[SV.LoggedUserName.GD()] = null;
            Session[SV.Permissions.GD()] = null;
        }
    }
}