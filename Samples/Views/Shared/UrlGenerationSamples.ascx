<%@ Control Language="C#" %>

<table border="0">
   <caption>Url Generation Samples</caption>
   <tr>
      <th>Code</th>
      <th>Expected Value</th>
      <th>Actual Value</th>
      <th>Comments</th>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "Account")</code>
      </td>
      <td><%: Url.Action("", "~Account") %></td>
      <td><%: Url.Action("", "Account") %></td>
      <td>Sibling (or self) reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "Api.Role")</code>
      </td>
      <td><%: Url.Action("", "~Api.Role")%></td>
      <td><%: Url.Action("", "Api.Role")%></td>
      <td>Child or child of sibling reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "+User")</code>
      </td>
      <td><%: Url.Action("", "~Admin.User") %></td>
      <td><%: Url.Action("", "+User") %></td>
      <td>Child reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "+Tasks.Maintenance")</code>
      </td>
      <td><%: Url.Action("", "~Admin.Tasks.Maintenance")%></td>
      <td><%: Url.Action("", "+Tasks.Maintenance")%></td>
      <td>Grand-child reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "..Admin")</code>
      </td>
      <td><%: Url.Action("", "~Admin") %></td>
      <td><%: Url.Action("", "..Admin") %></td>
      <td>Parent reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "~Admin")</code>
      </td>
      <td><%: Url.Action("", "~Admin")%></td>
      <td><%: Url.Action("", "~Admin")%></td>
      <td>baseRoute-relative reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "~~Admin")</code>
      </td>
      <td><%: Url.Action("", "~~Admin")%></td>
      <td><%: Url.Action("", "~~Admin")%></td>
      <td>Application-relative reference.</td>
   </tr>
</table>
<p>The actual value depends on the current route, except for application-relative references.</p>
