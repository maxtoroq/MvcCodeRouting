<%@ Page Language="C#" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

   <h1>Log On</h1>

   <%: Html.ValidationSummary() %>

   <form method="post">
      
      <dl>
         <dt>
            <%: Html.Label("username", "Username:") %>
         </dt>
         <dd>
            <%: Html.TextBox("username") %>
         </dd>
         <dt>
            <%: Html.Label("password", "Password:") %>
         </dt>
         <dd>
            <%: Html.Password("password") %>
         </dd>
         <dt></dt>
         <dd>
            <br />
            <input type="submit" value="Log On" />
         </dd>
      </dl>

   </form>

</asp:Content>
