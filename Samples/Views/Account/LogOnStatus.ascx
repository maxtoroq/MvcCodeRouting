<%@ Control Language="C#" %>

<% if (Page.User.Identity.IsAuthenticated) { %>
<div style="float: right;">
   <strong>
      <%: Page.User.Identity.Name %>
   </strong> - 
   <%: Html.ActionLink("Log Off", "LogOff", "~Account") %>
</div>
<% } %>