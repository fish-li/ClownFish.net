<%@ Page Title="商品管理" Language="C#" MasterPageFile="MasterPage.master" 
			Inherits="MyPageView<ProductsPageModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table cellpadding="4" border="0">
<tr><td>当前分类</td><td>
		<select id="ddlCurrentCategory" 
			onchange="<%= HtmlExtension.DropDownListAutoRedir("ddlCurrentCategory")%>"  
			combobox="not-editable" style="width:300px;">
<%= Model.Categories.ToHtmlOptions(Model.CurrentCategoryId, "categoryId", false, "searchWord", "page")%>
</select>
	</td><td>
	</td></tr>
</table>
<div style="height: 10px"></div>

<ul class="itemList">
<% foreach( Product product in Model.Products ) { %>
<li>
    <table class="GridView" cellspacing="0" cellpadding="4" border="0" style="border-collapse:collapse;">
		<tr><td style="white-space: normal;"><%= product.ProductName.HtmlEncode()%></td></tr>
        <tr><td>数量：<%= product.Quantity.ToText()%> （<%= product.Unit.HtmlEncode()%>）</td></tr>
		<tr><td>单价：<%= product.UnitPrice.ToText()%></td></tr>
    </table>
</li>
<% } %>
</ul>

<%= Model.PagingInfo.PaginationBar()%>

</asp:Content>

