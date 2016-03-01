<%@ Page Title="商品分类管理" Language="C#" MasterPageFile="MasterPage.master" 
			Inherits="MyPageView<CategoriesPageModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


<table class="GridView" cellspacing="0" cellpadding="4" border="0" style="border-collapse:collapse;">
<tr align="left">
	<th style="width:530px;">分类名称</th>
</tr>

<% foreach( Category category in Model.List ) { %>
    <tr>
        <td><%= category.CategoryName.HtmlEncode() %></td>
    </tr>
<%} %>

</table>




</asp:Content>

