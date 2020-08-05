<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="UsuarioView.aspx.cs" Inherits="TransporteFlexible.Views.Seguridad.Usuarios.UsuarioView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">

        <!-- Breadcrumbs-->
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                <a href="../../Shared/Welcome.aspx">Bienvenida</a>
            </li>
            <li class="breadcrumb-item">
                <a>Usuarios</a>
            </li>
        </ol>

        <div class="text-center border border-light p-5 w-100" id="formRegistro">

            <p class="h4 mb-4">Busqueda <span class="fa fa-search"></span></p>

            <div class="form-row">

                <div class="form-group col-md-4">
                    <label for="_txtNombreUsuario">Usuario</label>
                    <asp:TextBox ID="_tbNombreUsuario" class="form-control mb-4" placeHolder="Nombre de Usuario" runat="server" MaxLength="20"></asp:TextBox>
                </div>

                <div class="form-group col-md-4">
                    <label for="_tbDni">DNI</label>
                    <asp:TextBox ID="_tbDni" ValidationGroup="RegistroFormValidator" class="form-control mb-4" placeHolder="DNI" runat="server" MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="_tbDniREV" ValidationGroup="RegistroFormValidator" ForeColor="Red" runat="server" ErrorMessage="Solo números" ControlToValidate="_tbDni" ValidationExpression="^[0-9]*"></asp:RegularExpressionValidator>
                </div>

                <div class="form-group col-md-4">
                    <label for="BajaCBX" class="position-static">Usuarios dados de Baja</label>
                    <asp:CheckBox ID="BajaCBX" class="form-control mb-4" runat="server" />
                </div>

            </div>
            <div class="form-group w-100">
                <asp:Button ID="SearchUserBTN" class="btn btn-info btn-block my-4" runat="server" Text="Filtrar Usuario" OnClick="SearchUserBTN_Click" />
            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-table"></i>
                Usuarios
                <div class="float-right">
                    <asp:LinkButton ID="ExportXMLButton" runat="server" ToolTip="Exportar XML" CssClass="btn btn-primary" OnClick="ExportXMLButton_Click"><i class="fas fa-file-export"></i></asp:LinkButton>
                    <asp:LinkButton ID="NewUserButton" runat="server" OnClick="NewUserButton_Click" ToolTip="Nuevo Usuario" CssClass="btn btn-success"><i class="fas fa-plus"></i></asp:LinkButton>
                </div>
                
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="UserGV" AutoGenerateColumns="false" class="table table-bordered" runat="server" OnRowCommand="UserGV_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                            <asp:BoundField DataField="NombreUsuario" HeaderText="Nombre Usuario" />

                            <asp:TemplateField HeaderText="Roles">
                                <ItemTemplate>
                                    <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# Eval("Roles") %>'>
                                        <ItemTemplate>
                                            <%# (Container.ItemIndex+1)+"."+ Container.DataItem  %><br />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Habilitado">
                                <ItemTemplate>
                                    <span class="<%# ((bool)Eval("Habilitado")) ? "fas fa-check" : "fas fa-times"%>"></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Activo">
                                <ItemTemplate>
                                    <span class="<%# ((bool)Eval("Activo")) ? "fas fa-check" : "fas fa-times"%>"></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Baja">
                                <ItemTemplate>
                                    <span class="<%# ((bool)Eval("Baja")) ? "fas fa-check" : "fas fa-times"%>"></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Intentos" HeaderText="Intentos" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ToolTip="Permisos" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Permits" CssClass="btn btn-secondary"> <i class='fas fa-key'></i></asp:LinkButton>
                                    <asp:LinkButton runat="server" ToolTip="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Modify" CssClass="btn btn-secondary"><i class="fas fa-user-edit"></i> </asp:LinkButton>
                                    <asp:LinkButton runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="DeActive" CssClass="btn btn-secondary">
                                    <div title="<%# (bool)Eval("Activo") ? "Desactivar" : "Activar" %>">
                                        <i class="<%# (bool)Eval("Activo") ? "fas fa-user-slash " : "fas fa-user-check" %>"></i>
                                    </div>
                                    </asp:LinkButton>

                                    <asp:LinkButton runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="DownOrUp" CssClass="btn btn-danger">
                                    <div  title="<%# (bool)Eval("Baja") ? "Alta" : "Baja" %>">
                                    <i class="<%#(bool)Eval("Baja") ? "fas fa-arrow-up" : "far fa-trash-alt"%>"></i> 
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
            <asp:Label ID="_lblFechaActualizacion" runat="server" TextMode="DateTime"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
