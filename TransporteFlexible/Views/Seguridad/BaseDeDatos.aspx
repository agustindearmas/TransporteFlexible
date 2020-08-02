<%@ Page Title="Base de Datos" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="BaseDeDatos.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.BaseDeDatos" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <script src="../Javascript/disableControls.js"></script>
        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Base de Datos</a>
            </li>
        </ol>

        <p class="h4 mb-4">Gestión de Base de Datos <span class="fa fa-database"></span></p>
            <div class="form-row form-group">

                <%--RESTAURACION--%>
                <div class="col-md-6">

                    <div class="card">
                        <div class="card-header">
                            <p class="h5">Restauración <span class="fa fa-recycle"></span></p>
                        </div>

                        <div class="card-body">
                            <div class="form-row">
                                <div class="form-group col-md-12">
                                    <asp:FileUpload ID="fuRestore" runat="server" />
                                </div>
                            </div>

                            <div class="form-row">
                                <div class="form-group col-md-12">
                                    <asp:Button ID="RestoreBDButton" class="btn btn-primary w-100" runat="server" Text="Restaurar Base de Datos" OnClick="RestoreBDButton_Click" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
                <%--RESTAURACION--%>

                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header">
                            <div class="form-row">
                                <p class="h5">Respaldo <span class="fa fa-save"></span></p>
                            </div>
                        </div>
                        <div class="card-body">

                            <div class="form-group col-md-12">
                                <div class="form-row">
                                    <div class=" col-md-6">
                                        <label for="txtNombreRespaldo" class="float-right">Nombre del Respaldo:</label>
                                    </div>
                                    <div class=" col-md-6">
                                        <asp:TextBox ID="BkpNameTB" CssClass="w-100" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>


                            <div class="form-group col-md-12">
                                <asp:Button ID="BackUpBDButton" class="btn btn-primary w-100" runat="server" Text="Respaldar Base de Datos" OnClick="BackUpBDButton_Click" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group col-md-12">
                    <div class="card">
                        <div class="card-header">
                            <p class="h5 mb-4">Digitos Verificadores <span class="fa fa-user-lock"></span></p>
                        </div>
                        <div class="card-body">
                            <div class="form-group col-md-12">
                                <asp:Button ID="RecalculateDigitsBTN" class="btn btn-primary js-scroll-trigger w-100" runat="server" Text="Recalcular Digitos Verificadores" OnClick="RecalculateDigitsBTN_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    </div>
</asp:Content>
