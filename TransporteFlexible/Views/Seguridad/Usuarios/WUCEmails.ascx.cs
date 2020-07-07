using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Shared;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using TransporteFlexible.Helper.GridView;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios
{
    public partial class WUCEmails : GridViewASCXExtensions<Email>
    {
        public List<Email> emails;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDataGridView(emails);
                Session[SV.Emails.GD()] = emails;
            }
        }

        internal override void LoadDataGridView(List<Email> entities)
        {
            _emailsGridView.AllowPaging = true;
            DataTable dt = ConvertToDataTable(entities);
            _emailsGridView.DataSource = dt;
            _emailsGridView.DataBind();

        }

        internal override bool EsCampoNoNecesario(PropertyDescriptor prop)
        {
            return (prop.Name == "FechaDesde" ||
                     prop.Name == "UsuarioCreacion" || prop.Name == "UsuarioModificacion" ||
                     prop.Name == "FechaModificacion");
        }

        protected void EmailsGridView_RowCommands(object sender, GridViewCommandEventArgs e)
        {

            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = _emailsGridView.Rows[rowIndex];

            string emailId = row.Cells[0].Text;
            int emailIdInt;

            if (!string.IsNullOrWhiteSpace(emailId))
            {
                emailIdInt = Convert.ToInt32(emailId);
                switch (commandName)
                {
                    case "_edit":
                        EditEmail(rowIndex);
                        break;
                    case "_delete":
                        DeleteEmail(emailIdInt);
                        break;
                    case "_update":
                        UpdateEmail(emailIdInt, rowIndex);
                        break;
                    case "_cancel":
                        CancelUpdate(rowIndex);
                        break;
                    default:
                        break;
                }
            }
        }

        private void EditEmail(int rowIndex)
        {
            if (_emailsGridView.Rows[rowIndex].Cells[1].FindControl("txtEmail") is WebControl wc)
            {
                wc.Enabled = true;
                ShowSaveCancel(rowIndex);
            }
        }

        protected void UpdateEmail(int emailId, int rowIndex)
        {
            if (Page.IsValid)
            {
                if (_emailsGridView.Rows[rowIndex].Cells[1].FindControl("txtEmail") is WebControl wc)
                {
                    if (wc is TextBox emailTB)
                    {
                        EmailManager _emailMgr = new EmailManager();
                        Mensaje msj = _emailMgr.ExistEmail(emailTB.Text, emailId);

                        if (msj.CodigoMensaje != "OK")
                        {
                            MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                        }
                        else
                        {
                            HideSaveCancel(rowIndex);
                            msj = _emailMgr.UpdateEmail(emailId, emailTB.Text, Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));

                            if (msj.CodigoMensaje != "OK")
                            {
                                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                            }

                            if (msj.Resultado is Email email)
                            {
                                // Remuevo el email viejo de la lista en session 
                                // y le agrego el mail nuevo que es devuelto por el metodo que salva 
                                // el mail actualizado
                                List<Email> emailsSession = (List<Email>)Session[SV.Emails.GD()];
                                emailsSession.RemoveAll(x => x.Id == email.Id);
                                emailsSession.Add(email);
                                Session[SV.Emails.GD()] = emailsSession;
                                wc.Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void CancelUpdate(int rowIndex)
        {
            List<Email> emailsSession = (List<Email>)Session[SV.Emails.GD()];
            LoadDataGridView(emailsSession);
            if (_emailsGridView.Rows[rowIndex].Cells[1].FindControl("txtEmail") is WebControl wc)
                wc.Enabled = false;
        }

        private void DeleteEmail(int emailId)
        {
        }

        private void ShowSaveCancel(int rowIndex)
        {
            int cellIndex = _emailsGridView.Rows[rowIndex].Cells.Count - 1;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Edit").Visible = false;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Delete").Visible = false;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Save").Visible = true;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Cancel").Visible = true;
        }

        private void HideSaveCancel(int rowIndex)
        {
            int cellIndex = _emailsGridView.Rows[rowIndex].Cells.Count - 1;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Edit").Visible = true;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Delete").Visible = true;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Save").Visible = false;
            _emailsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("Cancel").Visible = false;
        }
    }
}