using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Shared;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using TransporteFlexible.Helper.GridView;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios
{
    public partial class WUCAddress : GridViewASCXExtensions<Address>
    {
        public List<Address> Addresses;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session[SV.Addresses.GD()] = Addresses;
                ProvinceManager _provinceMgr = new ProvinceManager();
                Session[SV.Provinces.GD()] = _provinceMgr.Retrieve(null);
                LoadDataGridView(Addresses);
            }
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            Address address = new Address();
            List<Address> addressSession = (List<Address>)Session[SV.Addresses.GD()];
            addressSession.Add(address);
            LoadDataGridView(addressSession);
        }

        protected void AddressGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = AddressGridView.Rows[rowIndex];

            string addressId = row.Cells[0].Text;
            int addressIdInt;

            if (!string.IsNullOrWhiteSpace(addressId))
            {
                addressIdInt = Convert.ToInt32(addressId);
                switch (commandName)
                {
                    case "_edit":
                        EditAddress(rowIndex);
                        break;
                    case "_delete":
                        DeleteAddress(addressIdInt);
                        break;
                    case "_update":
                        SaveAddressChanges(addressIdInt, rowIndex);
                        break;
                    case "_cancel":
                        CancelUpdate(rowIndex);
                        break;
                    default:
                        break;
                }
            }
        }

        private void CancelUpdate(int rowIndex)
        {
            List<Address> addressSession = (List<Address>)Session[SV.Addresses.GD()];
            LoadDataGridView(addressSession);
            EnableDisableRowControls(rowIndex, false);
        }

        private void EnableDisableRowControls(int rowIndex, bool enabledFlag)
        {
            Province selectedProvince = null;
            if (AddressGridView.Rows[rowIndex].Cells[1].FindControl("ddlProvinces") is WebControl ddlProvinces)
            {
                if (enabledFlag)
                {
                    DropDownList ddl = ddlProvinces as DropDownList;
                    List<Province> provinces = ((List<Province>)Session[SV.Provinces.GD()]);
                    ddl.DataSource = provinces;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "Id";
                    ddl.DataBind();
                    ddl.SelectedIndex = 0;
                    selectedProvince = provinces.Where(pro => pro.Id == 1).Single();
                   
                }
                ddlProvinces.Enabled = enabledFlag;
            }

            if (AddressGridView.Rows[rowIndex].Cells[1].FindControl("ddlLocations") is WebControl ddlLocation)
            {   
                if (enabledFlag)
                {
                    DropDownList ddl = ddlLocation as DropDownList;
                    ddl.DataSource = selectedProvince.Locations;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "Id";
                    ddl.DataBind();
                }
                ddlLocation.Enabled = enabledFlag;
            }

            if (AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtStreet") is WebControl txtStreet)
            {
                txtStreet.Enabled = enabledFlag;
            }

            if (AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtNumber") is WebControl txtNumber)
            {
                txtNumber.Enabled = enabledFlag;
            }

            if (AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtFloor") is WebControl txtFloor)
            {
                txtFloor.Enabled = enabledFlag;
            }

            if (AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtUnit") is WebControl txtUnit)
            {
                txtUnit.Enabled = enabledFlag;
            }
        }

        private void SaveAddressChanges(int addressIdInt, int rowIndex)
        {
            if (Page.IsValid)
            {
                if (addressIdInt != 0)
                {
                    UpdateAddress(addressIdInt, rowIndex);
                }
                else
                {
                    InsertAddress(addressIdInt, rowIndex);
                }
            }
        }

        private void InsertAddress(int addressIdInt, int rowIndex)
        {
            PersonManager _personMgr = new PersonManager();
            Address address = GetAddressFromGridView(addressIdInt, rowIndex);
            Message msjAdd = _personMgr.AddAddress(address, (int)Session[SV.UsuarioLogueado.GD()], (int)Session[SV.EditingPersonId.GD()]);
            if (msjAdd.CodigoMensaje != "OK")
            {
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msjAdd, Page);
            }
            else
            {
                HideSaveCancel(rowIndex);
                EnableDisableRowControls(rowIndex, false);
            }
        }

        private Address GetAddressFromGridView(int addressIdInt, int rowIndex)
        {
            DropDownList ddlProvinces = AddressGridView.Rows[rowIndex].Cells[1].FindControl("ddlProvinces") as DropDownList;
            DropDownList ddlLocation = AddressGridView.Rows[rowIndex].Cells[1].FindControl("ddlLocations") as DropDownList;
            TextBox txtStreet = AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtStreet") as TextBox;
            TextBox txtNumber = AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtNumber") as TextBox;
            TextBox txtFloor = AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtFloor") as TextBox;
            TextBox txtUnit = AddressGridView.Rows[rowIndex].Cells[1].FindControl("txtUnit") as TextBox;

            List<Address> addressSession = (List<Address>)Session[SV.Addresses.GD()];
            Address address = addressSession.Where(ad => ad.Id == addressIdInt).Single();
            address.Province = new Province { Id = Convert.ToInt32(ddlProvinces.SelectedValue) };
            address.Location = new Location { Id = Convert.ToInt32(ddlLocation.SelectedValue) };
            address.Street = txtStreet.Text;
            address.Number = Convert.ToInt32(txtNumber.Text);
            address.Floor = txtFloor.Text;
            address.Unit = txtUnit.Text;
            return address;
        }

        private void UpdateAddress(int addressIdInt, int rowIndex)
        {
            Address address = GetAddressFromGridView(addressIdInt, rowIndex);
            AddressManager _addressMgr = new AddressManager();
            Message msj = _addressMgr.SaveAddress(address, Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));

            if (msj.CodigoMensaje != "OK")
            {
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                address = msj.Resultado as Address;
                // Remuevo el address viejo de la lista en session 
                // y le agrego el address nuevo que es devuelto por el metodo que salva 
                // el address actualizado
                List<Address> addressSession = (List<Address>)Session[SV.Addresses.GD()];
                addressSession.RemoveAll(x => x.Id == address.Id);
                addressSession.Add(address);
                Session[SV.Addresses.GD()] = addressSession;
                LoadDataGridView(addressSession);
                EnableDisableRowControls(rowIndex, false);
                HideSaveCancel(rowIndex);
            }
        }

        private void DeleteAddress(int addressIdInt)
        {
            if (Page.IsValid)
            {
                PersonManager _personMgr = new PersonManager();
                int sessionUserId = (int)Session[SV.UsuarioLogueado.GD()];
                int sessionPeopleId = (int)Session[SV.EditingPersonId.GD()];
                Message msj = _personMgr.DeleteAddress(addressIdInt, sessionUserId, sessionPeopleId);
                if (msj.CodigoMensaje == "OK")
                {
                    List<Address> addressSession = (List<Address>)Session[SV.Addresses.GD()];
                    addressSession.RemoveAll(x => x.Id == addressIdInt);
                    Session[SV.Addresses.GD()] = addressSession;
                    LoadDataGridView(addressSession);
                }
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }

        private void EditAddress(int rowIndex)
        {
            EnableDisableRowControls(rowIndex, true);
            ShowSaveCancel(rowIndex);
        }

        internal override void LoadDataGridView(List<Address> entities)
        {
            AddressGridView.AllowPaging = true;
            DataTable dt = ConvertToDataTable(entities);
            AddressGridView.DataSource = dt;
            AddressGridView.DataBind();

            foreach (GridViewRow row in AddressGridView.Rows)
            {
                TableCell idControl = row.Controls[0] as TableCell;
                Address address = entities.Where(ad => ad.Id == Convert.ToInt32(idControl.Text)).Single();
                DropDownList ddlProvinces = row.FindControl("ddlProvinces") as DropDownList;
                List<Province> provinces = ((List<Province>)Session[SV.Provinces.GD()]);
                ddlProvinces.DataSource = provinces;
                ddlProvinces.SelectedValue = address.Province.Id.ToString();
                ddlProvinces.DataBind();

                DropDownList ddlLocations = row.FindControl("ddlLocations") as DropDownList;
                Province province = provinces.Where(pro => pro.Id == Convert.ToInt32(ddlProvinces.SelectedValue)).Single();
                ddlLocations.DataSource = province.Locations;
                ddlLocations.SelectedValue = address.Location.Id.ToString();
                ddlLocations.DataBind();

            }

        }

        internal override bool EsCampoNoNecesario(PropertyDescriptor prop)
        {
            return (prop.Name == "FechaDesde" ||
                    prop.Name == "UsuarioCreacion" || prop.Name == "UsuarioModificacion" ||
                    prop.Name == "FechaModificacion");
        }

        private void ShowSaveCancel(int rowIndex)
        {
            int cellIndex = AddressGridView.Rows[rowIndex].Cells.Count - 1;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Edit").Visible = false;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Delete").Visible = false;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Save").Visible = true;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Cancel").Visible = true;
        }

        private void HideSaveCancel(int rowIndex)
        {
            int cellIndex = AddressGridView.Rows[rowIndex].Cells.Count - 1;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Edit").Visible = true;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Delete").Visible = true;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Save").Visible = false;
            AddressGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Cancel").Visible = false;
        }

        protected void ddlProvinces_SelectedIndexChanged(object sender, EventArgs e)
        {
            int rowIndex = ((GridViewRow)((DropDownList)sender).NamingContainer).RowIndex;

            DropDownList ddlProvinces = AddressGridView.Rows[rowIndex].FindControl("ddlProvinces") as DropDownList;
            DropDownList ddlLocation = AddressGridView.Rows[rowIndex].FindControl("ddlLocations") as DropDownList;

            List<Province> provinces = ((List<Province>)Session[SV.Provinces.GD()]);
            string selectedValue = ddlProvinces.SelectedValue;
            Province province = provinces.Where(pro => pro.Id == Convert.ToInt32(selectedValue)).Single();

            ddlLocation.DataSource = province.Locations;
            ddlLocation.DataTextField = "Description";
            ddlLocation.DataValueField = "Id";
            ddlLocation.DataBind();
            ddlLocation.Enabled = true;
        }
    }
}