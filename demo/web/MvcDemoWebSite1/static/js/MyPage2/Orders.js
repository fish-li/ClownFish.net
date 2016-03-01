
$(function(){
	$('#tblQueryResult').datagrid({
		fit: true, nowrap: false, striped: true, collapsible:false,
		idField:'OrderID', rownumbers:false, singleSelect: true, border: false, pagination:true,
		columns:[[
			{title:'订单编号',field:'OrderNo',width:150,
				formatter:function(value,row){ return '<a href="javascript:;;" OrderId="' + row.OrderID + '" class="easyui-linkbutton" plain="true" iconCls="icon-open">' + row.OrderNo + '</a>'; }
			},
			{title:'时间',field:'OrderDate',width:180,
				formatter:function(value,row){ return row.OrderDate.JsonDateToString(); }
			},
			{title:'客户名称',field:'CustomerName',width:300,
				formatter:function(value,row){ 
					if( row.ValidCustomerId > 0 )
						return '<a href="javascript:;;" CustomerId=' + row.ValidCustomerId + ' class="easyui-linkbutton" plain="true" iconCls="icon-open">' + row.CustomerName.HtmlEncode() + '</a>';
					else
						return '';
				}
			},
			{title:"订单金额", field:"SumMoney", width:120, align: 'right',
				formatter:function(value,row){ return row.SumMoney.MoneyToString();  }
			},
			{title:"已处理", field:"Finished", width:60, align: 'center',
				formatter:function(value,row){ 
					if( row.Finished )
						return '<input type="checkbox" id="chk_Finished" disabled="disabled" checked="checked" />';
					else
						return '<input type="checkbox" id="chk_Finished" disabled="disabled"  />';
				}
			}
		]],
		onLoadSuccess: function(){
			$($('#tblQueryResult').datagrid("getPanel")).find('a.easyui-linkbutton').linkbutton()
				.filter("a[OrderId]").click( ShowOrderDialog ).end()
				.filter("a[CustomerId]").click( ShowCustomerDialog );
		}
	});
	
	
	// 初始化订单详情对话框
	$('#tblOrderDetail').datagrid({
		width: 660, height: 250, nowrap: false, striped: true, collapsible:false,
		idField:'OrderID', rownumbers:false, singleSelect: true, border: true, pagination:false,
		columns:[[
			{title:'商品名称',field:'ProductName',width:410,
				formatter:function(value,row){ 
					if( row.ProductID > 0 )
						return '<a href="javascript:;;" ProductId="' + row.ProductID + '" class="easyui-linkbutton" plain="true">' + row.ProductName + '</a>'; 
					else
						return row.ProductName;
				}
			},
			{title:'单位',field:'Unit',width:40, align: 'center'},
			{title:'数量',field:'Quantity',width:40, align: 'right'},
			{title:"单价", field:"UnitPrice", width:100, align: 'right',
				formatter:function(value,row){ return row.UnitPrice.MoneyToString(); }
			}
		]],
		onLoadSuccess: function(){
			$($('#tblOrderDetail').datagrid("getPanel")).find('a.easyui-linkbutton').linkbutton()
				.filter("a[ProductId]").click( ShowProductDialog );
		}
	});
	
	$("#ddlCategoryID").combobox({
		panelHeight: 350, editable: false, valueField:'CategoryID', textField:'CategoryName'
	});
	
	$.ajax({
		url: '/ajax/ns/Category/GetList.aspx', dataType: "json",
		success: function(data){
			if(data.length > 0)
				$("#ddlCategoryID").combobox("loadData", data);
		}
	});
	
	$("#ddlUnit").combobox({
		valueField:'text', textField:'text', url: "/ajax/ns/Product/GetUnitList.aspx"
	});
	$("#txtQuantity").removeClass("myTextbox").numberspinner();
	// end 初始化订单详情对话框


	$("#btnQuery").click( btnQuery_Click );
	$("#btnSetOrderStatus").click( SubmitSetOrderStatus );
});

function btnQuery_Click(){
	var setPaginationEvent = function(){
		var pager = $('#tblQueryResult').datagrid("getPager");
		$(pager).pagination({
			onSelectPage:function(pageNumber, pageSize){
				$('#tblQueryResult').datagrid('reload', {PageIndex: pageNumber, PageSize: pageSize});			
			}
		});	
	};
	
	
	var dateRange = GetDateRange2("txtStartDate", "txtEndDate");
	if( dateRange == null ) return false;
	var url = '/ajax/ns/Order/Search2.aspx?' + $.param({StartDate: dateRange.StartDate, EndDate: dateRange.EndDate});
    
	$('#tblQueryResult').datagrid({url: url });
	setPaginationEvent();
	
    return false;
}



function ShowCustomerDialog(){
	var dom = this;
	var customerId = $(this).attr("CustomerId");
	var url = "/ajax/ns/Customer/GetById.aspx?" + $.param({id: customerId});
	$.ajax({
		dataType: "json", url: url,
		success: function(json){
			$("#divCustomerInfo #txtCustomerName").val( json.CustomerName );
			$("#divCustomerInfo #txtContactName").val( json.ContactName );
			$("#divCustomerInfo #txtAddress").val( json.Address );
			$("#divCustomerInfo #txtPostalCode").val( json.PostalCode );
			$("#divCustomerInfo #txtTel").val( json.Tel );
			
			ShowViewerDialog("divCustomerInfo", 570, 330);
		}
	});	
	
	return false;
}


function ShowOrderDialog(){
	var dom = this;
	var orderId = $(this).attr("OrderId");
	
	$.ajax({
		dataType: "json", url: "/ajax/ns/Order/GetById.aspx?id=" + orderId,
		success: function(json){
			$("#divOrderInfo #btnSetOrderStatus").attr("orderId", json.OrderID );
			$("#divOrderInfo #txtOrderTime").val( json.OrderDate.JsonDateToString() );
			$("#divOrderInfo #txtCustomerName").val( json.CustomerName );
			$("#divOrderInfo #txtOrderRemark").val( json.Comment );
			if( json.Finished )  $("#divOrderInfo #chkOrderFinished").attr("checked", "checked");
			else  				$("#divOrderInfo #chkOrderFinished").removeAttr("checked");
			
			if( json.Details.length > 0 )
				json.Details.push({OrderID: 0, ProductName:"订单总金额：<b>" + json.SumMoney.MoneyToString()  
						+ "</b>", Unit:"", Quantity: "", UnitPrice: 0.0});
			
			$('#tblOrderDetail').datagrid("loadData", json.Details);
			
			ShowViewerDialog("divOrderInfo", 800, 530);
		}
	});	
}


function ShowProductDialog(){
	var dom = this;
	var productId = $(this).attr("ProductId");
	var url = "/ajax/ns/Product/GetById.aspx?" + $.param({id: productId});
	
	$.ajax({
		dataType: "json", url: url,
		success: function(json){
			$("#ddlCategoryID").combobox("setValue", json.CategoryID.toString());
			$("#txtProductName").val(json.ProductName);
			$("#ddlUnit").combobox("setValue", json.Unit);
			$("#txtUnitPrice").val(json.UnitPrice);
			$("#txtQuantity").val(json.Quantity);
			$("#txtRemark").val(json.Remark);
			
			ShowViewerDialog("divProductInfo", 700, 450);
		}
	});	
	
	return false;
}


function SubmitSetOrderStatus(){
	// 提交用户修改过的订单状态
	var orderId = $(this).attr("orderId");
	var finished = $("#chkOrderFinished").is(":checked");
	var j_waitDialog = ShowWaitMessageDialog();
	$.ajax({
		url: "/ajax/ns/Order/SetOrderStatus.aspx",  type: "GET", dataType: "text",
		data: {id: orderId, finished: finished },
		complete: function() { HideWaitMessageDialog(j_waitDialog); },
		success: function(responseText) { 
			$.messager.alert(g_MsgBoxTitle, "操作成功", "info", function(){ 
				$("#divOrderInfo").dialog('close'); 
				$('#tblQueryResult').datagrid("load");				
			});
		}
	});
}




