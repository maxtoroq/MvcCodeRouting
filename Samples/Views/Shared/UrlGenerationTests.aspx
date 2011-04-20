<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
   
   <h1>Url Generation Tests</h1>

   <table border="1" cellpadding="2">
      <tr>
         <th>Expected Value</th>
         <th>Actual Value</th>
         <th>Code</th>
         <th>Comments</th>
      </tr>
      <tr>
         <td>/Account</td>
         <td><%: Url.Action("", "Account") %></td>
         <td>
            <code>Url.Action("", "Account")</code>
         </td>
         <td>Relative sibling (or self) reference.</td>
      </tr>
      <tr>
         <td>/Admin</td>
         <td><%: Url.Action("", "..Admin") %></td>
         <td>
            <code>Url.Action("", "..Admin")</code>
         </td>
         <td>Relative parent reference.</td>
      </tr>
      <tr>
         <td>/Admin/User</td>
         <td><%: Url.Action("", ".User") %></td>
         <td>
            <code>Url.Action("", ".User")</code>
         </td>
         <td>Relative child reference.</td>
      </tr>
      <tr>
         <td>/Admin</td>
         <td><%: Url.Action("", "~Admin")%></td>
         <td>
            <code>Url.Action("", "~Admin")</code>
         </td>
         <td>Absolute reference.</td>
      </tr>
   </table>

</asp:Content>
