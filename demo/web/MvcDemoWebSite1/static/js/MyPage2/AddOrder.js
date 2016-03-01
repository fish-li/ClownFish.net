
$(function() {
	SetSearchTextbox("txtCustomerName", "hfCustomerID", 
					function(){ ShowPickerDialog('divCustomerList', ShowSelectedCustomer, CustomerPicker_Init); } );
	
	$("#tblOrderDetail").SetGridStyle();
    $('#btnPickProduct').click( function(){ ShowPickerDialog('divProductList', AddSelectedItemToTable, ProductPicker_Init); } );
    $('#btnSubmit').click(btnSubmit_click);
});



function ShowSelectedCustomer(j_dialog) {
	var selectedResult = GetCustomerPickerResult();
	if( selectedResult.length == 0 ){
		$.messager.alert(g_MsgBoxTitle, "没有需要操作的选择项。", "warning");
		return;
	}
	
	$("#txtCustomerName").val( selectedResult[0].name.HtmlEncode() );
    $("#hfCustomerID").val( selectedResult[0].id );	
	
	j_dialog.dialog('close');
}



function AddSelectedItemToTable(j_dialog) {

	var selectedResult = GetProductPickerResult();
	if( selectedResult.length == 0 ){
		$.messager.alert(g_MsgBoxTitle, "没有需要操作的选择项。", "warning");
		return;
	}
	
	var existIdList = $("#hfProductIdList").val();
	var existArray = existIdList.split(',');
	
	var productId, found;
	$.each(selectedResult, function(i, item){
		productId = item.id.toString();
		found = false;
		
		for( var i=0; i<existArray.length; i++)
			if( existArray[i] == productId ){
				found = true;
				break;
			}
		
		if ( found ) {
			var jjjj = $('#tblOrderDetail input[name=quantity_' + productId + "]");
            jjjj.val(parseInt(jjjj.val()) + 1);
        }
        else {
            existIdList = existIdList + productId + ',';

            $('#tblOrderDetail').append(
				'<tr><td><img src="/static/Images/delete.gif" alt="delete" onclick="DeleteDetailRow(this, ' + productId + ');return false;" style="cursor: pointer" /></td>' +
				'<td>' + item.name + '</td>' +
				'<td><input type="text" name="quantity_' + productId + '" class="easyui-numberspinner" style="width: 80px" min="1" value="1" /></td></tr>'
			);
            $('#tblOrderDetail input[name=quantity_' + productId + "]").numberspinner({ min: 1 });
        }
	});
	

	$('#tblOrderDetail').SetGridStyle();
	$("#hfProductIdList").val(existIdList);
	j_dialog.dialog('close');
}


function DeleteDetailRow(linkObject, productId) {
    var rowObject = linkObject.parentNode.parentNode;
    if (confirm("确定要删除指定的商品记录吗？ \n\n名称：" + rowObject.childNodes[1].childNodes[0].nodeValue)) {
        rowObject.parentNode.removeChild(rowObject);

        productId = productId.toString();
        document.getElementById("hfProductIdList").value =
			$.grep(document.getElementById("hfProductIdList").value
			.split(","), function(item, index) { return item != productId }).join(",");

        $('#tblOrderDetail').SetGridStyle();
    }
}



function btnSubmit_click() {
	if( $("#hfProductIdList").val().length == 0 ){
		$.messager.alert(g_MsgBoxTitle, "没有明细项目不能保存。", "warning"); return false;
	}
	
	// 获取订单明细项目，组合成一个字符串，格式：id=quantity;
	var detail = '';
	$("#tblOrderDetail input[name^=quantity_]").each(function(){
		detail += $(this).attr("name").substring(9) + "=" + $(this).val() + ";";
	});
	$("#hfOrderDetail").val(detail);
	 
	// 向服务器提交表单。注意URL参数。
	var j_dialog = ShowWaitMessageDialog();
    $("form").ajaxSubmit({
		url: "/ajax/ns/Order/AddOrder.aspx",
        complete: function() { HideWaitMessageDialog(j_dialog); },
        success: function(responseText, statusText) {
			$.messager.alert(g_MsgBoxTitle, "操作成功", "info", function(){ window.location = window.location; });
        }
    });
    return false;
}





