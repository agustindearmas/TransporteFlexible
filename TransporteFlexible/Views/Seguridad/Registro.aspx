<%@ Page Title="Registro" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Registro" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="mastheadSinPadding">

        <div class="container text-center border border-light p-5" id="formRegistro">

            <p class="h4 mb-4" style="color: white">Registrarse</p>

            <%--NOMBRE Y APELLIDO--%>
            <div class="form-row">
                <div class="form-group col-md-6">
                    <label for="_tbNombre" style="color: white">Nombre</label>
                    <asp:TextBox  ID="_tbNombre" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="Nombre"  runat="server" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbNombreRFV" ForeColor="Red" ValidationGroup="RegistroFormValidator" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbNombre" runat="server"></asp:RequiredFieldValidator>
                    <%--<asp:RegularExpressionValidator ID="_tbNombreREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo se permiten letras" ControlToValidate="_tbNombre" ValidationExpression="^[A-Za-z]*$"></asp:RegularExpressionValidator>--%>
                </div>

                <div class="form-group col-md-6">
                    <label for="_tbApellido" style="color: white">Apellido</label>
                    <asp:TextBox ID="_tbApellido" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="Apellido" runat="server" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbApellidoRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbApellido" runat="server"></asp:RequiredFieldValidator>
                    <%--<asp:RegularExpressionValidator ID="_tbApellidoREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo se permiten letras" ControlToValidate="_tbApellido" ValidationExpression="^[A-Za-z]*$"></asp:RegularExpressionValidator>--%>
                </div>
            </div>
            <%--NOMBRE Y APELLIDO--%>

            <%--DNI CUIL CHECKBOX--%>
            <div class="form-row">
                <div class="form-group col-md-4">
                    <label for="_tbDni" style="color: white">DNI</label>
                    <asp:TextBox ID="_tbDni" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="DNI" runat="server" MaxLength="15"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbDniRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbDni" runat="server"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="_tbDniREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="_tbDni" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                </div>

                <div class="form-group col-md-4">
                    <label for="_tbCuil" style="color: white">CUIL</label>
                    <asp:TextBox ID="_tbCuil" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="Cuil" runat="server" MaxLength="15"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbCuilRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbCuil" runat="server"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="_tbCuilREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="_tbCuil" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                </div>

                <div class="form-group col-md-4">
                    <label for="_cbEsCuit" style="color: white">¿Es CUIT?</label><br />
                    <asp:CheckBox ID="_cbEsCuit" runat="server" />
                </div>
            </div>
            <%--DNI CUIL CHECKBOX--%>

            <%--CONTACTOS TELEFONO EMAIL--%>
            <div class="form-row">
                <div class="form-group col-md-6">
                    <label for="_tbEmail" style="color: white">EMAIL</label>
                    <asp:TextBox ID="_tbEmail" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="EMAIL" runat="server" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbEmailRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbEmail" runat="server"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="_tbEmailREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="xxxx@xxxx.com" ControlToValidate="_tbEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                </div>

                <div class="form-group col-md-6">
                    <label for="_tbTelefono" style="color: white">TELÉFONO</label>
                    <asp:TextBox ID="_tbTelefono" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="TELEFONO" runat="server" MaxLength="15"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbTelefonoRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbTelefono" runat="server"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="_tbTelefonoREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="_tbTelefono" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                </div>
            </div>
            <%--CONTACTOS TELEFONO EMAIL--%>

            <%--USUARIO USERNAME PASSWORD--%>
            <div class="form-row">
                <div class="form-group col-md-4">
                    <label for="_tbNombreUsuario" style="color: white">Nombre de Usuario</label><br />
                    <asp:TextBox ID="_tbNombreUsuario" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="Nombre de Usuario" runat="server" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbNombreUsuarioRFV" ValidationGroup="RegistroFormValidator" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" ControlToValidate="_tbNombreUsuario" runat="server"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group col-md-4">
                    <label for="_tbContraseña" style="color: white">Contraseña</label><br />
                    <asp:TextBox ID="_tbContraseña" ValidationGroup="RegistroFormValidator" class="form-control mb-4" TextMode="Password" placeHolder="Contraseña" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbContraseñaRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbContraseña" runat="server"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="_tbContraseñaREV" ControlToValidate="_tbContraseña" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server"
                        ErrorMessage="La contraseña debe medir entre 8 y 20 caracteres y debe contar, al menos, con una letra mayuscula, una letra minuscula, un numero y algún caracter especial"
                        ValidationExpression="(?=^.{6,255}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*">
                    </asp:RegularExpressionValidator>
                </div>

                <div class="form-group col-md-4">
                    <label for="_tbRepetirContraseña" style="color: white">Repetir Contraseña</label><br />
                    <asp:TextBox ID="_tbRepetirContraseña" ValidationGroup="RegistroFormValidator" class="form-control mb-4" TextMode="Password" placeHolder="Repetir Contraseña" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="_tbRepetirContraseñaRFV" ValidationGroup="RegistroFormValidator" Display="Dynamic" ErrorMessage="Campo Requerido" ForeColor="Red" ControlToValidate="_tbRepetirContraseña" runat="server"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="_tbRepetirContraseñaREV" ControlToValidate="_tbRepetirContraseña" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server"
                        ErrorMessage="La contraseña debe medir entre 8 y 20 caracteres y debe contar, al menos, con una letra mayuscula, una letra minuscula, un numero y algún caracter especial"
                        ValidationExpression="(?=^.{6,255}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*">
                    </asp:RegularExpressionValidator>
                </div>
            </div>
            <%--USUARIO USERNAME PASSWORD--%>

            <%--BOTON--%>
            <div class="form-group">
                <asp:Button ID="_btnRegistrarme" class="btn btn-info btn-block my-4" runat="server"  ValidationGroup="RegistroFormValidator" Text="Registrarme" OnClick="BtnRegistrarme_Click" />
            </div>
            <%--BOTON--%>
        </div>
    </div>
</asp:Content>
