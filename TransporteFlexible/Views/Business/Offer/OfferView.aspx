<%@ Page Title="Ofertas" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="OfferView.aspx.cs" Inherits="TransporteFlexible.Views.Business.Offer.OfferView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Ofertas</a>
            </li>
        </ol>

        <asp:Label ID="CardHeaderLBL" CssClass="h4 mb-4" runat="server"><i class="fas fa-dollar-sign"></i>  Ofertas</asp:Label>

        
        <div class="text-center border border-light p-5 w-100">

            <p class="h4 mb-4">Busqueda de Ofertas <span class="fa fa-search"></span></p>
            <div class="form-group text-left w-100">
                <asp:Label runat="server">Busqueda de Ofertas por Carga</asp:Label>
                <asp:DropDownList ID="LoadsDDL" placeholder="Seleccione una Carga" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="LoadsDLL_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-dollar-sign"></i>
                Ofertas
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="OffersGridView" ClientIDMode="Predictable" OnRowCommand="OffersGridView_RowCommand" class="table table-bordered" AutoGenerateColumns="false" runat="server" OnPageIndexChanging="OffersGridView_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Id" runat="server" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />

                            <asp:TemplateField HeaderText="Carga">
                                <ItemTemplate>
                                    <asp:Label ID="LoadDescription" runat="server" Text='<%# Bind("Load.Description") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:BoundField DataField="Price" HeaderText="Precio" runat="server" DataFormatString="{0:c}"/>
                            
                            <asp:TemplateField HeaderText="Transportista">
                                <ItemTemplate>
                                    <asp:Label ID="CarrierUserName" runat="server" Text='<%# Bind("CarrierUser.NombreUsuario") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="Status" runat="server" Text='<%# Bind("OfferStatus.Descripcion") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="Aceptar" ToolTip="Aceptar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Aceptar" CssClass="btn btn-success"><i class="fas fa-check-circle"></i> </asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="Rechazar" ToolTip="Rechazar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Rechazar" CssClass="btn btn-danger"><i class="fas fa-ban"></i> </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="Eliminar" ToolTip="Eliminar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="EliminarOferta" CssClass="btn btn-danger"><i class="fas fa-trash-alt"></i> </asp:LinkButton>
                                    
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
