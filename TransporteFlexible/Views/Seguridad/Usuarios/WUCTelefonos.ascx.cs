using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Shared;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TransporteFlexible.Helper.GridView;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios
{
    public partial class WUCTelefonos : GridViewASCXExtensions<Telefono>
    {
        public List<Telefono> Phones;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDataGridView(Phones);
                Session[SV.Telefonos.GD()] = Phones;
            }
        }

        internal override void LoadDataGridView(List<Telefono> entities)
        {
            _phonesGridView.AllowPaging = true;
            DataTable dt = ConvertToDataTable(entities);
            _phonesGridView.DataSource = dt;
            _phonesGridView.DataBind();

        }

        internal override bool EsCampoNoNecesario(PropertyDescriptor prop)
        {
            return (prop.Name == "FechaDesde" ||
                     prop.Name == "UsuarioCreacion" || prop.Name == "UsuarioModificacion" ||
                     prop.Name == "FechaModificacion");
        }

        protected void PhonesGridView_RowCommands(object sender, GridViewCommandEventArgs e)
        {

            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = _phonesGridView.Rows[rowIndex];

            string phoneId = row.Cells[0].Text;
            int phoneIdInt;

            if (!string.IsNullOrWhiteSpace(phoneId))
            {
                phoneIdInt = Convert.ToInt32(phoneId);
                switch (commandName)
                {
                    case "_edit":
                        EditPhone(rowIndex);
                        break;
                    case "_delete":
                        DeletePhone(phoneIdInt);
                        break;
                    case "_update":
                        UpdatePhone(phoneIdInt, rowIndex);
                        break;
                    case "_cancel":
                        CancelUpdate(rowIndex);
                        break;
                    default:
                        break;
                }
            }
        }

        private void EditPhone(int rowIndex)
        {
            if (_phonesGridView.Rows[rowIndex].Cells[1].FindControl("txtPhone") is WebControl wc)
            {
                wc.Enabled = true;
                ShowSaveCancel(rowIndex);
            }
        }

        protected void UpdatePhone(int phoneId, int rowIndex)
        {
            if (Page.IsValid)
            {
                if (_phonesGridView.Rows[rowIndex].Cells[1].FindControl("txtPhone") is WebControl wc)
                {
                    if (wc is TextBox phoneTb)
                    {
                        if (phoneId != 0)
                        {
                            PhoneManager _phoneMgr = new PhoneManager();

                            Message msj = _phoneMgr.SavePhone(phoneId, phoneTb.Text, Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));

                            if (msj.CodigoMensaje != "OK")
                            {
                                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                            }

                            if (msj.Resultado is Telefono phone)
                            {
                                HideSaveCancel(rowIndex);
                                // Remuevo el Telefono viejo de la lista en session 
                                // y le agrego el Telefono nuevo que es devuelto por el metodo que salva 
                                // el telefono actualizado
                                List<Telefono> phoneSession = (List<Telefono>)Session[SV.Telefonos.GD()];
                                phoneSession.RemoveAll(x => x.Id == phone.Id);
                                phoneSession.Add(phone);
                                Session[SV.Telefonos.GD()] = phoneSession;
                                LoadDataGridView(phoneSession);
                                wc.Enabled = false;
                            }
                        }
                        else
                        {
                            PersonManager _personMgr = new PersonManager();
                            Message msjAdd = _personMgr.AddPhone(phoneTb.Text, (int)Session[SV.UsuarioLogueado.GD()], (int)Session[SV.EditingPersonId.GD()]);
                            if (msjAdd.CodigoMensaje != "OK")
                            {
                                MensajesHelper.ProcesarMensajeGenerico(GetType(), msjAdd, Page);
                            }
                            else
                            {
                                HideSaveCancel(rowIndex);
                                wc.Enabled = false;
                            }
                        }

                    }
                }
            }
        }

        private void CancelUpdate(int rowIndex)
        {
            List<Telefono> phoneSession = (List<Telefono>)Session[SV.Telefonos.GD()];
            LoadDataGridView(phoneSession);
            if (_phonesGridView.Rows[rowIndex].Cells[1].FindControl("txtPhone") is WebControl wc)
                wc.Enabled = false;
        }

        private void DeletePhone(int phoneId)
        {
            if (Page.IsValid)
            {
                PersonManager _personMgr = new PersonManager();
                int sessionUserId = (int)Session[SV.UsuarioLogueado.GD()];
                int sessionPeopleId = (int)Session[SV.EditingPersonId.GD()];
                Message msj = _personMgr.DeletePhone(phoneId, sessionUserId, sessionPeopleId);
                if (msj.CodigoMensaje == "OK")
                {
                    List<Telefono> phoneSession = (List<Telefono>)Session[SV.Telefonos.GD()];
                    phoneSession.RemoveAll(x => x.Id == phoneId);
                    Session[SV.Telefonos.GD()] = phoneSession;
                    LoadDataGridView(phoneSession);
                }
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
        }

        private void ShowSaveCancel(int rowIndex)
        {
            int cellIndex = _phonesGridView.Rows[rowIndex].Cells.Count - 1;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Edit").Visible = false;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Delete").Visible = false;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Save").Visible = true;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Cancel").Visible = true;
        }

        private void HideSaveCancel(int rowIndex)
        {
            int cellIndex = _phonesGridView.Rows[rowIndex].Cells.Count - 1;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Edit").Visible = true;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Delete").Visible = true;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Save").Visible = false;
            _phonesGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Cancel").Visible = false;
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            Telefono phone = new Telefono();
            List<Telefono> phonesSession = (List<Telefono>)Session[SV.Telefonos.GD()];
            phonesSession.Add(phone);
            LoadDataGridView(phonesSession);
        }
    }
}