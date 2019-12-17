<%@ Page Title="Validación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ValidaEmail.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.ValidaEmail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mastheadSinPadding">
        <section id="about" class="about-section text-center">
            <div class="">
                <div class="row">
                    <div class="col-lg-8 mx-auto">
                        <h2 class="text-white mb-4">Transporte Flexible</h2>
                        <p class="text-white-50">
                            Transporte flexible es un emprendimiento dedicado a conectar dadores de carga con transportistas, por medio de un sistema conocido como T-FLEX.              
                        T-FLEX es un sistema (cliente servidor de acceso WEB) que permite la independización y autogestión del trabajo del transportista, y la posibilidad de elegir 
                        al dador de carga el precio, el seguro y el transportista que más se ajuste a sus necesidades de transporte. Es un tipo de negocio B2B que presta sus servicio a
                        pequeñas y medianas empresas dadoras de carga y transportistas
                        </p>
                    </div>
                </div>
            </div>
        </section>
        <div class="form-group">
            <asp:Button ID="btnValidarCuenta" class="btn btn-info btn-block my-4" runat="server" Text="Validar Cuenta" OnClick="btnValidarCuenta_Click" />
        </div>
    </div>
</asp:Content>
