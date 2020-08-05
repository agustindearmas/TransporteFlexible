<%@ Page Title="Rol" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="RolAMView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Roles.RolAMView" %>

<asp:Content ContentPlaceHolderID="MainContent" ID="BodyContent" runat="server">
    <div class="container-fluid">
        <!-- Breadcrumbs-->
        <div>
            <ol class="breadcrumb">
                <li class="breadcrumb-item">
                    <a href="../../Shared/Welcome.aspx">Bienvenida</a>
                </li>
                <li class="breadcrumb-item">
                    <a href="../../Seguridad/Roles/RolesView.aspx">Roles</a>
                </li>
                <li class="breadcrumb-item">
                    <a>Usuario</a>
                </li>
            </ol>
        </div>
        <div class="card mb-3">
            <div class="card-header">
                <asp:Label ID="TitleLBL" runat="server" CssClass="h4"><i class="fas fa-id-card-alt"></i> Edición de Rol: </asp:Label>
            </div>
            <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-12">
                        <label for="RolNameTB">Nombre del Rol</label>
                        <asp:TextBox ID="RolNameTB" class="form-control mb-4" placeHolder="Nombre del Rol" runat="server" MaxLength="14"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RolNameRFV" ControlToValidate="RolNameTB" ForeColor="Red" ValidationGroup="RoleVG" Display="Dynamic" ErrorMessage="Campo Requerido" runat="server"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-4">
                        <asp:Label runat="server" CssClass="h4">Permisos Asignados</asp:Label>
                        <asp:ListBox ID="AssignedListBox" CssClass="align-content-center col-md-12" runat="server"></asp:ListBox>
                    </div>
                    <div class="form-group col-md-4 text-center p-5">
                        <asp:LinkButton ID="AsignLinkButton" ToolTip="Asignar" CssClass="btn btn-warning" runat="server" OnClick="AsignLinkButton_Click"><span class="fa fa-arrow-left"/></asp:LinkButton>
                        <asp:LinkButton ID="UnsignLinkButton" ToolTip="Desasignar" CssClass="btn btn-warning" runat="server" OnClick="UnsignLinkButton_Click"><span class="fa fa-arrow-right"/></asp:LinkButton>
                    </div>
                    <div class="form-group col-md-4">
                        <asp:Label runat="server" CssClass="h4">Permisos sin Asignar</asp:Label>
                        <asp:ListBox ID="DeallocatedListBox" CssClass="align-content-center col-md-12" runat="server"></asp:ListBox>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-12 text-right">
                        <asp:Button ID="SaveRolButton" ValidationGroup="RoleVG" CssClass="btn btn-success" runat="server" Text="Guardar" OnClick="SaveRolButton_Click" />
                        <asp:Button ID="CancelRolButton" CssClass="btn btn-danger" runat="server" Text="Cancelar" OnClick="CancelRolButton_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
