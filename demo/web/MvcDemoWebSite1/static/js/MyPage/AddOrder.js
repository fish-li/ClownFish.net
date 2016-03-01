
$(function() {
	SetSearchTextbox("txtCustomerName", "hfCustomerID", PickCustomerClick);
	$("#tblOrderDetail").SetGridStyle();
    $('#btnPickProduct').click(function() {
        ShowPickerDialog('divProductList', '/ajax/ns/Product/ShowProductPicker.aspx', AddSelectedItemToTable, ShowProductPickerSuccess); return false;
    });
    $('#btnSubmit').click(btnSubmit_click);
});

function PickCustomerClick(){
	ShowPickerDialog('divCustomerList', '/ajax/ns/Customer/ShowCustomerPicker.aspx', ShowSelectedCustomer, ShowCustomerPickerSuccess); return false;
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



function ShowProductPickerSuccess(divId) {
    $('#btnProductPickerFilterByName').linkbutton({ iconCls: "icon-search" }).unbind().click(function() {
        var j_textbox = $("#txtProductFilterName");
		var j_category = $("#ProductPicker_CurrentCategoryId");
		var paramString = $.param({searchWord: j_textbox.val(), categoryId: j_category.val() });        
        var url = '/ajax/ns/Product/ShowProductPicker.aspx?' + paramString
        ShowPickerPage(url, divId, ShowProductPickerSuccess);
        return false;
    });
}

function ShowCustomerPickerSuccess(divId) {
	$('#btnCustomerPickerFilterByName').linkbutton({iconCls: "icon-search"}).unbind().click(function() {
		var j_textbox = $("#txtCustomerFilterName");
		var paramString = $.param({ searchWord: j_textbox.val() });
		var url = "/ajax/ns/Customer/ShowCustomerPicker.aspx?" + paramString
		ShowPickerPage(url, divId, ShowCustomerPickerSuccess);
		return false;
	});
}

function ShowSelectedCustomer(divId) {
	return ShowSelectedItem(divId, "txtCustomerName", "hfCustomerID");
}



function AddSelectedItemToTable(divId) {
    var productId, found;
    var existIdList = $("#hfProductIdList").val();
	var existArray = existIdList.split(',');

    $("#" + divId + " table.GridView :checkbox").filter(":checked").each(function() {
        productId = $(this).attr("name").substring(7);
		found = false;
		
		for( var i=0; i<existArray.length; i++)
			if( existArray[i] == productId ){
				found = true;
				break;
			}
		
        if (found) {
		    var jjjj = $('#tblOrderDetail input[name=quantity_' + productId + "]");
            jjjj.val(parseInt(jjjj.val()) + 1);
        }
        else {
			var name = $("#" + divId + " span[sid=itemName_" + productId + "]").text();
            existIdList = existIdList + productId + ',';

            $('#tblOrderDetail').append(
				'<tr><td><img src="/static/Images/delete.gif" alt="delete" onclick="DeleteDetailRow(this, ' + productId + ');return false;" style="cursor: pointer" /></td>' +
				'<td>' + name + '</td>' +
				'<td><input type="text" name="quantity_' + productId + '" class="easyui-numberspinner" style="width: 80px" min="1" value="1" /></td></tr>'
			);
            $('#tblOrderDetail input[name=quantity_' + productId + "]").numberspinner({ min: 1 });
        }
    });

    if (typeof (productId) == "undefined") {
		$.messager.alert(g_MsgBoxTitle, "没有需要操作的选择项。", "warning");
        return false;
    }
    else {
        $('#tblOrderDetail').SetGridStyle();
		$("#hfProductIdList").val(existIdList);
        return true;
    }
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






