<%@ Page Title="客户管理" Language="C#" MasterPageFile="MasterPage.master" 
		Inherits="MyPageView<CustomersPageModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<ul class="itemList">
<% foreach( Customer customer in Model.List ) { %>
<li>
    <table class="GridView" cellspacing="0" cellpadding="4" border="0" style="border-collapse:collapse;">
		<tr><td><%= customer.CustomerName.HtmlEncode()%></td></tr>
        <tr><td><%= customer.ContactName.HtmlEncode() %></td></tr>
        <tr><td><%= customer.Address.HtmlEncode() %></td></tr>
        <tr><td><%= customer.PostalCode.HtmlEncode() %></td></tr>
        <tr><td><%= customer.Tel.HtmlEncode() %></td></tr>
    </table>
</li>
<% } %>
</ul>

<%= Model.PagingInfo.PaginationBar()%>

</asp:Content>

