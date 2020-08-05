using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Business;
using Common.Satellite.Seguridad;
using Negocio.Managers.Business.Carriers;
using Negocio.Managers.Business.Handlers;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using TransporteFlexible.Helper;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Business.Loads
{
    public partial class LoadsView : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Permiso 38
            if (Session[SV.LoggedUserName.GD()] != null) // COMPRUEBO QUE LA SESION NO HAYA EXPIRADO
            {
                if (SecurityHelper.CheckPermissions(18, Session[SV.Permissions.GD()]))
                {
                    int userId = (int)Session[SV.LoggedUserId.GD()];
                    UserManager _userMgr = new UserManager();
                    Rol role = _userMgr.GetUserRol(userId);
                    if (role != null)
                    {
                        if (!IsPostBack)
                        {
                            switch (role.Id)
                            {
                                case 2:
                                    //Dador de Carga
                                    BuildHandlersView(userId);
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
            TitleViewLBL.Text = "<i class='fas fa-truck-loading'></i>  Todas las Cargas";
            LoadManager _loadMgr = new LoadManager();
            List<Load> loads = _loadMgr.GetLoads(null);
            FillLoadsDDL(loads);
            FillDataGridView();
            SetUpButtonForCarrier();
        }

        private void BuildHandlersView(int userId)
        {
            LoadManager _loadMgr = new LoadManager();
            Session[SV.HandlerUserId.GD()] = userId;
            Load load = new Load { HandlerUser = new User { Id = userId } };
            List<Load> loads = _loadMgr.GetLoads(load);
            FillLoadsDDL(loads);
            FillDataGridView();
            SetUpButtonForHandler();
            
        }

        private void SetUpButtonForCarrier()
        {
            LoadsGridView.Columns[LoadsGridView.Columns.Count - 3].Visible = true;
            LoadsGridView.Columns[LoadsGridView.Columns.Count - 2].Visible = false;
            LoadsGridView.Columns[LoadsGridView.Columns.Count -1].Visible = true;
        }

        private void SetUpButtonForHandler()
        {
            LoadsGridView.Columns[LoadsGridView.Columns.Count - 3].Visible = false;
            LoadsGridView.Columns[LoadsGridView.Columns.Count - 2].Visible = true;
            LoadsGridView.Columns[LoadsGridView.Columns.Count -1].Visible = false;
        }

        private void FillDataGridView()
        {
            int HandlerUserId = 0;
            if (Session[SV.HandlerUserId.GD()] != null)
            {
                HandlerUserId = Convert.ToInt32(Session[SV.HandlerUserId.GD()]);
            }

            int loadId = Convert.ToInt32(LoadsDDL.SelectedValue);

            LoadManager _loadMgr = new LoadManager();
            
            List<Load> loads = _loadMgr.GetLoads(new Load { Id =loadId, HandlerUser = new User { Id = HandlerUserId } });
            LoadsGridView.DataSource = loads;
            LoadsGridView.DataBind();

            ActualizationDateLBL.Text = DateTime.Now.ToString();
        }

        private void FillLoadsDDL(List<Load> loads)
        {
            LoadsDDL.DataSource = loads;
            LoadsDDL.DataTextField = "Description";
            LoadsDDL.DataValueField = "Id";
            LoadsDDL.DataBind();

            ListItem li = new ListItem
            {
                Value = "0",
                Text = "Todas las cargas"
            };
            LoadsDDL.Items.Add(li);
            LoadsDDL.SelectedValue = "0";
        }

        protected void LoadsDLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataGridView();
        }

        protected void LoadsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer
            int index = Convert.ToInt32(e.CommandArgument);
            string commandName = e.CommandName;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.      
            GridViewRow row = LoadsGridView.Rows[index];

            string id = row.Cells[0].Text;

            Session[SV.EdditingLoadId.GD()] = id;

            int loadId = Convert.ToInt32(id);

            switch (commandName)
            {
                case "Edicion":
                    GoToEditLoadView(loadId);
                    break;
                case "Borrar":
                    DeleteLoad(loadId);
                    break;
                case "Ofertas":
                    GotToViewOffers(loadId);
                    break;
                case "EditOffer":
                    EditOffer(row.RowIndex);
                    break;
                case "Informacion":
                    GoToEditLoadView(loadId, "info");
                    break;
                case "Ofertar":
                    OfferPrice(row.RowIndex);
                    break;
                case "CancelarOferta":
                    CancelOffer(row.RowIndex);
                    break;
                default:
                    break;
            }
        }

        private void CancelOffer(int rowIndex)
        {
            HideOfferButtons(rowIndex);
            int cellIndex = LoadsGridView.Rows[rowIndex].Cells.Count - 2;
            TextBox offerTB = LoadsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("OfferTB") as TextBox;
            offerTB.Enabled = false;
            offerTB.Text = "";
        }

        private void OfferPrice(int rowIndex)
        {
            int carrierId = (int)Session[SV.LoggedUserId.GD()];
            int loadId = Convert.ToInt32(LoadsGridView.Rows[rowIndex].Cells[0].Text);
            int cellIndex = LoadsGridView.Rows[rowIndex].Cells.Count - 2;
            TextBox offerTB = LoadsGridView.Rows[rowIndex].Cells[cellIndex].FindControl("OfferTB") as TextBox;

            if (Decimal.TryParse(offerTB.Text, out decimal price))
            {
                OfferManager _offerMgr = new OfferManager();
                Common.Satellite.Shared.Message msj = _offerMgr.OfferPrice(loadId, carrierId, price);
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            
   
        }

        private void EditOffer(int rowId)
        {
            ShowOfferButtons(rowId);
            int cellIndex = LoadsGridView.Rows[rowId].Cells.Count - 2;
            WebControl offerTB = LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("OfferTB") as WebControl;
            offerTB.Enabled = true;

        }

        private void HideOfferButtons(int rowId)
        {
            int cellIndex = LoadsGridView.Rows[rowId].Cells.Count - 1;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("EditOffer").Visible = true;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("OfferInfo").Visible = true;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("Offer").Visible = false;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("CancelarOferta").Visible = false;
        }

        private void ShowOfferButtons(int rowId)
        {
            int cellIndex = LoadsGridView.Rows[rowId].Cells.Count - 1;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("EditOffer").Visible = false;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("OfferInfo").Visible = false;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("Offer").Visible = true;
            LoadsGridView.Rows[rowId].Cells[cellIndex].FindControl("CancelarOferta").Visible = true;
        }

        private void GotToViewOffers(int loadId)
        {
            throw new NotImplementedException();
        }

        private void DeleteLoad(int loadId)
        {
            throw new NotImplementedException();
        }

        private void GoToEditLoadView(int loadId, string type = null)
        {
            string qs = "?id=" + loadId.ToString();
            if (type == null)
            {
                qs = qs + "&type=" + type;
            }
            string urlRedirect = string.Concat(ViewsEnum.LoadAM.GD(),qs);
            Response.Redirect(urlRedirect);
        }

        protected void LoadsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }
    }
}