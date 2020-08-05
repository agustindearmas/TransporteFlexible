using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Business;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Business.Carriers;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using TransporteFlexible.Helper;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Business.Offer
{
    public partial class OfferView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Permiso 38
            if (Session[SV.LoggedUserName.GD()] != null) // COMPRUEBO QUE LA SESION NO HAYA EXPIRADO
            {
                if (SecurityHelper.CheckPermissions(38, Session[SV.Permissions.GD()]))
                {

                    if (!IsPostBack)
                    {
                        int userId = (int)Session[SV.LoggedUserId.GD()];
                        UserManager _userMgr = new UserManager();
                        Rol role = _userMgr.GetUserRol(userId);
                        if (role != null)
                        {
                            switch (role.Id)
                            {
                                case 2:
                                    //Dador de Carga
                                    BuildHandlersView();
                                    break;
                                case 3:
                                    //Transportista
                                    BuildCarriersView();
                                    break;
                                case 4:
                                    //Independiente
                                    BuildCarriersView();
                                    break;
                                case 5:
                                    //Conductor
                                    BuildCarriersView();
                                    break;
                                default:
                                    Response.Redirect(ViewsEnum.Default.GD());
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    Response.Redirect(ViewsEnum.Default.GD());
                }
            }
            else
            {
                // SI LA SESION EXPIRO REDIRIJO A LA PAGINA SESION EXPIRADA
                Response.Redirect(ViewsEnum.SessionExpired.GD());
            }

        }

        private void BuildCarriersView()
        {
            CardHeaderLBL.Text = "<i class='fas fa-dollar-sign'></i>  Mis Ofertas";
            Session[SV.CarrierId.GD()] = Session[SV.LoggedUserId.GD()];
            FillDDL();
            LoadDataGridView();
            SetUpButtonsForCarrier();
        }

        private void SetUpButtonsForCarrier()
        {
            OffersGridView.Columns[OffersGridView.Columns.Count - 2].Visible = false;
            OffersGridView.Columns[OffersGridView.Columns.Count - 1].Visible = true;
        }
        private void SetUpButtonForHander()
        {
            OffersGridView.Columns[OffersGridView.Columns.Count - 2].Visible = true;
            OffersGridView.Columns[OffersGridView.Columns.Count - 1].Visible = false;
        }
        private void BuildHandlersView()
        {
            FillDDL();
            LoadDataGridView();
            SetUpButtonForHander();
        }

        private void FillDDL()
        {
            int carrierId = 0;
            if (Session[SV.CarrierId.GD()] != null)
            {
                carrierId = (int)Session[SV.CarrierId.GD()];
            }

            OfferManager _offerMgr = new OfferManager();
            List<Common.Satellite.Business.Offer> offers =
                _offerMgr.GetOffers(new Common.Satellite.Business.Offer { CarrierUser = new User { Id = carrierId } });

            LoadsDDL.DataTextField = "Description";
            LoadsDDL.DataValueField = "Id";

            LoadsDDL.DataSource = from a in offers select new { a.Id, Description = a.Load.Description };
            LoadsDDL.DataBind();

            ListItem li = new ListItem
            {
                Value = "0",
                Text = "Todas las Cargas"
            };

            LoadsDDL.Items.Add(li);
            LoadsDDL.SelectedValue = "0";

        }

        protected void LoadsDLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDataGridView();
        }

        protected void OffersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int index = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = OffersGridView.Rows[index];

            string id = row.Cells[0].Text;

            Session[SV.OfferId.GD()] = id;

            int offerId = Convert.ToInt32(id);

            switch (commandName)
            {
                case "Aceptar":
                    AcceptOffer(offerId);
                    break;

                case "Rechazar":
                    break;

                case "EliminarOferta":
                    break;

                default:
                    break;
            }
        }

        private void AcceptOffer(int offerId)
        {
            OfferManager _offerMgr = new OfferManager();
            int loggedUserId = Convert.ToInt32(Session[SV.LoggedUserId.GD()]);
            Message msj = _offerMgr.AcceptOffer(offerId, loggedUserId);
            MessageHelper.ProcessMessage(GetType(), msj, Page);
        }

        protected void OffersGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        private void LoadDataGridView()
        {
            int carrierId = 0;
            if (Session[SV.CarrierId.GD()] != null)
            {
                carrierId = (int)Session[SV.CarrierId.GD()];
            }

            int offerId = Convert.ToInt32(LoadsDDL.SelectedValue);

            OfferManager _offerMgr = new OfferManager();
            List<Common.Satellite.Business.Offer> offers =
                _offerMgr.GetOffers(new Common.Satellite.Business.Offer { Id = offerId , CarrierUser = new User { Id = carrierId } });


            OffersGridView.AllowPaging = true;
            OffersGridView.DataSource = offers;
            OffersGridView.DataBind();
            ActualizationDateLBL.Text = DateTime.Now.ToString();
        }
    }
}