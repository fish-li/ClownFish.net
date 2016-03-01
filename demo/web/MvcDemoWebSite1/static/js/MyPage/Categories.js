
$(function() {
	$('#btnCreateItem').click(function(){
	    $("#divAddItem :text").val("");
		ShowEditItemDialog('', 'divAddItem', 500, 200, InsertCategory);
		return false;
	});
	
	$("table.GridView a.easyui-linkbutton[rowId]").click(ShowCategoryDialog);
});

function ShowCategoryDialog(){
	var dom = this;
	var categoryId = $(this).attr("rowId");
	$("#txtCategoryName").val($(this).text());

	ShowEditItemDialog(categoryId, 'divAddItem', 500, 200, function(j_dialog){
		// 验证输入
		if (ValidateForm() ) {
			var j_waitDialog = ShowWaitMessageDialog();
			$.ajax({
				url: "/ajax/ns/Category/Update.aspx?CategoryID=" + categoryId,  type: "POST", 
				data: $("#divAddItem :text").fieldSerialize(),
				complete: function() { HideWaitMessageDialog(j_waitDialog); },
				success: function(responseText) {
						$.messager.alert(g_MsgBoxTitle, "操作成功。", "info", function(){ 
							// 直接修改页面中的文字
							var j_tr = $(dom).closest("tr");
							j_tr.find("a[rowId]").linkbutton({text: $("#txtCategoryName").val()});
							j_dialog.hide().dialog('close');
						});
				}
			});
		}
	});
	
	return false;
}


function InsertCategory(j_dialog){
	if( ValidateForm() == false ) return;
	
	var j_waitDialog = ShowWaitMessageDialog();
	$.ajax({
		url: "/ajax/ns/Category/Insert.aspx",  type: "POST", 
		data: $("#divAddItem :text").fieldSerialize(),
		complete: function() { HideWaitMessageDialog(j_waitDialog); },
		success: function(responseText) {
				$.messager.alert(g_MsgBoxTitle, "操作成功。", "info", function(){ 
					j_dialog.hide().dialog('close');
					window.location = window.location;
				});
		}
	});
}


function ValidateForm(){
	if( ValidateControl("#txtCategoryName", "分类名称 不能为空。") == false ) return false;
	return true;
}


