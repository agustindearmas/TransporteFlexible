<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="TransporteFlexible.Views.Shared.Welcome" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <span class="alert-success" style="font-size:50px">¡Hola!:
            <asp:Label ID="WelcomeUserTB" runat="server" Text="Label"></asp:Label> bienvenido a TFLEX &copy;<br />
            Hoy es <span> <%: DateTime.Now.Date.ToShortDateString() %></span>
        </span>
    </div>

</asp:Content>
