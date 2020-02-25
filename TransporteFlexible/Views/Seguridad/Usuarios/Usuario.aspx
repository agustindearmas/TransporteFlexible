<%@ Page Title="Usuario" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Usuario.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Usuarios.Usuario" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   <div class="container-fluid">

        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Bienvenida.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Usuarios</a>
            </li>
        </ol>

    </div>
    <div class="text-center border border-light p-5 w-100" id="formRegistro">

        <p class="h4 mb-4">Busqueda <span class="fa fa-search"></span></p>

        <div class="form-row">

            <div class="form-group col-md-4">
                <label for="_txtIdUsuario" class="position-static">Id Usuario</label>
                <asp:TextBox ID="_tbIdUsuario" class="form-control mb-4" placeholder="Id" runat="server"></asp:TextBox>
            </div>            

            <div class="form-group col-md-4">
                <label for="_txtNombreUsuario">Usuario</label>
                <asp:TextBox ID="_tbNombreUsuario" ValidationGroup="RegistroFormValidator" placeHolder="Nombre de Usuario" runat="server" MaxLength="20"></asp:TextBox>
            </div>

            <div class="form-group col-md-4">
                    <label for="_tbDni" style="color: white">DNI</label>
                    <asp:TextBox ID="_tbDni" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="DNI" runat="server" MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="_tbDniREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="_tbDni" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                </div>


        </div>
        <div class="form-group w-100">
            <asp:Button ID="btnFiltrarUsuarios" class="btn btn-info btn-block my-4" runat="server" Text="Filtrar Usuario" />
        </div>
    </div>



    <div class="card mb-3">
        <div class="card-header">
            <i class="fas fa-table"></i>
            Usuarios
        </div>
        <div class="card-body">
            <div class="table-responsive">
                <asp:GridView ID="_usuariosGridView" class="table table-bordered" runat="server"></asp:GridView>
            </div>
        </div>
        <div class="card-footer small text-muted">Fecha actualización:
            <asp:Label ID="_lblFechaActualizacion" runat="server" TextMode="DateTime"></asp:Label></div>
    </div>
</asp:Content>
