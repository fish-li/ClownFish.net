<%@ Control Language="C#" ClassName="TagLinks" Inherits="MyUserControlView<List<DEMO.Model.BigPipe.BlogLink>>" %>

<p><b>推荐排行榜</b></p>
<ul>
<%  foreach( var link in Model ) {%>
	<li><a href="<%= link.Href %>" target="_blank"><%= link.Text %></a></li>
<% } %>
</ul>
