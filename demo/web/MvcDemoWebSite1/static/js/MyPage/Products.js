
$(function(){
	$("select").SetComboBox();
	$("#txtQuantity").removeClass("myTextbox").numberspinner({min: 1, width: 80});
	SetQuantityTextboxEvent();
	
	$('#btnCreateItem').click(function(){
		$("#divProductInfo").ResetControlValues();
		ShowEditItemDialog('', 'divProductInfo', 620, 450, InsertProduct);
		return false;
	});
	
	$('table.GridView a.easyui-linkbutton[rowId]').click( ShowAjaxEditProductDialog );
});

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


function ShowAjaxEditProductDialog(){
	var dom = this;
	var productId = $(this).attr("rowId");
	// 当前显示的分类范围
	var currentCategoryId = $("#hfCurrentCategoryId").val();
	
	// 首先获取指定的商品资料
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
					// 以AJAX方式再次提交修改请求
					$("#formCreateProduct").ajaxSubmit({
						url: "/ajax/ns/Product/Update.aspx",  type: "POST", dataType: "text",
						data: {ProductId: productId},
						complete: function() { HideWaitMessageDialog(j_waitDialog); },
						success: function(responseText) {
								$.messager.alert(g_MsgBoxTitle, "操作成功。", "info", function(){ 
									var categoryId = $("#ddlCategoryID").combobox("getValue");
									if( categoryId == currentCategoryId ) {
										// 直接修改页面中的文字
										var j_tr = $(dom).closest("tr");
										j_tr.find("a[rowId]").linkbutton({text: $("#txtProductName").val()});
										j_tr.find("span[name=Unit]").text( $("#ddlUnit").combobox("getValue") );
										j_tr.find("span[name=UnitPrice]").text( $("#txtUnitPrice").val() );
										j_tr.find(":text.quantityTextbox").val( $("#txtQuantity").val() );
									}
									else	// 已经修改了商品的分类，必须要刷新页面了。
										window.location.href = window.location.href;
										
									j_dialog.hide().dialog('close');
								});
						}
					});
				}
			});
		}
	});	
	
	return false;
}


function InsertProduct(j_dialog){
	if( ValidateForm() == false ) return;
	
	var currentCategoryId = $("#hfCurrentCategoryId").val();
	var j_waitDialog = ShowWaitMessageDialog();
	$("#formCreateProduct").ajaxSubmit({
		url: "/ajax/ns/Product/Insert.aspx",
		complete: function() { HideWaitMessageDialog(j_waitDialog); },
		success: function(responseText) {
				$.messager.alert(g_MsgBoxTitle, "操作成功。", "info", function(){ 
					j_dialog.hide().dialog('close');
					
					var categoryId = $("#ddlCategoryID").combobox("getValue");
					if( categoryId == currentCategoryId ) {
						window.location.href = window.location.href;
					}			
				});
		}
	});
}

function ValidateForm(){
	if( ValidateControl("#txtProductName", "商品名称 不能为空。") == false ) return false;
	if( ValidateControl(":hidden[name=Unit]", "单位 不能为空。") == false ) return false;
	return true;
}


