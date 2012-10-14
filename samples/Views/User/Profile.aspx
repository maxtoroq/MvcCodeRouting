<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Samples.Models.User>" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1><%: Model.Name %></h1>

   <%: Html.ActionLink("« Back to list", "")%>

</asp:Content>
