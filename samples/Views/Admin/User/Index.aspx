<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Users</h1>

   <ul>
   <% foreach (var id in Enumerable.Range(1, 3)) { %>
      <li>
         <%: Html.ActionLink(id.ToString(), "Edit", new { id = id }) %>
      </li>    
   <% } %>
   </ul>

</asp:Content>
