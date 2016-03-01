<%@ Control Language="C#" %>

<table cellpadding="4" cellspacing="0" style="width: 99%">
<tr><td style="width: 60px">订单日期</td><td>
	<input type="text" class="myTextbox" readonly="readonly" id="txtOrderTime" style="width: 200px" />
	</td></tr>
<tr><td>客户</td><td>
	<input type="text" class="myTextbox" readonly="readonly" id="txtCustomerName" style="width: 400px" />
	</td></tr>
<tr><td class="vertical">订单明细</td><td>
	<table id="tblOrderDetail"></table>
</td></tr>

<tr><td class="vertical">备注</td><td>
	<textarea class="myTextbox" style="width: 660px; height: 70px" readonly="readonly" id="txtOrderRemark" ></textarea>
	</td></tr>
<tr><td></td><td>
	<label><input type="checkbox" id="chkOrderFinished" />订单已处理</label>	&nbsp;&nbsp;&nbsp;
		<a id="btnSetOrderStatus" href="javascript:;;" class="easyui-linkbutton" iconCls="icon-ok">修改订单状态</a>		
	</td></tr>
</table>
