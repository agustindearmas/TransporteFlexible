using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Shared;
using Negocio.Managers.Shared;
using System;
using System.Linq;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Shared
{
    public partial class Welcome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                WelcomeUserTB.Text = Session[SV.LoggedUserName.GD()].ToString();
                try
                {
                    ConfigManager _configMgr = new ConfigManager();
                    Configuracion config = new Configuracion { Id = 1 };
                    config = _configMgr.Retrieve(config).Single();

                    if (config != null)
                    {
                        if (config.Valor == "1")
                        {
                            Message msj = MessageFactory.GetMessage("MS78", ViewsEnum.Bitacora.GD());
                            MessageHelper.ProcessMessage(GetType(), msj, Page);
                        }
                    }
                }
                catch
                {
                }
            }
            else
            { Response.Redirect("/"); }
        }
    }
}