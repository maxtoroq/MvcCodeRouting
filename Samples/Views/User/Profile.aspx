<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1><%: ViewBag.Username %></h1>

   <%: Html.ActionLink("« Back to list", "")%>

</asp:Content>
