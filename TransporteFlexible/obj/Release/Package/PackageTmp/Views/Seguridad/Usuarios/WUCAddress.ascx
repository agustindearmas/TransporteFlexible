<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WUCAddress.ascx.cs" Inherits="TransporteFlexible.Views.Seguridad.Usuarios.WUCAddress" %>
<div class="card mb-3">
    <div class="card-header">
        <i class="fas fa-map-marker-alt"></i>
        Direcciones
        <asp:LinkButton ID="Add" runat="server" ToolTip="Nueva Direccion" OnClick="Add_Click" CssClass="btn btn-success float-right"><i class="fas fa-plus"></i></asp:LinkButton>
    </div>
    
    <div class="card-body">
        <div class="table-responsive">
            <asp:GridView ID="AddressGridView" AutoGenerateColumns="false" class="table table-bordered" runat="server" OnRowCommand="AddressGridView_RowCommand" ClientIDMode="Predictable">
                <Columns>
                    <asp:BoundField DataField="Id" runat="server" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />

                    <asp:TemplateField HeaderText="Provincia" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlProvinces" DataTextField="Description" DataValueField="Id" AutoPostBack="True" runat="server" Enabled="false" OnSelectedIndexChanged="ddlProvinces_SelectedIndexChanged">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Localidad" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlLocations" DataTextField="Description" DataValueField="Id" runat="server" AutoPostBack="True" Enabled="false">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Calle" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtStreet" MaxLength="100" ValidationGroup="AddressVG" Width="100%" runat="server" BorderStyle="None" Text='<%# Bind("Street") %>' Enabled="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvStreet" ForeColor="Red" ValidationGroup="AddressVG" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="txtStreet" runat="server"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Numero" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtNumber" ValidationGroup="AddressVG" Width="100%" runat="server" BorderStyle="None" Text='<%# Bind("Number") %>' Enabled="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNumber" ValidationGroup="AddressVG" ForeColor="Red" Display="Dynamic" ErrorMessage="Campo Requerido" ControlToValidate="txtNumber" runat="server"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revNumber" ValidationGroup="AddressVG" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="txtNumber" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Piso" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtFloor" MaxLength="3" ValidationGroup="AddressVG" Width="100%" runat="server" BorderStyle="None" Text='<%# Bind("Floor") %>' Enabled="false"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Departamento" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtUnit" MaxLength="5" ValidationGroup="AddressVG" Width="100%" runat="server" BorderStyle="None" Text='<%# Bind("Unit") %>' Enabled="false"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="Acciones">

                        <ItemTemplate>
                            <asp:LinkButton ID="Edit" runat="server" ToolTip="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_edit" CssClass="btn btn-secondary"><i class="fas fa-edit"></i> </asp:LinkButton>
                            <asp:LinkButton ID="Delete" runat="server" ToolTip="Eliminar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_delete" CssClass="btn btn-danger"> <i class="fas fa-trash-alt"></i></asp:LinkButton>

                            <asp:LinkButton ID="Save" ValidationGroup="AddressVG" runat="server" Visible="false" ToolTip="Actualizar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_update" CssClass="btn btn-secondary"><i class="fas fa-save"></i> </asp:LinkButton>
                            <asp:LinkButton ID="Cancel" runat="server" Visible="false" ToolTip="Cancelar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_cancel" CssClass="btn btn-danger"> <i class="fas fa-arrow-right"></i></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
