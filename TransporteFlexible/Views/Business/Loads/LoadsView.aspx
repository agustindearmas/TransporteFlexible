<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="LoadsView.aspx.cs" Inherits="TransporteFlexible.Views.Business.Loads.LoadsView" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Cargas</a>
            </li>
        </ol>
        <asp:Label ID="TitleViewLBL" class="h4 mb-4" runat="server"><i class="fas fa-truck-loading"></i>  Mis Cargas</asp:Label>
        
        <div class="text-center border border-light p-5 w-100">

            <p class="h4 mb-4">Busqueda de Cargas <span class="fas fa-search"></span></p>
            <div class="form-group text-left w-100">
                <asp:DropDownList ID="LoadsDDL" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="LoadsDLL_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-pallet"></i>
                Cargas
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="LoadsGridView" ClientIDMode="Predictable" OnRowCommand="LoadsGridView_RowCommand" class="table table-bordered" AutoGenerateColumns="false" runat="server" OnPageIndexChanging="LoadsGridView_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Id" runat="server" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />

                            <asp:TemplateField HeaderText="Carga" >
                                <ItemTemplate>
                                    <asp:Label ID="Description" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Cantidad">
                                <ItemTemplate>
                                    <asp:Label ID="Quantity" runat="server" Text='<%# Bind("LoadDetail.Quantity") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Desde">
                                <ItemTemplate>
                                    <asp:Label ID="From" runat="server" Text='<%# Eval("AddressStart.Province.Description") + " " + Eval("AddressStart.Location.Description") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                         <asp:TemplateField HeaderText="Hasta">
                                <ItemTemplate>
                                    <asp:Label ID="To" runat="server" Text='<%# Eval("AddressEnd.Province.Description") + " " + Eval("AddressEnd.Location.Description") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="Oferta">
                                <ItemTemplate>
                                    <asp:TextBox ID="OfferTB" runat="server" Enabled="false"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="OfferREV" ValidationGroup="OfferVG" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="OfferTB" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                                </ItemTemplate>
                            </asp:TemplateField>

                            
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ToolTip="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Edicion" CssClass="btn btn-success"><i class="fas fa-pencil-alt"></i> </asp:LinkButton>
                                    <asp:LinkButton runat="server" ToolTip="Eliminar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Borrar" CssClass="btn btn-danger"><i class="fas fa-trash-alt"></i> </asp:LinkButton>
                                    <asp:LinkButton runat="server" ToolTip="Ofertas" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Ofertas" CssClass="btn btn-primary"><i class="fas fa-dollar-sign"></i> </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="EditOffer" runat="server" ToolTip="Ofertar a Carga" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="EditOffer" CssClass="btn btn-success"><i class="fas fa-dollar-sign"></i> </asp:LinkButton>
                                    <asp:LinkButton ID="OfferInfo" runat="server" ToolTip="Ver Información" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Informacion" CssClass="btn btn-secondary"><i class="fas fa-info"></i> </asp:LinkButton>

                                    <asp:LinkButton ID="Offer" ValidationGroup="OfferVG" Visible="false" runat="server" ToolTip="Ofertar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Ofertar" CssClass="btn btn-success"><i class="fas fa-dollar-sign"></i> </asp:LinkButton>
                                    <asp:LinkButton ID="CancelarOferta" Visible="false" runat="server" ToolTip="Cancelar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="CancelarOferta" CssClass="btn btn-danger"><i class="fas fa-arrow-right"></i> </asp:LinkButton>
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
