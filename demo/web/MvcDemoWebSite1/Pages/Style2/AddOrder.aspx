<%@ Page Title="新增订单" Language="C#" MasterPageFile="MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%= UiHelper.RefJsFileHtml("/static/js/MyPage2/AddOrder.js")%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div class="easyui-panel" title="新增订单" fit="true" style="padding: 8px">

<form id="formAddOrder" action="" method="post">
<table cellpadding="4" cellspacing="0">
<tr><td style="width: 100px">订单日期</td><td>
	<input type="text" id="txtOrderDate" name="OrderDate" readonly="readonly" class="myTextboxReadonly" value='<%= DateTime.Now.ToString("yyyy-MM-dd") %>' /></td></tr>

<tr><td>客户</td><td>
	<input type="text" id="txtCustomerName" class="myTextbox" style="width: 400px" />
	<input type="hidden" id="hfCustomerID" name="CustomerID" />
	</td></tr>

<tr><td class="vertical">订单明细</td><td>
	<table cellpadding="4" cellspacing="0"  id="tblOrderDetail">
		<tr><td style="width: 25px"></td><td style="width: 500px">商品名称</td><td style="width: 100px">数量</td></tr>
	</table>
	<div style="padding-top: 5px">
		<a id="btnPickProduct" href="javascript:;;" class="easyui-linkbutton" plain="true" iconCls="icon-add">添加商品项目</a>
		<input type="hidden" id="hfProductIdList" />
		<input type="hidden" id="hfOrderDetail" name="OrderDetail" />
	</div>

</td></tr>

<tr><td class="vertical">备注</td><td>
	<textarea id="txtComment" name="Comment" class="myTextbox" style="width: 650px; height: 70px" cols="50"></textarea></td></tr>

<tr><td style="height: 35px"></td><td></td></tr>
	
<tr><td></td><td>
	<a id="btnSubmit" href="#" class="easyui-linkbutton" iconCls="icon-ok">确定保存此订单记录　并继续新增</a>
	<span style="padding-left: 25px"></span>
	<a href="javascript:window.location = window.location;" id="anchorCancel" class="easyui-linkbutton" iconCls="icon-remove">放弃当前输入内容，我要新增记录</a>
	</td></tr>
</table>

</form>
</div>




<div id="divProductList" title="（添加）商品项目" style="padding: 8px; display: none;">
<%= UcExecutor.Render("/Pages/Style2/Controls/ProductPicker.ascx", null) %>
</div>


<div id="divCustomerList" title="选择客户" style="padding: 8px; display: none;">
<%= UcExecutor.Render("/Pages/Style2/Controls/CustomerPicker.ascx", null) %>
</div>

</asp:Content>

