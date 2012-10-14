<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Samples.Models.User>>"%>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Users</h1>

   <ul>
   <% foreach (var user in Model) {%>
      <li>
         <%: Html.ActionLink(user.Name, "Profile", new { username = user.Name }) %>
      </li>      
   <% } %>
   </ul>

</asp:Content>
