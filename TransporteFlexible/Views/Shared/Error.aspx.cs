using System;

namespace TransporteFlexible.Views.Shared
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            _spnError.InnerText = Session["Error"].ToString();
        }
    }
}