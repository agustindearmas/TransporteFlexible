<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WUCEmails.ascx.cs" Inherits="TransporteFlexible.Views.Seguridad.Usuarios.WUCEmails" %>
<div class="card mb-3">
    <div class="card-header">
        <i class="fas fa-at"></i>
        Emails
        <asp:LinkButton ID="Add" runat="server" ToolTip="Nuevo Email" OnClick="Add_Click" CssClass="btn btn-success float-right"><i class="fas fa-plus"></i></asp:LinkButton>

    </div>
    <div class="card-body">
        <div class="table-responsive">
            <asp:GridView ID="_emailsGridView" AutoGenerateColumns="false" class="table table-bordered" runat="server" OnRowCommand="EmailsGridView_RowCommands" ClientIDMode="Predictable">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />


                    <asp:TemplateField HeaderText="Email">
                        <ItemTemplate>
                            <asp:TextBox ID="txtEmail" ValidationGroup="EmailVG" Width="100%" runat="server" BorderStyle="None" Text='<%# Bind("EmailAddress") %>' Enabled="false"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revEmail" ValidationGroup="EmailVG" ForeColor="Red" runat="server" ErrorMessage="xxxx@xxxx.com" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Habilitado">
                        <ItemTemplate>
                            <span class="<%# ((bool)Eval("Habilitado")) ? "fas fa-check" : "fas fa-times"%>"></span>
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="Acciones">

                        <ItemTemplate>
                            <asp:LinkButton ID="Edit" runat="server" ToolTip="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_edit" CssClass="btn btn-secondary"><i class="fas fa-edit"></i> </asp:LinkButton>
                            <asp:LinkButton ID="Delete" runat="server" ToolTip="Eliminar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_delete" CssClass="btn btn-danger"> <i class="fas fa-trash-alt"></i></asp:LinkButton>

                            <asp:LinkButton ID="Save" ValidationGroup="EmailVG" runat="server" Visible="false" ToolTip="Actualizar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_update" CssClass="btn btn-secondary"><i class="fas fa-save"></i> </asp:LinkButton>
                            <asp:LinkButton ID="Cancel" runat="server" Visible="false" ToolTip="Cancelar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="_cancel" CssClass="btn btn-danger"> <i class="fas fa-arrow-right"></i></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>


                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
