<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="PermisoView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Usuarios.Permisos.PermisoView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="../../../Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a href="../UsuarioView.aspx">Usuarios</a>
            </li>
            <li class="breadcrumb-item">
                <a>Permisos</a>
            </li>
        </ol>



        <div class="text-left border border-light p-5 w-100" id="formRegistro">
            <asp:Label ID="Label1" runat="server" CssClass="h4 mb-4"><span class="fa fa-user"></span> Usuario: </asp:Label>
            <asp:Label ID="lblUser" runat="server" Text="" CssClass="h4 mb-4"></asp:Label>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-table"></i>
                Permisos
            </div>
            <div class="card-body">
                <div class="text-center border border-light p-5 w-100">

                    <div class="row">
                        <div class="col-md-4">
                            <asp:Label runat="server" CssClass="h4">Permisos Asignados</asp:Label>
                        </div>
                        <div class="col-md-4">
                        </div>
                        <div class="col-md-4">
                            <asp:Label runat="server" CssClass="h4">Permisos Sin Asignar</asp:Label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <asp:ListBox ID="lbxAsignados" CssClass="align-content-center col-md-12" runat="server"></asp:ListBox>
                        </div>
                        <div class="col-md-4">
                            <asp:LinkButton ID="lbAsignar" ToolTip="Asignar" CssClass="btn btn-warning" runat="server" OnClick="lbAsignar_Click"><span class="fa fa-arrow-left"/></asp:LinkButton>
                            <asp:LinkButton ID="lbDesasignar" ToolTip="Desasignar" CssClass="btn btn-warning" runat="server" OnClick="lbDesasignar_Click"><span class="fa fa-arrow-right"/></asp:LinkButton>
                        </div>
                        <div class="col-md-4">
                            <asp:ListBox ID="lbxDesasignados" CssClass="align-content-center col-md-12" runat="server"></asp:ListBox>
                        </div>
                    </div>

                </div>
                <br />
                <div style="float: right">
                    <asp:Button ID="btnGuardar" CssClass="btn btn-success" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnCancelar" CssClass="btn btn-secondary" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                </div>

            </div>

        </div>
    </div>
</asp:Content>
