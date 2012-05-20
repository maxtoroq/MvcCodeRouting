<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Users</h1>

   <ul>
   <% foreach (var u in new[] { "Mark", "Don", "Mel", "Craig" }) {%>
      <li>
         <%: Html.ActionLink(u, "Profile", new { username = u }) %>
      </li>      
   <% } %>
   </ul>

</asp:Content>
