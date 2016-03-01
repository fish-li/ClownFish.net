<%@ Page Title="商品管理" Language="C#" MasterPageFile="MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%= UiHelper.RefJsFileHtml("/static/js/MyPage2/Products.js")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div class="easyui-layout" fit="true">
	<div region="north" title="商品列表" split="false" style="height:65px; overflow: hidden; border: 0px;" border="false">
		<table cellpadding="4" border="0">
			<tr><td>当前分类</td><td>
				<select id="ddlCurrentCategory"  combobox="not-editable" style="width:300px;"></select>
				</td><td>
					<a id="btnCreateItem" href="#" class="easyui-linkbutton" iconCls="icon-add">添加商品</a>
				</td></tr>
		</table>
	</div>
	
	<div region="center" style="overflow:hidden;">	
		<table id="grid1"></table>	
	</div>
</div>





<div id="divProductInfo" title="商品" style="padding: 8px; display: none">
<%= UcExecutor.Render("/Pages/Style2/Controls/ProductInfo.ascx", null)%>
</div>

</asp:Content>

