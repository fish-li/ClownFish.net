
$(function() {
	$('#grid1').datagrid({
		title:"客户列表", fit: true, nowrap: false, striped: true, collapsible:false,
		idField:'CustomerID', rownumbers:false, singleSelect: true, border: false, pagination:true,
		url:'/ajax/ns/Customer/List.aspx',	
		columns:[[
			{title:'', field:'xx', align:'center', width: 40,
				formatter:function(value,row){ return '<a href="/ajax/ns/Customer/Delete.aspx?id=' + row.CustomerID + '" title="删除" class="easyui-linkbutton" plain="true"><img src="/static/Images/delete.gif" alt="删除" /></a>'; }
			},
			{title:'客户名称',field:'CustomerName',width:260,
				formatter:function(value,row){ return '<a href="javascript:;;" class="easyui-linkbutton" rowId="' + row.CustomerID + '" plain="true">' + row.CustomerName.HtmlEncode() + '</a>'; }
			},
			{title:"联系人", field:"ContactName", width:80},
			{title:"地址", field:"Address", width:300},
			{title:"邮编", field:"PostalCode", width:80},
			{title:"电话", field:"Tel", width:150}
		]],	
		toolbar:[{
			id:'btnAdd',	text:'新增客户记录',	iconCls:'icon-add',
			handler: ShowNewDialog
		}],
		onLoadSuccess: function(){
			$($('#grid1').datagrid("getPanel")).find('a.easyui-linkbutton').linkbutton()
				.filter(g_deleteButtonFilter).click( CommonDeleteRecord ).end()
				.filter("a[rowId]").click( ShowEditDialog );
		}
	});
	
	var pager = $('#grid1').datagrid("getPager");
	$(pager).pagination({
		onSelectPage:function(pageNumber, pageSize){
			$('#grid1').datagrid('reload', {PageIndex: pageNumber, PageSize: pageSize});			
		}
	});	

});


function ShowNewDialog(){
	$("#divCustomerInfo :text").val("");
	ShowEditItemDialog('', 'divCustomerInfo', 570, 300, function InsertCategory(j_dialog){
		if( ValidateForm() == false ) return;
		
		var j_waitDialog = ShowWaitMessageDialog();
		$.ajax({
			url: "/ajax/ns/Customer/Insert.aspx",  type: "POST", 
			data: $("#divCustomerInfo :text").fieldSerialize(),
			complete: function() { HideWaitMessageDialog(j_waitDialog); },
			success: function(responseText) {
				$.messager.alert(g_MsgBoxTitle, "操作成功。", "info", function(){ 
					j_dialog.hide().dialog('close');					
					$('#grid1').datagrid('reload');
				});
			}
		});
	});

}


function ShowEditDialog(){
	var dom = this;
	var customerId = $(this).attr("rowId");

	// 首先获取指定的客户资料
	$.ajax({
		url: "/ajax/ns/Customer/GetById.aspx?id=" + customerId, dataType: "json",
		success: function(json) { 
			$("#txtCustomerName").val(json.CustomerName);
			$("#txtContactName").val(json.ContactName);
			$("#txtAddress").val(json.Address);
			$("#txtPostalCode").val(json.PostalCode);
			$("#txtTel").val(json.Tel);
			
			// 显示编辑对话框
			ShowEditItemDialog(customerId, 'divCustomerInfo', 570, 300, function(j_dialog){
				// 验证输入
				if (ValidateForm() ) {
					var j_waitDialog = ShowWaitMessageDialog();
					$.ajax({
						type: "POST", 
						url: "/ajax/ns/Customer/Update.aspx",  data: $.param({CustomerID: customerId}) + "&" + $("#divCustomerInfo :text").fieldSerialize(),
						complete: function() { HideWaitMessageDialog(j_waitDialog); },
						success: function(responseText) {
							$.messager.alert(g_MsgBoxTitle, "操作成功。", "info", function(){ 
								j_dialog.hide().dialog('close');
								$('#grid1').datagrid('reload');
							});
						}
					});
				}
			});
		}
	});	
}



function ValidateForm(){
	if( ValidateControl("#txtCustomerName", "客户名称 不能为空。") == false ) return false;
	return true;
}

