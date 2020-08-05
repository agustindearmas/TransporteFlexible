<%@ Page Title="Ingreso" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ingreso.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Ingreso" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mastheadSinPadding">

        <div class="container border border-light p-5" id="formIngreso">

            <p class="h1 text-center" style="color: black">Ingreso</p>

            <div class="form-group">
                <asp:TextBox ID="UserNameTB" class="form-control mb-4" placeHolder="Nombre de Usuario" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="UserNameTBRFV" ValidationGroup="LoginVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="UserNameTB" runat="server"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <asp:TextBox ID="PasswordTB" TextMode="Password" class="form-control mb-4" placeHolder="Contraseña" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="PasswordRFV" ValidationGroup="LoginVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="PasswordTB" runat="server"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <asp:Button ID="BtnIngresar" ValidationGroup="LoginVG" class="btn btn-info btn-block my-4" runat="server" Text="Ingreso" OnClick="BtnIngresar_Click" />
            </div>

            <div class="form-row text-center">
                <div class="col-md-6">
                    <!-- Register -->
                    <p>
                        <a id="_registrarme" href="/#registro">¿No sos parte? Registrate.</a>
                    </p>
                </div>
                <div class="col-md-6">

                    <!-- Forgot password -->
                    <a id="_olvideContraseña" href="/">¿Olvidaste tu Contraseña?</a>

                </div>
            </div>
        </div>
    </div>

</asp:Content>
