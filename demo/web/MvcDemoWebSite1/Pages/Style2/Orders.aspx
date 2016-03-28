<%@ Page Title="订单管理" Language="C#" MasterPageFile="MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%= UiHelper.RefJsFileHtml("/static/js/MyPage2/Orders.js")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">



<div class="easyui-layout" fit="true">
	<div region="north" title="订单查询" split="false" style="height:65px; overflow: hidden; border: 0px;" border="false">
		<table cellpadding="4" cellspacing="0" >
		<tr><td>订单日期 从</td>
			<td><input type="text" id="txtStartDate" name="StartDate" value='2010-11-01' class="myTextbox easyui-datebox" style="width: 120px" /></td>
			<td style="width: 35px; text-align: right">到</td>
			<td><input type="text" id="txtEndDate" name="EndDate" value='<%= DateTime.Now.ToString("yyyy-MM-dd") %>' class="myTextbox easyui-datebox" style="width: 120px" /></td>
			<td><a href="javascript:;" id="btnQuery" class="easyui-linkbutton" iconCls="icon-find">查找订单</a></td>
		</tr>
		</table>
	</div>
	
	<div region="center" style="overflow:hidden;">	
		<table id="tblQueryResult"></table>
	</div>
</div>





<div id="divCustomerInfo" title="查看客户资料" style="padding: 8px; display: none;">
<%= UcExecutor.Render("/Pages/Style2/Controls/CustomerInfo.ascx", null)%>
</div>

<div id="divOrderInfo" title="订单明细" style="padding: 8px; display: none;">
<%= UcExecutor.Render("/Pages/Style2/Controls/OrderInfo.ascx", null)%>
</div>

<div id="divProductInfo" title="查看商品资料" style="padding: 8px; display: none;">
<%= UcExecutor.Render("/Pages/Style2/Controls/ProductInfo.ascx", null)%>
</div>


</asp:Content>

