
$(function() {
	$('#grid1').datagrid({
		title:"商品分类列表",fit: true, nowrap: false, striped: true, collapsible:false,
		idField:'CategoryID', rownumbers:false, singleSelect: true, border: false, pagination:false,
		url:'/ajax/ns/Category/List.aspx',		
		columns:[[
			{title:'', field:'xx', align:'center', width: 40,
				formatter:function(value,row){ return '<a href="/ajax/ns/Category/Delete.aspx?id=' + row.CategoryID + '" title="删除" class="easyui-linkbutton" plain="true"><img src="/static/Images/delete.gif" alt="删除" /></a>'; }
			},
			{title:'分类名称',field:'CategoryName',width:300,
				formatter:function(value,row){ return '<a href="javascript:;;" class="easyui-linkbutton" rowId="' + row.CategoryID + '" plain="true">' + row.CategoryName.HtmlEncode() + '</a>'; }
			}
		]],	
		toolbar:[{
			id:'btnAdd',	text:'新增商品分类',	iconCls:'icon-add',
			handler: ShowNewDialog
		}],
		onLoadSuccess: function(){
			$($('#grid1').datagrid("getPanel")).find('a.easyui-linkbutton').linkbutton()
				.filter(g_deleteButtonFilter).click( CommonDeleteRecord ).end()
				.filter("a[rowId]").click( ShowEditDialog );
		}
	});

});


function ShowNewDialog(){
	$("#divAddItem :text").val("");
	ShowEditItemDialog('', 'divAddItem', 500, 200, function InsertCategory(j_dialog){
		if( ValidateForm() == false ) return;
		
		var j_waitDialog = ShowWaitMessageDialog();
		$.ajax({
			url: "/ajax/ns/Category/Insert.aspx",  type: "POST", 
			data: $("#divAddItem :text").fieldSerialize(),
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
						j_dialog.hide().dialog('close');
						$('#grid1').datagrid('reload');
					});
				}
			});
		}
	});
	
}



function ValidateForm(){
	if( ValidateControl("#txtCategoryName", "分类名称 不能为空。") == false ) return false;
	return true;
}


