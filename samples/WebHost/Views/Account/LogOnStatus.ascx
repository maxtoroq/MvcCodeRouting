<%@ Control Language="C#" %>

<% if (Page.User.Identity.IsAuthenticated) { %>
<div style="float: right;">
   <strong>
      <%: Page.User.Identity.Name %>
   </strong> - 
   <a href="<%: Url.Action("LogOff", "~Account") %>">Log Off</a>
</div>
<% } %>