<%@ Page Title="Roles" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="RolesView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Roles.RolesView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Roles</a>
            </li>
        </ol>

        <p class="h4 mb-4"><i class="fas fa-id-card-alt"></i>Roles</p>

        <div class="text-center border border-light p-5 w-100">

            <p class="h4 mb-4">Busqueda <span class="fa fa-search"></span></p>
            <div class="form-group w-100">
                <asp:DropDownList ID="RolesDLL" placeholder="Seleccione un Rol" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RolesDLL_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-id-card-alt"></i>
                Roles
                <div class="float-right">
                    <asp:LinkButton ID="ExportXMLButton" runat="server" ToolTip="Exportar XML" CssClass="btn btn-primary" OnClick="ExportXMLButton_Click"><i class="fas fa-file-export"></i></asp:LinkButton>
                    <asp:LinkButton ID="NewRolButton" runat="server" OnClick="NewRolButton_Click" ToolTip="Nuevo Rol" CssClass="btn btn-success"><i class="fas fa-plus"></i></asp:LinkButton>
                </div>

            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="RolesGridView" OnRowCommand="RolesGridView_RowCommand" class="table table-bordered" AutoGenerateColumns="false" runat="server" OnPageIndexChanging="RolesGridView_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Id" runat="server" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Nombre del Rol" runat="server" />
                            <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creacion" runat="server" />
                            <asp:BoundField DataField="UsuarioCreacion" HeaderText="Usuario Creación" runat="server" />
                            <asp:BoundField DataField="FechaModificacion" HeaderText="Fecha Modificación" runat="server" />
                            <asp:BoundField DataField="UsuarioModificacion" HeaderText="Usuario Modificación" runat="server" />

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ToolTip="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="E" CssClass="btn btn-secondary"><i class="fas fa-edit"></i> </asp:LinkButton>
                                    <asp:LinkButton runat="server" ToolTip="Eliminar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="D" CssClass="btn btn-danger"><i class="far fa-trash-alt"></i> </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <div class="card-footer small text-muted">
                Fecha actualización:
            <asp:Label ID="ActualizationDateLBL" runat="server" TextMode="DateTime"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
