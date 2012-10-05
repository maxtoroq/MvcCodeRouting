<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Samples.Models.User>" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Edit User <%: Model.Id %></h1>

   <%: Html.ActionLink("« Back to list", "")%>

</asp:Content>
