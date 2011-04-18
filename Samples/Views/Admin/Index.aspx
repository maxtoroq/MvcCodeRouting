<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Admin</h1>
   <ul>
      <li>
         <%: Html.ActionLink("Users", "", "User") %>
      </li>
   </ul>

</asp:Content>
