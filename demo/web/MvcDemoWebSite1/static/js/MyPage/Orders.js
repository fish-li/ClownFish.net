
$(function(){
	$("#btnQuery").click( btnQuery_Click );
	$("fieldset").MakeFieldsetCollapseEnabled();
});

function btnQuery_Click(){
	var dateRange = GetDateRange("txtStartDate", "txtEndDate");
	if( dateRange == null ) return false;
	var url = '/ajax/ns/Order/Search.aspx?' + $.param({StartDate: dateRange.StartDate, EndDate: dateRange.EndDate});
	ShowPickerPage(url, 'divResultList_inner', ShowResultSuccess);
    return false;
}

function ShowResultSuccess(divId){
	$('#' + divId + ' a.easyui-linkbutton')
	.filter("[Customer=0]").remove().end()
	.linkbutton()
	.filter("[OrderNo]").click(ShowOrderDialog).end()
	.filter("[Customer]").click(ShowCustomerDialog);
	
	$('#' + divId + ' th').addClass("nowrap");
	$('#' + divId + ' td').addClass("nowrap");
}


function ShowCustomerDialog(){
	var dom = this;
	var customerId = $(this).attr("Customer");
	var url = "/ajax/ns/Customer/Show.aspx?" + $.param({id: customerId});
	ShowViewerDialog("divCustomerInfo", url, function(){
		$("#divCustomerInfo").show();
	}, 570, 330);
	return false;
}


function ShowOrderDialog(){
	var dom = this;
	var orderId = $(this).attr("OrderNo");
	var url = "/ajax/ns/Order/Show.aspx?id=" + orderId;
	ShowViewerDialog("divOrderInfo", url, function(){
		$("#tblOrderDetail").SetGridStyle().find("a.easyui-linkbutton").linkbutton().click(ShowProductDialog);
		$("#btnSetOrderStatus").linkbutton().click(function(){	SubmitSetOrderStatus( orderId, dom ); return false; });
	}, 800, 530);
	return false;
}

function SubmitSetOrderStatus(orderId, dom){
	// 提交用户修改过的订单状态
	var finished = $("#divOrderInfo :checkbox[id$=chkFinished]").is(":checked");
	var j_waitDialog = ShowWaitMessageDialog();
	$.ajax({
		url: "/ajax/ns/Order/SetOrderStatus.aspx",  type: "GET", dataType: "text",
		data: {id: orderId, finished: finished },
		complete: function() { HideWaitMessageDialog(j_waitDialog); },
		success: function(responseText) { 
				$.messager.alert(g_MsgBoxTitle, "操作成功", "info", function(){ 
					$("#divOrderInfo").dialog('close'); 
					// 修改页面上的控件显示
					if( finished )
						$(dom).closest("tr").find(":checkbox[name$=chk_Finished]").attr("checked", "checked");
					else
						$(dom).closest("tr").find(":checkbox[name$=chk_Finished]").removeAttr("checked");
				});
		}
	});
}


function ShowProductDialog(){
	var dom = this;
	var productId = $(this).attr("ProductId");
	var url = "/ajax/ns/Product/Show.aspx?" + $.param({id: productId});
	ShowViewerDialog("divProductInfo", url, function(){
		$("#divProductInfo").show();
	}, 700, 450);
	return false;
}


