<%@ Page Title="Base de Datos" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="BaseDeDatos.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.BaseDeDatos" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Bienvenida.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Base de Datos</a>
            </li>
        </ol>

    </div>
    <div class="text-center border border-light p-5 w-100" id="formRegistro">
        <asp:Panel ID="pnlRespRest" runat="server">
            <p class="h4 mb-4">Gestión de Base de Datos <span class="fa fa-database"></span></p>


            <div class="form-group col-md-12 card border-info">
                <div class="form-row">
                    <p class="h5 mb-4">Restauración <span class="fa fa-recycle"></span></p>
                </div>

                <div class="form-row">
                    <div class="form-group col-md-12">
                        <asp:FileUpload ID="fuRestore" runat="server" />
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-12">
                        <asp:Button ID="btnRestaurar" class="btn btn-primary js-scroll-trigger w-100" runat="server" Text="Restaurar Base de Datos" OnClick="btnRestaurar_Click" />
                    </div>
                </div>
            </div>




            <div class="form-group col-md-12 card border-info">

                <div class="form-row">
                    <p class="h5 mb-4">Respaldo <span class="fa fa-save"></span></p>
                </div>


                <div class="form-row">
                    <div class="form-group col-md-12">
                        <label for="txtNombreRespaldo">Nombre del Respaldo</label>
                        <asp:TextBox ID="txtNombreRespaldo" class="form-control mb-4 w-100" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-12">
                        <asp:Button ID="btnGenerarRespaldo" class="btn btn-primary js-scroll-trigger w-100" runat="server" Text="Respaldar Base de Datos" OnClick="btnGenerarRespaldo_Click" />
                    </div>
                </div>
            </div>

            <div class="form-group col-md-12 card border-info">

                <div class="form-row">
                    <p class="h5 mb-4">Digitos Verificadores <span class="fa fa-user-lock"></span></p>
                </div>


                <div class="form-row">
                    <div class="form-group col-md-12">
                        <asp:Button ID="btnRecalDV" class="btn btn-primary js-scroll-trigger w-100" runat="server" Text="Recalcular Digitos Verificadores" OnClick="btnRecalDV_Click" />
                    </div>
                </div>
            </div>



        </asp:Panel>
    </div>
</asp:Content>
