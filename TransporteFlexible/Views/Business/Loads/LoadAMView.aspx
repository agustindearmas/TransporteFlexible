<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="LoadAMView.aspx.cs" Inherits="TransporteFlexible.Views.Business.Loads.LoadAMView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="/Views/Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Carga Alta Modificación</a>
            </li>
        </ol>



        <div class="card mb-3">
            <div class="card-header">
                <div class="form-row">
                    <div class="form-group text-left col-md-6">
                        <asp:Label runat="server" CssClass="h4 mb-4"> Edición de Carga  <i class="fas fa-pallet"></i></asp:Label>
                    </div>

                    <div class="mb-12 col-md-6">
                        <div class="float-right">
                            <asp:LinkButton ID="Save" runat="server" ValidationGroup="LoadVG" ToolTip="Guardar Carga" OnClick="Save_Click" CssClass="btn btn-secondary">Guardar</asp:LinkButton>
                            <asp:LinkButton ID="Cancelar" runat="server" ToolTip="Cancelar Carga" CssClass="btn btn-danger" OnClick="Cancelar_Click">Cancelar</asp:LinkButton>
                        </div>
                    </div>
                </div>

            </div>
            <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-6">
                        <label for="ProductDDL" style="color: black">Indole</label>
                        <asp:DropDownList ID="ProductDDL" runat="server" CssClass="form-control" AutoPostBack="true">
                            <asp:ListItem Value="0" Text="N/A" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Agropecuaria"></asp:ListItem>
                            <asp:ListItem Value="3" Text="Automotriz"></asp:ListItem>
                            <asp:ListItem Value="4" Text="Industria"></asp:ListItem>
                            <asp:ListItem Value="5" Text="Construcción"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="PackageDDL" style="color: black">Empaque</label>
                        <asp:DropDownList ID="PackageDDL" runat="server" CssClass="form-control" AutoPostBack="true">
                            <asp:ListItem Value="0" Text="N/A" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Paletizado"></asp:ListItem>
                            <asp:ListItem Value="3" Text="Granel"></asp:ListItem>
                            <asp:ListItem Value="4" Text="Suelto"></asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="ProductTB" style="color: black">Nombre del Item o Producto</label>
                        <asp:TextBox runat="server" ID="ProductTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ProductRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="ProductTB" runat="server"></asp:RequiredFieldValidator>
                    </div>
                </div>


                <div class="form-row">
                    <div class="form-group col-md-6">
                        <label for="DescriptionTB" style="color: black">Descripción</label>
                        <asp:TextBox runat="server" ID="DescriptionTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="DescriptionRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="DescriptionTB" runat="server"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="UnitDDL" style="color: black">Unidad</label>
                        <asp:DropDownList ID="UnitDDL" runat="server" CssClass="form-control" AutoPostBack="true">
                            <asp:ListItem Value="0" Text="N/A" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Unidad"></asp:ListItem>
                            <asp:ListItem Value="3" Text="Kilo"></asp:ListItem>
                            <asp:ListItem Value="4" Text="Litro"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="QuantityTB" style="color: black">Cantidad</label>
                        <asp:TextBox runat="server" ID="QuantityTB" CssClass="form-control"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="QuantityREV" ValidationGroup="LoadVG" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="QuantityTB" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                        <asp:RequiredFieldValidator ID="QuantityRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="QuantityTB" runat="server"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group col-md-12">
                        <label for="ObservationsTB" style="color: black">Observaciones</label>
                        <asp:TextBox runat="server" ID="ObservationsTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ObservationsRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="ObservationsTB" runat="server"></asp:RequiredFieldValidator>
                    </div>
                </div>

            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <asp:Label runat="server" CssClass="h4 mb-4"> Origen  <i class="fas fa-location-arrow"></i></asp:Label>
            </div>
            <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-4">
                        <label for="ProvinceStartDDL" style="color: black">Provincia</label>
                        <asp:DropDownList ID="ProvinceStartDDL" OnSelectedIndexChanged="ProvinceStartDDL_SelectedIndexChanged" runat="server" CssClass="form-control" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-4">
                        <label for="LocationStartDDL" style="color: black">Localidad</label>
                        <asp:DropDownList ID="LocationStartDDL" runat="server" CssClass="form-control" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-4">
                        <label for="StartPostalCodeTB" style="color: black">Codigo Postal</label>
                        <asp:TextBox runat="server" ID="StartPostalCodeTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="StartPostalCodeRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="StartPostalCodeTB" runat="server"></asp:RequiredFieldValidator>
                    </div>
                </div>


                <div class="form-row">
                    <div class="form-group col-md-3">
                        <label for="StartStreetTB" style="color: black">Calle</label>
                        <asp:TextBox runat="server" ID="StartStreetTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="StartStreetRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="StartStreetTB" runat="server"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="StartNumberTB" style="color: black">Numero</label>
                        <asp:TextBox runat="server" ID="StartNumberTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="StartNumberRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="StartNumberTB" runat="server"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="StartFloorTB" style="color: black">Piso</label>
                        <asp:TextBox TextMode="Number" runat="server" ID="StartFloorTB" CssClass="form-control"></asp:TextBox>
                        
                    </div>

                    <div class="form-group col-md-3">
                        <label for="StartUnitTB" style="color: black">Departamento</label>
                        <asp:TextBox runat="server" ID="StartUnitTB" CssClass="form-control"></asp:TextBox>
                        
                    </div>
                </div>
            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <asp:Label runat="server" CssClass="h4 mb-4"> Destino  <i class="fas fa-location-arrow"></i></asp:Label>
            </div>
            <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-4">
                        <label for="EndProvinceDDL" style="color: black">Provincia</label>
                        <asp:DropDownList ID="EndProvinceDDL" OnSelectedIndexChanged="EndProvinceDDL_SelectedIndexChanged" runat="server" CssClass="form-control" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-4">
                        <label for="EndLocationDDL" style="color: black">Localidad</label>
                        <asp:DropDownList ID="EndLocationDDL" runat="server" CssClass="form-control" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-4">
                        <label for="EndPostalCodeTB" style="color: black">Codigo Postal</label>
                        <asp:TextBox runat="server" ID="EndPostalCodeTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="EndPostalCodeRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="EndPostalCodeTB" runat="server"></asp:RequiredFieldValidator>
                    </div>
                </div>


                <div class="form-row">
                    <div class="form-group col-md-3">
                        <label for="EndStreetTB" style="color: black">Calle</label>
                        <asp:TextBox runat="server" ID="EndStreetTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="EndStreetRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="EndStreetTB" runat="server"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="EndNumberTB" style="color: black">Numero</label>
                        <asp:TextBox runat="server" ID="EndNumberTB" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="EndNumberRFV" ValidationGroup="LoadVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="EndNumberTB" runat="server"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group col-md-3">
                        <label for="EndFloorTB" style="color: black">Piso</label>
                        <asp:TextBox TextMode="Number" runat="server" ID="EndFloorTB" CssClass="form-control"></asp:TextBox>
                        
                    </div>

                    <div class="form-group col-md-3">
                        <label for="EndUnitTB" style="color: black">Departamento</label>
                        <asp:TextBox runat="server" ID="EndUnitTB" CssClass="form-control"></asp:TextBox>
                        
                    </div>
                </div>
            </div>
        </div>

        <div class="form-row float-right">
            <div class="form-group">
                <asp:LinkButton ID="SaveDown" OnClick="SaveDown_Click" runat="server" ValidationGroup="LoadVG" ToolTip="Guardar Carga" CssClass="btn btn-secondary">Guardar</asp:LinkButton>
                <asp:LinkButton ID="CancelDown" OnClick="CancelDown_Click" runat="server" ToolTip="Cancelar Carga" CssClass="btn btn-danger">Cancelar</asp:LinkButton>
            </div>
        </div>

    </div>
</asp:Content>


