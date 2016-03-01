
$(function(){
	$('#grid1').datagrid({
		fit: true, nowrap: false, striped: true, collapsible:false,
		idField:'ProductID', rownumbers:false, singleSelect: true, border: false, pagination:true,
		//url:'/ajax/ns/Product/List.aspx',	
		columns:[[
			{title:'', field:'xx', align:'center', width: 40,
				formatter:function(value,row){ return '<a href="/ajax/ns/Product/Delete.aspx?id=' + row.ProductID + '" title="删除" class="easyui-linkbutton" plain="true"><img src="/static/Images/delete.gif" alt="删除" /></a>'; }
			},
			{title:'商品名称',field:'CustomerName',width:460,
				formatter:function(value,row){ return '<a href="javascript:;;" class="easyui-linkbutton" rowId="' + row.ProductID + '" plain="true">' + row.ProductName.HtmlEncode() + '</a>'; }
			},
			{title:"单位", field:"Unit", width:50, align: 'center'},
			{title:"单价", field:"UnitPrice", width:90, align: 'right',
				formatter:function(value,row){ return row.UnitPrice.MoneyToString(); }
			},
			{title:"数量", field:"Quantity", width:70, align: 'center',
				formatter:function(value,row){ return '<input type="text" pid="' + row.ProductID + '" value="' + row.Quantity + '" class="quantityTextbox" />'; }
			}
		]],	
		onLoadSuccess: function(){
			$($('#grid1').datagrid("getPanel")).find('a.easyui-linkbutton').linkbutton()
				.filter(g_deleteButtonFilter).click( CommonDeleteRecord ).end()
				.filter("a[rowId]").click( ShowEditDialog );
			SetQuantityTextboxEvent();
		}
	});
	
	// 每次当重新设置grid控件的 url 性后，下面的这个事件就要再次重新指定。这就是 EasyUI 的一个很傻的地方。
	var setPaginationEvent = function(){
		var pager = $('#grid1').datagrid("getPager");
		$(pager).pagination({
			onSelectPage:function(pageNumber, pageSize){
				$('#grid1').datagrid('reload', {PageIndex: pageNumber, PageSize: pageSize});			
			}
		});	
	};
	

	$("#ddlCurrentCategory, #ddlCategoryID").combobox({
		panelHeight: 350, editable: false, valueField:'CategoryID', textField:'CategoryName'
	});
	
	$.ajax({
		url: '/ajax/ns/Category/GetList.aspx', dataType: "json",
		success: function(data){
			if(data.length > 0){
				data[0].selected = true;
				$("#ddlCurrentCategory, #ddlCategoryID").combobox("loadData", data);
				
				$("#ddlCurrentCategory").combobox({ onSelect: function(){
					$('#grid1').datagrid({ queryParams: {PageIndex: 1}, 
							url: '/ajax/ns/Product/List.aspx?CategoryId=' + $("#ddlCurrentCategory").combobox("getValue") });
					setPaginationEvent();
				}});
				
				$('#grid1').datagrid({url: '/ajax/ns/Product/List.aspx?CategoryId=' + data[0].CategoryID });
				setPaginationEvent();
			}
			else{
				$("#btnCreateItem").linkbutton({disabled: true});
				$.messager.alert(g_MsgBoxTitle, "提交失败，返回错误消息或代码:<br />" + responseText ,'error');
			}
		}
	});
	
	$("#ddlUnit").combobox({
		valueField:'text', textField:'text', url: "/ajax/ns/Product/GetUnitList.aspx"
	});
	

	$("#txtQuantity").removeClass("myTextbox").numberspinner({min: 1});
		
	$('#btnCreateItem').click( ShowNewDialog );
});


function ShowNewDialog(){
	$("#ddlCategoryID").combobox("setValue", $("#ddlCurrentCategory").combobox("getValue") );
	$("#txtProductName").val("");
	$("#txtUnitPrice").val("");
	$("#txtQuantity").val("");
	$("#txtRemark").val("");
	
	ShowEditItemDialog('', 'divProductInfo', 620, 450, function InsertCategory(j_dialog){
		if( ValidateForm() == false ) return;
		
		var j_waitDialog = ShowWaitMessageDialog();
		$("#formCreateProduct").ajaxSubmit({
			url: "/ajax/ns/Product/Insert.aspx",
			complete: function() { HideWaitMessageDialog(j_waitDialog); },
			success: function(responseText) {
				$.messager.alert(g_MsgBoxTitle, "操作成功。", "info", function(){ 
					j_dialog.hide().dialog('close');					
					$('#grid1').datagrid('reload');
				});
			}
		});
	});
	return false;
}


function ShowEditDialog(){
	var dom = this;
	var productId = $(this).attr("rowId");
	var currentCategoryId = $("#ddlCurrentCategory").combobox("getValue");

	$.ajax({
		url: "/ajax/ns/Product/GetById.aspx?id=" + productId,  type: "POST", dataType: "json",
		success: function(json) { 
			// 将商品信息显示到对话框中
			$("#ddlCategoryID").combobox("setValue", json.CategoryID.toString());
			$("#txtProductName").val(json.ProductName);
			$("#ddlUnit").combobox("setValue", json.Unit);
			$("#txtUnitPrice").val(json.UnitPrice);
			$("#txtQuantity").val(json.Quantity);
			$("#txtRemark").val(json.Remark);
			
			// 显示编辑对话框
			ShowEditItemDialog(productId, 'divProductInfo', 620, 450, function(j_dialog){
				// 验证输入
				if (ValidateForm() ) {
					var j_waitDialog = ShowWaitMessageDialog();
					$("#formCreateProduct").ajaxSubmit({
						url: "/ajax/ns/Product/Update.aspx",  type: "POST", dataType: "text",
						data: {ProductId: productId},
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


function SetQuantityTextboxEvent(){
	$("input.quantityTextbox").change(function(){
		var j_textbox = $(this);
		var recId = j_textbox.attr("pid");
		var newValue = j_textbox.val();
		if( /^-?\d+$/.test(newValue) == false ){
			alert("请输入有效的整数数字。"); return false; 
		}

		$.ajax({
			dataType: "text", type: "POST",
			url: "/ajax/ns/Product/ChangeProductQuantity.aspx",
			data: {productId: recId, quantity: newValue  },
			success: function (responseText) {
				$.messager.show({title:'操作成功', msg:'库存数量已修改。', timeout:3000, showType:'slide' });
			}
		});
	});
}



function ValidateForm(){
	if( ValidateControl("#txtProductName", "商品名称 不能为空。") == false ) return false;
	if( ValidateControl(":hidden[name=Unit]", "单位 不能为空。") == false ) return false;
	return true;
}


