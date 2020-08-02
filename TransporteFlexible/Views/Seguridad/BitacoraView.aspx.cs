using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using TransporteFlexible.Helper;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad
{
    public partial class BitacoraView : System.Web.UI.Page
    {
        private readonly LogManager _logMgr;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                //Permiso numero 16
                if (SecurityHelper.CheckPermissions(16, Session[SV.Permissions.GD()]))
                {
                    if (!IsPostBack)
                    {
                        PopularFiltro();
                        BuildView();
                    }
                }
                else
                {
                    Message msj = MessageFactory.GetMessage("MS39", "/");
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                }
            }
            else
            {
                Response.Redirect(ViewsEnum.SessionExpired.GD());
            }
        }

        public BitacoraView()
        {
            _logMgr = new LogManager();
        }

        private void PopularFiltro()
        {
            CriticidadBitacoraManager cbMgr = new CriticidadBitacoraManager();
            ListItem li = new ListItem
            {
                Text = "None",
                Value = "None"
            };
            CriticallyLevelDLL.Items.Add(li);
            List<NivelCriticidad> niveles = cbMgr.Retrieve(null);

            foreach (var nivel in niveles)
            {
                ListItem li2 = new ListItem
                {
                    Value = nivel.Descripcion,
                    Text = nivel.Descripcion
                };
                CriticallyLevelDLL.Items.Add(li2);
            }
        }

        private void BuildView()
        {
            Message msj = _logMgr.GetDecryptedBinnacles(
                 DateFromTB.Text,
                 DateToTB.Text,
                 CriticallyLevelDLL.SelectedIndex,
                 EventTB.Text,
                 UserNameTB.Text,
                 DownCBX.Checked);

            if (msj.CodigoMensaje != "OK")
            {
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                List<Bitacora> bitacoras = msj.Resultado as List<Bitacora>;
                LoadDataGridView(bitacoras);
            }
        }

        private void LoadDataGridView(List<Bitacora> binnacles)
        {
            BinnacleGridView.AllowPaging = true;
            DataTable dt = ConvertToDataTable(binnacles);
            Session["Binnacle"] = dt;
            BinnacleGridView.DataSource = dt;
            BinnacleGridView.DataBind();
            ActualizationDateLBL.Text = DateTime.Now.ToString();
        }

        public DataTable ConvertToDataTable(IList<Bitacora> data)
        {
            DataTable table = new DataTable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Bitacora));


            foreach (PropertyDescriptor prop in properties)
            {
                if (!NoNecesaryFieldsInGridView(prop))
                {
                    if (prop.DisplayName == "NivelCriticidad")
                        table.Columns.Add(prop.DisplayName, "".GetType());
                    else
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }

            foreach (Bitacora item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (!NoNecesaryFieldsInGridView(prop))
                    {
                        if (prop.DisplayName == "NivelCriticidad")
                            row[prop.Name] = ((NivelCriticidad)prop.GetValue(item)).Descripcion;
                        else
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private bool NoNecesaryFieldsInGridView(PropertyDescriptor prop)
        {
            return (prop.Name == "FechaDesde" || prop.Name == "FechaHasta" ||
                     prop.Name == "UsuarioModificacion" || prop.Name == "FechaModificacion"
                     || prop.Name == "DVH");
        }

        protected void BinnacleGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            BinnacleGridView.PageIndex = e.NewPageIndex;
            BuildView();
        }

        protected void FilterBinnacleBTN_Click(object sender, EventArgs e)
        {
            BuildView();
        }

        protected void ExportXMLButton_Click(object sender, EventArgs e)
        {
            XMLManager _xmlMgr = new XMLManager();
            DataTable dt = Session["Binnacle"] as DataTable;
            Message msj = _xmlMgr.ExportDataTableToXMLFile(dt, "bitacora", "Bitacora.xml");
            MessageHelper.ProcessMessage(GetType(), msj, Page);

        }

        protected void BinnacleGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int binnacleId = Convert.ToInt32(e.Values["Id"]);
            Message msj = _logMgr.Down(binnacleId);
            MessageHelper.ProcessMessage(GetType(), msj, Page);
            BuildView();
        }
    }
}