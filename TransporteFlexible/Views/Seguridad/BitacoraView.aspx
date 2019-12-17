<%@ Page Title="Bitacora" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="BitacoraView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.BitacoraView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Bienvenida.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Bitacora</a>
            </li>
        </ol>

    </div>
    <div class="text-center border border-light p-5 w-100" id="formRegistro">

        <p class="h4 mb-4">Busqueda <span class="fa fa-search"></span></p>

        <div class="form-row">
            <div class="form-group col-md-6">
                <label for="txtFechaDesde">Desde</label>
                <asp:TextBox ID="_txtFechaDesde" class="form-control mb-4" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
            </div>
            <div class="form-group col-md-6">
                <label for="txtFechaHasta">Hasta</label>
                <asp:TextBox ID="_txtFechaHasta" class="form-control mb-4" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-4">
                <label for="_ddlNivelCriticidad" class="position-static">Nivel de Criticidad</label>
                <asp:DropDownList ID="_ddlNivelCriticidad" class="form-control mb-4" runat="server">
                </asp:DropDownList>
            </div>

            <div class="form-group col-md-4">
                <label for="_txtEvento" class="position-static">Evento</label>
                <asp:TextBox ID="_txtEvento" class="form-control mb-4" placeholder="Evento" runat="server"></asp:TextBox>
            </div>            

            <div class="form-group col-md-4">
                <label for="_txtUsuario">Usuario</label>
                <asp:TextBox ID="_txtUsuario" class="form-control mb-4" placeholder="Usuario" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-group w-100">
            <asp:Button ID="btnFiltrarBita" class="btn btn-info btn-block my-4" runat="server" Text="Filtrar Bitácora" OnClick="btnFiltrarBita_Click" />
        </div>
    </div>



    <div class="card mb-3">
        <div class="card-header">
            <i class="fas fa-table"></i>
            Bitácora de Sucesos
        </div>
        <div class="card-body">
            <div class="table-responsive">
                <asp:GridView ID="_bitacoraGridView" class="table table-bordered" runat="server" OnPageIndexChanging="_bitacoraGridView_PageIndexChanging"></asp:GridView>
            </div>
        </div>
        <div class="card-footer small text-muted">Fecha actualización:
            <asp:Label ID="_lblFechaActualizacion" runat="server" TextMode="DateTime"></asp:Label></div>
    </div>
</asp:Content>
