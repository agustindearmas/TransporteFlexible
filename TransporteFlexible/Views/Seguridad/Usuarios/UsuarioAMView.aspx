<%@ Page Title="Usuario" Language="C#" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="UsuarioAMView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Usuarios.UsuarioAMView" %>

<%@ Register Src="~/Views/Seguridad/Usuarios/WUCEmails.ascx" TagPrefix="UserControlEmails" TagName="WUCEmails" %>
<%@ Register Src="~/Views/Seguridad/Usuarios/WUCTelefonos.ascx" TagPrefix="UserControlTelefonos" TagName="WUCTelefonos" %>
<%@ Register Src="~/Views/Seguridad/Usuarios/WUCAddress.ascx" TagPrefix="UserControlAddresses" TagName="WUCAddress" %>

<asp:Content ContentPlaceHolderID="MainContent" ID="BodyContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <div>
            <ol class="breadcrumb">
                <li class="breadcrumb-item">
                    <a href="../../Shared/Welcome.aspx">Bienvenida</a>
                </li>
                <li class="breadcrumb-item">
                    <a href="../Usuarios/UsuarioView.aspx">Usuarios</a>
                </li>
                <li class="breadcrumb-item">
                    <a>Usuario</a>
                </li>
            </ol>
        </div>

        <div class="form-row">
            <div class="form-group mb-6 col-md-6">
                <asp:Label ID="TitleLBL" runat="server" CssClass="h4"><span class="fa fa-user"></span> Edición de Usuario: </asp:Label>
                <asp:Label ID="UserNameTitleTB" runat="server" CssClass="h4"></asp:Label>
            </div>
            <asp:Panel runat="server" CssClass="form-group mb-6 col-md-6" ID="UserAddButtonsPNL" Visible="false">
                <div class="mb-12 col-md-12">
                    <div class="float-right">
                        <asp:LinkButton ID="AddNewUserButton" runat="server" ValidationGroup="PersonVG" ToolTip="Guardar Cambios" CssClass="btn btn-secondary" OnClick="AddNewUserButton_Click">Guardar</asp:LinkButton>
                        <asp:LinkButton ID="CancelAddNewUserButton" runat="server" ToolTip="Cancelar" CssClass="btn btn-danger" OnClick="CancelAddNewUserButton_Click">Cancelar</asp:LinkButton>
                    </div>
                </div>

            </asp:Panel>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-address-card"></i>
                <asp:Label ID="PersonCardLBL" runat="server" Text="Datos Personales"></asp:Label>

                <asp:Panel runat="server" CssClass="form-row  float-right" ID="PersonButtonsPNL">
                        <div class="form-group mb-6 col-md-6">
                            <asp:LinkButton ID="SavePersonButton" runat="server" ValidationGroup="PersonVG" ToolTip="Guardar Cambios" CssClass="btn btn-secondary" OnClick="SavePersonButton_Click">Guardar</asp:LinkButton>
                        </div>
                        <div class="form-group mb-6 col-md-6">
                            <asp:LinkButton ID="CancelPersonButton" runat="server" ToolTip="Cancelar Cambios" CssClass="btn btn-danger" OnClick="CancelPersonButton_Click">Cancelar</asp:LinkButton>
                        </div>
                </asp:Panel>
                <asp:HiddenField runat="server" ID="PersonIdHF" Value="0" />
            </div>
            <div class="card-body">

                <div class="form-row">
                    <div class=" form-group mb-4 col-md-6">
                        <label for="PersonNameTB" style="color: black">Nombre</label><br />
                        <asp:TextBox ID="PersonNameTB" ValidationGroup="PersonaFormValidator" class="form-control" placeHolder="Nombre" runat="server" MaxLength="20"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PersonNameRFV" ValidationGroup="PersonVG" ControlToValidate="PersonNameTB" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" runat="server"></asp:RequiredFieldValidator>
                    </div>
                    <div class=" form-group mb-4 col-md-6">
                        <label for="PersonLastNameTB" style="color: black">Apellido</label><br />
                        <asp:TextBox ID="PersonLastNameTB" ValidationGroup="PersonVG" class="form-control" placeHolder="Apellido" runat="server" MaxLength="20"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="_tbApellidoRequiredValidator" ValidationGroup="PersonVG" ControlToValidate="PersonLastNameTB" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" runat="server"></asp:RequiredFieldValidator>
                    </div>

                </div>

                <%--DNI CUIL CHECKBOX--%>
                <div class="form-row">
                    <div class="form-group col-md-4 mb-4">
                        <label for="PersonDniTB" style="color: black">DNI</label>
                        <asp:TextBox ID="PersonDniTB" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="DNI" runat="server" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PersonDniRFV" ValidationGroup="PersonVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="PersonDniTB" runat="server"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="PersonDniREV" ValidationGroup="PersonVG" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="PersonDniTB" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                    </div>

                    <div class="form-group col-md-4 mb-4">
                        <label for="PersonCuilTB" style="color: black">CUIL</label>
                        <asp:TextBox ID="PersonCuilTB" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="Cuil" runat="server" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PersonCuilRFV" ValidationGroup="PersonVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="PersonCuilTB" runat="server"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="PersonCuilREV" ValidationGroup="PersonVG" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="PersonCuilTB" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                    </div>

                    <div class="form-group col-md-4 mb-4">
                        <label for="PersonCuitDLL" style="color: black">¿Es CUIT?</label><br />
                        <asp:DropDownList ID="PersonCuitDLL" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Si" Value="True"></asp:ListItem>
                            <asp:ListItem Text="No" Value="False"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <%--DNI CUIL CHECKBOX--%>

                <%--CONTACTOS TELEFONO EMAIL ONLY USED IN ADD MODE--%>
                <asp:Panel runat="server" ID="ContactAddPNL" Visible="false">
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label for="PersonEmailTB" style="color: black">EMAIL</label>
                            <asp:TextBox ID="PersonEmailTB" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="EMAIL" runat="server" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PersonEmailRFV" ValidationGroup="PersonVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="PersonEmailTB" runat="server"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="PersonEmailREV" ValidationGroup="PersonVG" ForeColor="Red" runat="server" ErrorMessage="xxxx@xxxx.com" ControlToValidate="PersonEmailTB" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                        </div>

                        <div class="form-group col-md-6">
                            <label for="PersonPhoneTB" style="color: black">TELÉFONO</label>
                            <asp:TextBox ID="PersonPhoneTB" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="TELEFONO" runat="server" MaxLength="15"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PersonPhoneRFV" ValidationGroup="PersonVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="PersonPhoneTB" runat="server"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="PersonPhoneREV" ValidationGroup="PersonVG" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="PersonPhoneTB" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                        </div>
                    </div>
                </asp:Panel>
                <%--CONTACTOS TELEFONO EMAIL--%>

                <%--User Info ONLY USED IN ADD MODE--%>
                <asp:Panel runat="server" ID="UserInfoPNL" Visible="false">
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label for="UserNameAddTB" style="color: black">Nombre de Usuario</label><br />
                            <asp:TextBox ID="UserNameAddTB" ValidationGroup="PersonVG" class="form-control" placeHolder="Nombre de Usuario" runat="server" MaxLength="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="UserNameAddRFV" ValidationGroup="PersonVG" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" ControlToValidate="UserNameAddTB" runat="server"></asp:RequiredFieldValidator>
                        </div>

                        <div class="form-group col-md-6">
                           <label for="RolDLL" style="color: black">Rol</label><br />
                        <asp:DropDownList ID="RolDLL" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>
                <%--User Info--%>

                <%--AUDITORIA--%>
                <asp:Panel ID="AuditPersonFieldsPNL" runat="server">
                    <div class="form-row">
                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="PersonCreationUserLBL" Style="color: black">Usuario Creación</asp:Label><br />
                            <asp:Label ID="PersonCreationUserLBL" runat="server" CssClass="form-control" Text="USUARIO CREACION"></asp:Label>
                        </div>

                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="PersonCreationDateLBL" Style="color: black">Fecha de Creación</asp:Label><br />
                            <asp:Label ID="PersonCreationDateLBL" runat="server" CssClass="form-control" Text="FECHA CREACION"></asp:Label>
                        </div>

                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="PersonModificationUserLBL" Style="color: black">Usuario de Modificación</asp:Label><br />
                            <asp:Label ID="PersonModificationUserLBL" runat="server" CssClass="form-control" Text="USUARIO MODIFICACION"></asp:Label>
                        </div>

                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="PersonModificationDateLBL" Style="color: black">Fecha Modificación</asp:Label><br />
                            <asp:Label ID="PersonModificationDateLBL" runat="server" CssClass="form-control" Text="FECHA MODIFICACION"></asp:Label>
                        </div>

                    </div>
                </asp:Panel>
                <%--AUDITORIA--%>
            </div>
            <%--PERSONA--%>



            <%--PERSONA--%>
        </div>

        <asp:Panel ID="UserFieldsPNL" runat="server">
            <%--USUARIO--%>
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fas fa-user-shield"></i>
                    Datos de Usuario
               
                    <div class="form-row  float-right">
                        <div class="form-group mb-6 col-md-6">
                            <asp:LinkButton ID="SaveUserButton" runat="server" ToolTip="Guardar Cambios" CssClass="btn btn-secondary" ValidationGroup="UserVG" OnClick="SaveUserButton_Click">Guardar</asp:LinkButton>
                        </div>
                        <div class="form-group mb-6 col-md-6">
                            <asp:LinkButton ID="CancelUserButton" runat="server" ToolTip="Cancelar Cambios" CssClass="btn btn-danger" OnClick="CancelUserButton_Click">Cancelar</asp:LinkButton>
                        </div>
                    </div>
                    <asp:HiddenField runat="server" ID="UserIdHF" Value="0" />
                </div>
                <div class="card-body">

                    <div class="form-row">
                        <div class="form-group mb-4 col-md-4">
                            <label for="UserNameTB" style="color: black">Nombre Usuario</label><br />
                            <asp:TextBox ID="UserNameTB" ValidationGroup="UserVG" class="form-control" placeHolder="Nombre de Usuario" runat="server" MaxLength="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="UserNameRFV" ControlToValidate="UserNameTB" ValidationGroup="UserVG" Display="Dynamic" ForeColor="Red" ErrorMessage="Campo Requerido" runat="server"></asp:RequiredFieldValidator>
                        </div>

                        <div class="form-group col-md-4 mb-4">
                            <label for="RolDLL" style="color: black">Rol</label><br />
                        <asp:DropDownList ID="RoleEditDLL" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        </div>

                        <div class="form-group col-md-4 mb-4">
                            <label for="BlockDLL" style="color: black">Activo</label><br />
                            <asp:DropDownList ID="BlockDLL" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Activo" Value="True"></asp:ListItem>
                                <asp:ListItem Text="Inactivo" Value="False"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="CreationUserNameLBL" Style="color: black">Usuario Creación</asp:Label><br />
                            <asp:Label ID="CreationUserNameLBL" runat="server" CssClass="form-control" Text="USUARIO CREACION"></asp:Label>
                        </div>

                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="CreationDateLBL" Style="color: black">Fecha de Creación</asp:Label><br />
                            <asp:Label ID="CreationDateLBL" runat="server" CssClass="form-control" Text="FECHA CREACION"></asp:Label>
                        </div>

                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="ModifyUserNameLBL" Style="color: black">Usuario de Modificación</asp:Label><br />
                            <asp:Label ID="ModifyUserNameLBL" runat="server" CssClass="form-control" Text="USUARIO MODIFICACION"></asp:Label>
                        </div>

                        <div class="form-group col-md-3 mb-4">
                            <asp:Label runat="server" for="ModificationDateLBL" Style="color: black">Fecha Modificación</asp:Label><br />
                            <asp:Label ID="ModificationDateLBL" runat="server" CssClass="form-control" Text="FECHA MODIFICACION"></asp:Label>
                        </div>

                    </div>
                    <%--USUARIO--%>
                </div>
            </div>
        </asp:Panel>

    </div>
    <%--USUARIO--%>

    <%--CONTACTO--%>
    <asp:Panel runat="server" ID="ContactInfoPNL">
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
                <UserControlAddresses:WUCAddress runat="server" ID="WUCAddress" />
                <%--DIRECCIONES--%>
            </div>
        </div>
    </asp:Panel>
    <%--CONTACTO--%>
    </div>
</asp:Content>
