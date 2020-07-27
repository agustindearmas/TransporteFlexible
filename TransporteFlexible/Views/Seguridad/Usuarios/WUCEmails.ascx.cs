﻿using Common.Enums.Seguridad;
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
        public List<Email> Emails;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDataGridView(Emails);
                Session[SV.Emails.GD()] = Emails;
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

        protected void Add_Click(object sender, EventArgs e)
        {
            Email email = new Email();
            List<Email> emailsSession = (List<Email>)Session[SV.Emails.GD()];
            emailsSession.Add(email);
            LoadDataGridView(emailsSession);
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
                        if (emailId != 0)
                        {
                            EmailManager _emailMgr = new EmailManager();
                            Message msj = _emailMgr.ExistEmail(emailTB.Text, emailId);

                            if (msj.CodigoMensaje != "OK")
                            {
                                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                            }
                            else
                            {

                                msj = _emailMgr.SaveEmail(emailId, emailTB.Text, Convert.ToInt32(Session[SV.UsuarioLogueado.GD()]));

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
                                    LoadDataGridView(emailsSession);
                                    wc.Enabled = false;
                                }
                                HideSaveCancel(rowIndex);
                            }
                        }
                        else
                        {
                            PersonManager _personMgr = new PersonManager();
                            Message msjAdd = _personMgr.AddEmail(emailTB.Text, (int)Session[SV.UsuarioLogueado.GD()], (int)Session[SV.EditingPersonId.GD()]);
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
            List<Email> emailsSession = (List<Email>)Session[SV.Emails.GD()];
            LoadDataGridView(emailsSession);
            if (_emailsGridView.Rows[rowIndex].Cells[1].FindControl("txtEmail") is WebControl wc)
                wc.Enabled = false;
        }

        private void DeleteEmail(int emailId)
        {
            if (Page.IsValid)
            {
                PersonManager _personMgr = new PersonManager();
                int sessionUserId = (int)Session[SV.UsuarioLogueado.GD()];
                int sessionPeopleId = (int)Session[SV.EditingPersonId.GD()];
                Message msj = _personMgr.DeleteEmail(emailId, sessionUserId, sessionPeopleId);
                if (msj.CodigoMensaje == "OK")
                {
                    List<Email> emailsSession = (List<Email>)Session[SV.Emails.GD()];
                    emailsSession.RemoveAll(x => x.Id == emailId);
                    Session[SV.Emails.GD()] = emailsSession;
                    LoadDataGridView(emailsSession);
                }
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
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