using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
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
        private readonly BitacoraManager _bitacoraMgr;
        protected void Page_Load(object sender, EventArgs e)
        {
            //Permiso numero 16
            if (PermisosHelper.ValidarPermisos(16, Session["Permisos"]))
            {
                if (!IsPostBack)
                {
                    PopularFiltro();
                    PopularTabla();
                }
            }
            else
            {
               Mensaje msj = Mensaje.CrearMensaje("MS39", false, true, null, "/");
               MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }

        public BitacoraView()
        {
            _bitacoraMgr = new BitacoraManager();
        }

        private void PopularFiltro()
        {
            CriticidadBitacoraManager cbMgr = new CriticidadBitacoraManager();

            List<NivelCriticidad> niveles = cbMgr.Retrieve(null);
            ListItem li = new ListItem
            {
                Text = "None",
                Value = "None"
            };
            _ddlNivelCriticidad.Items.Add(li);
            foreach (var nivel in niveles)
            {
                ListItem li2 = new ListItem
                {
                    Value = nivel.Descripcion,
                    Text = nivel.Descripcion
                };
                _ddlNivelCriticidad.Items.Add(li2);
            }
        }

        private void PopularTabla()
        {
            Mensaje msj = _bitacoraMgr.ObtenerBitacorasDesencriptadas(
                _txtFechaDesde.Text,
                _txtFechaHasta.Text,
                _ddlNivelCriticidad.SelectedIndex,
                _txtEvento.Text,
                _txtUsuario.Text);
            if (msj.CodigoMensaje != "OK")
            {
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                List<Bitacora> bitacoras = (List<Bitacora>)msj.Resultado;
                BuildDataGridView(bitacoras);
            }
        }

        private void BuildDataGridView(List<Bitacora> bitacoras)
        {
            _bitacoraGridView.AllowPaging = true;
            DataTable dt = ConvertToDataTable(bitacoras);
            _bitacoraGridView.DataSource = dt;
            _bitacoraGridView.DataBind();
            _lblFechaActualizacion.Text = DateTime.Now.ToString();
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (!EsCampoNoNecesario(prop))
                {
                    if (prop.DisplayName == "NivelCriticidad")
                    {
                        table.Columns.Add(prop.DisplayName, "".GetType());
                    }
                    else
                    {
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }

                }
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (!EsCampoNoNecesario(prop))
                    {
                        if (prop.DisplayName == "NivelCriticidad")
                        {
                            row[prop.Name] = ((NivelCriticidad)prop.GetValue(item)).Descripcion;
                        }
                        else
                        {
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        }
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        protected void _bitacoraGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            _bitacoraGridView.PageIndex = e.NewPageIndex;
            PopularTabla();
        }

        protected void btnFiltrarBita_Click(object sender, EventArgs e)
        {
            PopularTabla();
        }

        private bool EsCampoNoNecesario(PropertyDescriptor prop)
        {
            return (prop.Name == "FechaDesde" || prop.Name == "FechaHasta" ||
                    prop.Name == "UsuarioCreacion" || prop.Name == "UsuarioModificacion" ||
                    prop.Name == "FechaModificacion");
        }

    }
}