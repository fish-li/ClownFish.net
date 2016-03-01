<%@ Page Title="商品分类管理" Language="C#" MasterPageFile="MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%= UiHelper.RefJsFileHtml("/static/js/MyPage2/Categories.js")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table id="grid1"></table>





<div id="divAddItem" title="商品分类" style="padding: 8px; display: none">

<table cellpadding="4" border="0px">
<tr><td style="width: 80px">分类名称</td>
	<td><input name="CategoryName" type="text" maxlength="20" id="txtCategoryName" class="myTextbox w300" /></td></tr>
</table>

</div>





</asp:Content>

