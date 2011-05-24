<%@ Control Language="C#" %>

<table border="0">
   <caption>Url Generation Tests</caption>
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
      <td>/Account</td>
      <td><%: Url.Action("", "Account") %></td>
      <td>Sibling (or self) reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "..Admin")</code>
      </td>
      <td>/Admin</td>
      <td><%: Url.Action("", "..Admin") %></td>
      <td>Parent reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "+User")</code>
      </td>
      <td>/Admin/User</td>
      <td><%: Url.Action("", "+User") %></td>
      <td>Child reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "+Tasks.Maintenance")</code>
      </td>
      <td>/Admin/Tasks/Maintenance</td>
      <td><%: Url.Action("", "+Tasks.Maintenance")%></td>
      <td>Grand-child reference.</td>
   </tr>
   <tr>
      <td>
         <code>Url.Action("", "~Admin")</code>
      </td>
      <td>/Admin</td>
      <td><%: Url.Action("", "~Admin")%></td>
      <td>Absolute reference.</td>
   </tr>
</table>
<p>The actual value depends on the current route, except for absolute references.</p>
