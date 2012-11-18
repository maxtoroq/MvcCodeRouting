<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Samples.Models.User>" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1><%: Model.Name %></h1>

   <a href="<%: Url.Action("") %>">« Back to list</a>

</asp:Content>
