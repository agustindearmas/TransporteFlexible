    <%@ Page Title="Ingreso" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ingreso.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Ingreso" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">    
    <div class="mastheadSinPadding">

        <div class="container text-center border border-light p-5" id="formIngreso">

            <p class="h4 mb-4" style="color:goldenrod">Ingresar</p>

            <div class="form-group">                
                <asp:TextBox ID="_tbNombreUsuario" class="form-control mb-4" placeHolder="Nombre de Usuario" runat="server"></asp:TextBox>
            </div>

            <div class="form-group">
                <%--<asp:Label ID="lblContraseña" runat="server" Text="Contraseña"></asp:Label>--%>
                <asp:TextBox ID="_tbContraseña" TextMode="Password" class="form-control mb-4" placeHolder="Contraseña" runat="server"></asp:TextBox>
            </div>

            <div class="d-flex justify-content-around">
                
                <div>
                    <!-- Forgot password -->
                    <a id="_olvideContraseña" href="/">¿Olvidaste tu Contraseña?</a>
                </div>
            </div>

            <div class="form-group">
                <asp:Button ID="BtnIngresar" class="btn btn-info btn-block my-4" runat="server" Text="Log In" OnClick="BtnIngresar_Click" />
            </div>

            <!-- Register -->
            <p><a id="_registrarme" href="/#registro">¿No sos parte? Registrate.</a>
            </p>
        </div>
    </div>

</asp:Content>
