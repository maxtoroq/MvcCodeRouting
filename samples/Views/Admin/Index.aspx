<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Admin</h1>
   <ul>
      <li>
         <a href="<%: Url.Action("", "+User") %>">Users</a>
      </li>
   </ul>

</asp:Content>
