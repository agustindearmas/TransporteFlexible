<%@ Page Title="Bitacora" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="BitacoraView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.BitacoraView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Bitacora</a>
            </li>
        </ol>

        <p class="h4 mb-4"><i class="fas fa-book-open"></i>Bitácora de Sucesos</p>

        <div class="text-center border border-light p-5 w-100">

            <p class="h4 mb-4">Busqueda <span class="fa fa-search"></span></p>

            <div class="form-row">
                <div class="form-group col-md-6">
                    <label for="DateFromTB">Desde</label>
                    <asp:TextBox ID="DateFromTB" class="form-control" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
                </div>
                <div class="form-group col-md-6">
                    <label for="DateToTB">Hasta</label>
                    <asp:TextBox ID="DateToTB" class="form-control" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group col-md-3">
                    <label for="CriticallyLevelDLL" class="position-static">Nivel de Criticidad</label>
                    <asp:DropDownList ID="CriticallyLevelDLL" class="form-control" runat="server">
                    </asp:DropDownList>
                </div>

                <div class="form-group col-md-3">
                    <label for="EventTB" class="position-static">Evento</label>
                    <asp:TextBox ID="EventTB" class="form-control mb-4" placeholder="Evento" runat="server"></asp:TextBox>
                </div>

                <div class="form-group col-md-3">
                    <label for="UserNameTB">Nombre de Usuario</label>
                    <asp:TextBox ID="UserNameTB" class="form-control" placeholder="Usuario" runat="server"></asp:TextBox>
                </div>
                <div class="form-group col-md-3">
                    <label for="UserNameTB">Suceso Dado de Baja</label>
                    <asp:CheckBox ID="DownCBX" CssClass="form-control" runat="server" />
                </div>

            </div>
            <div class="form-group w-100">
                <asp:Button ID="FilterBinnacleBTN" class="btn btn-info btn-block my-4" runat="server" Text="Filtrar Bitácora" OnClick="FilterBinnacleBTN_Click" />
            </div>
        </div>



        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-table"></i>
                Bitácora de Sucesos
                <div class="float-right">
                    <asp:LinkButton ID="ExportXMLButton" runat="server" ToolTip="Exportar XML" CssClass="btn btn-primary" OnClick="ExportXMLButton_Click"><i class="fas fa-file-export"></i></asp:LinkButton>
                </div>

            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="BinnacleGridView" OnRowDeleting="BinnacleGridView_RowDeleting" class="table table-bordered" AutoGenerateColumns="false" runat="server" OnPageIndexChanging="BinnacleGridView_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="Id" runat="server" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />

                            <asp:BoundField DataField="Evento" HeaderText="Evento" runat="server" />
                            <asp:BoundField DataField="Suceso" HeaderText="Suceso" runat="server" />
                            <asp:BoundField DataField="NivelCriticidad" HeaderText="Nivel Criticidad" runat="server" />
                            <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creacion" runat="server" />
                            <asp:BoundField DataField="UsuarioCreacion" HeaderText="Usuario Creacion" runat="server" />

                            <asp:TemplateField HeaderText="Acciones">

                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Delete" CssClass="btn btn-danger">
                                    <div  title="<%# (bool)Eval("Baja") ? "Alta" : "Baja" %>">
                                    <i class="<%#(bool)Eval("Baja") ? "fas fa-arrow-up" : "fas fa-trash-alt"%>"></i> 
                                    </div>
                                    </asp:LinkButton>
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
