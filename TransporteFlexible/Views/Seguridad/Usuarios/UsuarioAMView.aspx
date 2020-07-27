<%@ Page Title="" Language="C#" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="UsuarioAMView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Usuarios.UsuarioAMView" %>

<%@ Register Src="~/Views/Seguridad/Usuarios/WUCEmails.ascx" TagPrefix="UserControlEmails" TagName="WUCEmails" %>
<%@ Register Src="~/Views/Seguridad/Usuarios/WUCTelefonos.ascx" TagPrefix="UserControlTelefonos" TagName="WUCTelefonos" %>
<%@ Register Src="~/Views/Seguridad/Usuarios/WUCAddress.ascx" TagPrefix="UserControlAddresses" TagName="WUCAddress" %>



<asp:Content ContentPlaceHolderID="MainContent" ID="BodyContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <div>
            <ol class="breadcrumb">
                <li class="breadcrumb-item">
                    <a href="../../Shared/Bienvenida.aspx">Bienvenida</a>
                </li>
                <li class="breadcrumb-item">
                    <a href="../Usuarios/UsuarioView.aspx">Usuarios</a>
                </li>
                <li class="breadcrumb-item">
                    <a>Usuario</a>
                </li>
            </ol>
        </div>

        <%--USUARIO--%>
        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-user-shield"></i>
                Datos de Usuario
            </div>
            <div class="card-body">

                <div class="form-row">
                    <div class="form-group mb-4 col-md-4">
                        <label for="_tbNombreUsuario" style="color: black">Nombre Usuario</label><br />
                        <asp:TextBox ID="_tbNombreUsuario" ValidationGroup="RegistroFormValidator" class="form-control" placeHolder="Nombre de Usuario" runat="server" MaxLength="20"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="_tbNombreUsuarioRFV" ValidationGroup="RegistroFormValidator" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" ControlToValidate="_tbNombreUsuario" runat="server"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group col-md-4 mb-4">
                        <label for="_txtIntentos" runat="server" style="color: black">Intentos de Sesión</label><br />
                        <asp:TextBox ID="_tbIntentos" class="form-control" runat="server" Disabled="true"></asp:TextBox>

                    </div>

                    <div class="form-group col-md-4 mb-4">
                        <label for="_ddlActivo" style="color: black">Bloqueo</label><br />
                        <asp:DropDownList ID="_ddlActivo" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Activo" Value="True"></asp:ListItem>
                            <asp:ListItem Text="Inactivo" Value="False"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblUsuarioCreación" Style="color: black">Usuario Creación</asp:Label><br />
                        <asp:Label ID="_lblUsuarioCreación" runat="server" CssClass="form-control" Text="USUARIO CREACION"></asp:Label>
                    </div>

                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblFechaCreación" Style="color: black">Fecha de Creación</asp:Label><br />
                        <asp:Label ID="_lblFechaCreación" runat="server" CssClass="form-control" Text="FECHA CREACION"></asp:Label>
                    </div>

                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblHabilitado" Style="color: black">Usuario de Modificación</asp:Label><br />
                        <asp:Label ID="_lblUsuarioModificación" runat="server" CssClass="form-control" Text="USUARIO MODIFICACION"></asp:Label>
                    </div>

                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblBaja" Style="color: black">Fecha Modificación</asp:Label><br />
                        <asp:Label ID="_lblFechaModificacion" runat="server" CssClass="form-control" Text="FECHA MODIFICACION"></asp:Label>
                    </div>

                </div>

                <%--USUARIO--%>
            </div>

        </div>
        <%--USUARIO--%>

        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-address-card"></i>
                Datos Personales
            </div>
            <div class="card-body">

                <div class="form-row">
                    <div class=" form-group mb-4 col-md-6">
                        <label for="_tbNombre" style="color: black">Nombre</label><br />
                        <asp:TextBox ID="_tbNombre" ValidationGroup="PersonaFormValidator" class="form-control" placeHolder="Nombre" runat="server" MaxLength="20"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="_tbNombreRequiredValidator" ValidationGroup="PersonaFormValidator" ControlToValidate="_tbNombre" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" runat="server"></asp:RequiredFieldValidator>
                    </div>
                    <div class=" form-group mb-4 col-md-6">
                        <label for="_tbNombre" style="color: black">Apellido</label><br />
                        <asp:TextBox ID="_tbApellido" ValidationGroup="PersonaFormValidator" class="form-control" placeHolder="Apellido" runat="server" MaxLength="20"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="_tbApellidoRequiredValidator" ValidationGroup="PersonaFormValidator" ControlToValidate="_tbApellido" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" runat="server"></asp:RequiredFieldValidator>
                    </div>

                </div>

                <%--DNI CUIL CHECKBOX--%>
                <div class="form-row">
                    <div class="form-group col-md-4 mb-4">
                        <label for="_tbDni" style="color: black">DNI</label>
                        <asp:TextBox ID="_tbDni" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="DNI" runat="server" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="_tbDniRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbDni" runat="server"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="_tbDniREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="_tbDni" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                    </div>

                    <div class="form-group col-md-4 mb-4">
                        <label for="_tbCuil" style="color: black">CUIL</label>
                        <asp:TextBox ID="_tbCuil" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="Cuil" runat="server" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="_tbCuilRFV" ValidationGroup="RegistroFormValidator" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="_tbCuil" runat="server"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="_tbCuilREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="_tbCuil" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                    </div>

                    <div class="form-group col-md-4 mb-4">
                        <label for="_ddlEsCuit" style="color: black">¿Es CUIT?</label><br />
                        <asp:DropDownList ID="_ddlEsCuit" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Si" Value="True"></asp:ListItem>
                            <asp:ListItem Text="No" Value="False"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <%--DNI CUIL CHECKBOX--%>

                <%--AUDITORIA--%>
                <div class="form-row">
                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblUsuarioCreación" Style="color: black">Usuario Creación</asp:Label><br />
                        <asp:Label ID="_lblUsuarioCreacionPersona" runat="server" CssClass="form-control" Text="USUARIO CREACION"></asp:Label>
                    </div>

                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblFechaCreación" Style="color: black">Fecha de Creación</asp:Label><br />
                        <asp:Label ID="_lblFechaCreacionPersona" runat="server" CssClass="form-control" Text="FECHA CREACION"></asp:Label>
                    </div>

                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblHabilitado" Style="color: black">Usuario de Modificación</asp:Label><br />
                        <asp:Label ID="_lbUsuarioModificacionPersona" runat="server" CssClass="form-control" Text="USUARIO MODIFICACION"></asp:Label>
                    </div>

                    <div class="form-group col-md-3 mb-4">
                        <asp:Label runat="server" for="_lblBaja" Style="color: black">Fecha Modificación</asp:Label><br />
                        <asp:Label ID="_lblFechamodificacionPersona" runat="server" CssClass="form-control" Text="FECHA MODIFICACION"></asp:Label>
                    </div>

                </div>
                <%--AUDITORIA--%>
            </div>
            <%--PERSONA--%>



            <%--PERSONA--%>
        </div>

        <%--CONTACTO--%>
        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-address-book"></i>
                Datos de Contacto
            </div>
            <div class="card-body">

                <%--EMAILS--%>
                <UserControlEmails:WUCEmails runat="server" ID="WUCEmails" />
                <%--EMAILS--%>

                <%--TELEFONOS--%>
                <UserControlTelefonos:WUCTelefonos runat="server" ID="WUCTelefonos" />
                <%--TELEFONOS--%>

                <%--DIRECCIONES--%>
                <UserControlAddresses:WUCAddress runat="server" id="WUCAddress" />
                <%--DIRECCIONES--%>
            </div>
        </div>
        <%--CONTACTO--%>
    </div>
</asp:Content>
