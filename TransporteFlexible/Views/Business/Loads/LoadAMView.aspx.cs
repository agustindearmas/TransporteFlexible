using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Business;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Business.Handlers;
using Negocio.Managers.Seguridad;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using TransporteFlexible.Helper;

namespace TransporteFlexible.Views.Business.Loads
{
    public partial class LoadAMView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                                    BuildView();
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

        private void BuildView()
        {
            if (Request.QueryString.HasKeys()) // VERIFICO QUE EXISTAN QUERY PARAMS
            {
                string idQS = Request.QueryString.Get("id");// OBTENGO QS USER ID
                string typeQS = Request.QueryString.Get("type");// OBTENGO QS TYPE PAGE, ME INDICA EN QUE MODO DEBO CARGAR LA PAGINA

                if (idQS == null || typeQS == null)
                {
                    Response.Redirect(ViewsEnum.Default.GD());
                }

                int loadId = Convert.ToInt32(idQS);

                if (loadId == 0)
                {
                    BuildAddVIew();
                }
                else
                {
                    BuildEditView(loadId);
                }

            }
        }

        private void BuildEditView(int loadId)
        {

        }

        private void BuildAddVIew()
        {
            ProvinceManager _provMgr = new ProvinceManager();
            List<Province> provinces = _provMgr.Retrieve(null);
            Session[SV.Provinces.GD()] = provinces;
            ProvinceStartDDL.DataSource = provinces;
            ProvinceStartDDL.DataTextField = "Description";
            ProvinceStartDDL.DataValueField = "Id";
            ProvinceStartDDL.DataBind();

            LocationStartDDL.Enabled = false;

            EndProvinceDDL.DataSource = provinces;
            EndProvinceDDL.DataTextField = "Description";
            EndProvinceDDL.DataValueField = "Id";
            EndProvinceDDL.DataBind();

            EndLocationDDL.Enabled = false;
        }

        protected void EndProvinceDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Province> provinces = ((List<Province>)Session[SV.Provinces.GD()]);
            string selectedValue = EndProvinceDDL.SelectedValue;
            Province province = provinces.Where(pro => pro.Id == Convert.ToInt32(selectedValue)).Single();

            EndLocationDDL.DataSource = province.Locations;
            EndLocationDDL.DataTextField = "Description";
            EndLocationDDL.DataValueField = "Id";
            EndLocationDDL.DataBind();
            EndLocationDDL.Enabled = true;

        }

        protected void ProvinceStartDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Province> provinces = ((List<Province>)Session[SV.Provinces.GD()]);
            string selectedValue = ProvinceStartDDL.SelectedValue;
            Province province = provinces.Where(pro => pro.Id == Convert.ToInt32(selectedValue)).Single();

            LocationStartDDL.DataSource = province.Locations;
            LocationStartDDL.DataTextField = "Description";
            LocationStartDDL.DataValueField = "Id";
            LocationStartDDL.DataBind();
            LocationStartDDL.Enabled = true;
        }

        protected void Cancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewsEnum.Loads.GD());
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            SaveLoad();

        }

        protected void SaveDown_Click(object sender, EventArgs e)
        {
            SaveLoad();
        }

        protected void CancelDown_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewsEnum.Loads.GD());
        }


        private void SaveLoad()
        {
            if (Page.IsValid)
            {


                //CARGA
                string selectedProduct = ProductDDL.SelectedValue;
                string selectedPackage = PackageDDL.SelectedValue;
                string productName = ProductTB.Text;
                string description = DescriptionTB.Text;
                string unitSelected = UnitDDL.SelectedValue;
                string quantity = QuantityTB.Text;
                string obs = ObservationsTB.Text;

                LoadManager _lMgr = new LoadManager();
                LoadDetailManager _ldMgr = new LoadDetailManager();

                LoadDetail ld = new LoadDetail
                {
                    Id = 0,
                    DVH = 0,
                    Observations = obs,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    Quantity = Convert.ToInt32(quantity),
                    UsuarioCreacion = 1,
                    UsuarioModificacion = 1
                };
                int ldid = _ldMgr.Save(ld);




                //Addres Start
                string selectedStartProvince = ProvinceStartDDL.SelectedValue;
                string selectedStartLocation = LocationStartDDL.SelectedValue;
                string startPostalCode = StartPostalCodeTB.Text;
                string startStreet = StartStreetTB.Text;
                string startNumer = StartNumberTB.Text;
                string startFloor = StartFloorTB.Text;
                string startUnit = StartUnitTB.Text;

                AddressManager aMgr = new AddressManager();

                Address addStart = new Address
                {
                    Id = 0,
                    Floor = startFloor,
                    Street = startStreet,
                    Unit = startUnit,
                    Number = Convert.ToInt32(startNumer),
                    Province = new Province { Id = Convert.ToInt32(selectedStartProvince) },
                    Location = new Location { Id = Convert.ToInt32(selectedStartLocation) },
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    UsuarioCreacion = 1,
                    UsuarioModificacion = 1,
                    DVH = 0,
                };
                aMgr.CryptFields(addStart);
                addStart.Id = aMgr.Save(addStart);



                //Addres End
                string endProvinceSelected = EndProvinceDDL.SelectedValue;
                string endLocationSelected = EndLocationDDL.SelectedValue;
                string endPostalCode = EndPostalCodeTB.Text;
                string endStreet = EndStreetTB.Text;
                string endNumber = EndNumberTB.Text;
                string endFloor = EndFloorTB.Text;
                string endUnit = EndUnitTB.Text;

                Address addEnd = new Address
                {
                    Id = 0,
                    Floor = endFloor,
                    Street = endStreet,
                    Unit = endUnit,
                    Number = Convert.ToInt32(endNumber),
                    Province = new Province { Id = Convert.ToInt32(endProvinceSelected) },
                    Location = new Location { Id = Convert.ToInt32(endLocationSelected) },
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    UsuarioCreacion = 1,
                    UsuarioModificacion = 1,
                    DVH = 0,
                };
                aMgr.CryptFields(addEnd);
                addEnd.Id = aMgr.Save(addEnd);




                Load l = new Load
                {
                    Id = 0,
                    AddressEnd = addEnd,
                    AddressStart = addStart,
                    Description = description,
                    LoadDetail = ld,
                    HandlerUser = new User { Id = (int)Session[SV.LoggedUserId.GD()] },
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    UsuarioCreacion = 1,
                    UsuarioModificacion = 1,
                    DVH = 0,


                };
                
                _lMgr.Save(l);

                Response.Redirect(ViewsEnum.Loads.GD());
            }
        }
    }
}