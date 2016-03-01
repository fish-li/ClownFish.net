

function ShowEditItemDialog(itemId, divId, width, heigth, okFunc, shownFunc){
	if( typeof(width) != "number") width = 600;
	if( typeof(height) != "number") height = 430;
	
	var isEdit = ( itemId.length > 0 );	
	var j_dialog = $("#" + divId);
	
	if( j_dialog.attr("srcTitle") == undefined )
		j_dialog.attr("srcTitle", j_dialog.attr("title"));	// title属性会在创建对话框后被清除！
	
	var dlgTitle = (isEdit ? "编辑" : "添加" ) + j_dialog.attr("srcTitle");
	
	j_dialog.show().dialog({
        width: width, height: heigth, modal: true, resizable: true , title: dlgTitle , closable: true,
        buttons: [
            { text: (isEdit ? "保存" : "创建"), iconCls: 'icon-ok', plain: true,
                handler: function() {
					if( typeof(okFunc) == "function")
						okFunc(j_dialog);
                }
            }, 
            { text: '取消', iconCls: 'icon-cancel',  plain: true,
                handler: function() { 
					j_dialog.dialog('close');
			    }
            }],
		onOpen: function() { 
			if( typeof(shownFunc) == "function")
				shownFunc(j_dialog);
			
			j_dialog.find(":text.myTextbox").first().focus(); 
		}
	});
}


function ShowPickerDialog(divId, okFunc, shownFunc, width, height) {
	if( typeof(width) != "number") width = 850;
	if( typeof(height) != "number") height = 530;
	
	var j_dialog = $("#" + divId);
	
    j_dialog.show().dialog({
        height: height, width: width, modal: true, resizable: true, 
        buttons: [
            { text: '确定', iconCls: 'icon-ok', plain: true,
                handler: function() {
                    if( typeof(okFunc) == "function")
						okFunc(j_dialog);
                }
            }, 
            { text: '取消', iconCls: 'icon-cancel',  plain: true,
                handler: function() { 
				    j_dialog.dialog('close');
			    }
            }],
		onOpen: function() { 
			if( typeof(shownFunc) == "function")
				shownFunc(j_dialog);
		}
    });
}


function ShowViewerDialog(divId, width, height) {
	if( typeof(width) != "number") width = 850;
	if( typeof(height) != "number") height = 530;
	
    $("#" + divId).show().dialog({
        height: height, width: width, modal: true, resizable: true, 
        buttons: [
            { text: '关闭', iconCls: 'icon-cancel',  plain: true,
                handler: function() { 
				    $("#" + divId).dialog('close');
			    }
            }]
    });
}
