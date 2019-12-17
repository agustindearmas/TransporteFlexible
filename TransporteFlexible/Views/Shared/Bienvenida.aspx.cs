using System;

namespace TransporteFlexible.Views.Shared
{
    public partial class bienvenida : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            _lblbienvenidoUsuario.Text = Session["NombreUsuario"].ToString();
        }
    }
}