<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Account</h1>

   <ul>
      <li>
         <%: Html.ActionLink("Log Off", "LogOff") %>
      </li>
   </ul>

</asp:Content>
