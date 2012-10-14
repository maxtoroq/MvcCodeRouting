<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Samples.Models.User>>" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Users</h1>

   <ul>
   <% foreach (var user in Model) { %>
      <li>
         <%: Html.ActionLink(user.Name, "Edit", new { id = user.Id }) %>
      </li>    
   <% } %>
   </ul>

</asp:Content>
