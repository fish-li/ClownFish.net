

function ShowEditItemDialog(itemId, divId, width, height, okFunc, shownFunc){
	if( typeof(width) != "number") width = 600;
	if( typeof(height) != "number") height = 430;
	
	var isEdit = ( itemId.length > 0 );	
	var j_dialog = $("#" + divId);
	
	if( j_dialog.attr("srcTitle") == undefined )
		j_dialog.attr("srcTitle", j_dialog.attr("title"));	// title属性会在创建对话框后被清除！
	
	var dlgTitle = (isEdit ? "编辑" : "添加" ) + j_dialog.attr("srcTitle");
	
	j_dialog.show().dialog({
        width: width, height: height, modal: true, resizable: true , title: dlgTitle , closable: true,
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


function ShowPickerDialog(divId, url, okButtonHanlder, getSuccessHandler, width, height) {
	if( typeof(width) != "number") width = 880;
	if( typeof(height) != "number") height = 580;
	
    $("#" + divId).show().dialog({
        height: height, width: width, modal: true, resizable: true, 
        buttons: [
            { text: '确定', iconCls: 'icon-ok', plain: true,
                handler: function() {
                    if (okButtonHanlder ) {
						if( okButtonHanlder(divId) )
							$("#" + divId).dialog('close');
					}
					else
						$("#" + divId).dialog('close');
                }
            }, 
            { text: '取消', iconCls: 'icon-cancel',  plain: true,
                handler: function() { 
				    $("#" + divId).dialog('close');
			    }
            }],
		onOpen: function() {
			ShowPickerPage(url, divId + "_inner", getSuccessHandler);
		}
    });
}

function ShowViewerDialog(divId, url, getSuccessHandler, width, height) {
	if( typeof(width) != "number") width = 850;
	if( typeof(height) != "number") height = 530;
	
    $("#" + divId).show().dialog({
        height: height, width: width, modal: true, resizable: true, 
        buttons: [
            { text: '关闭', iconCls: 'icon-cancel',  plain: true,
                handler: function() { 
				    $("#" + divId).dialog('close');
			    }
            }],
		onOpen: function() {
			ShowPickerPage(url, divId + "_inner", getSuccessHandler);
		}
    });
}

function CloseDialog(divId){
	$("#" + divId).hide().dialog('close');
}

function ShowNormalDialog(divId, okButtonHanlder, width, height) {
	if( typeof(width) != "number") width = 850;
	if( typeof(height) != "number") height = 530;
	
    $("#" + divId).show().dialog({
        height: height, width: width, modal: true, resizable: true, 
        buttons: [
            { text: '确定', iconCls: 'icon-ok', plain: true,
                handler: function() {
                    if (okButtonHanlder ) {
						okButtonHanlder(divId);
					}
					else
						$("#" + divId).dialog('close');
                }
            }, 
            { text: '取消', iconCls: 'icon-cancel',  plain: true,
                handler: function() { 
				    $("#" + divId).dialog('close');
			    }
            }]
    });
}


function ShowPickerPage(href, divId, getSuccessHandler) {
	$('#' + divId).html(__waitHTML);
    $.ajax({
        url: href, type: "GET", dataType: "html", cache: false,
        //beforeSend: function() { $('#' + divId).hide(); },
        complete: function() { $('#' + divId).fadeIn("fast"); },
        success: function(responseHtml) {
            // 设置结果，并设置表格样式
            $('#' + divId).html(responseHtml).FindAndSetGridStyle();

            // 重新绑定事件
			$('#' + divId + ' a[autoRedire=true]').click(function() { ShowPickerPage($(this).attr("href"), divId, getSuccessHandler); return false; });
            $('#' + divId + ' select[autoRedire=true]').removeAttr("onchange").change(function() { 
				if( $(this).val().length > 0 ) 	ShowPickerPage($(this).val(), divId, getSuccessHandler); 
				return false; 
			});
			$('#' + divId + ' :text[autoRedire=true]').removeAttr("onchange").unbind('change').change(function(){
				var num = parseInt($(this).val());
				if( num > 0 && num <= parseInt($(this).attr("max")) ) {
					var url = UrlCombine( $(this).attr("baseUrl") ,  $(this).attr("param") + "=" + num.toString());
					ShowPickerPage(url, divId, getSuccessHandler); 
				}
				return false;
			})
			.unbind('keypress').keypress(function(e){
				if( e.keyCode == 13 || e.keyCode == 10 ){
					$(this).change();
					return false;
				}
			});
			
			if( getSuccessHandler )
				getSuccessHandler(divId);
        }
    });
}

function ShowUserControlInDiv(href, divId) {
	$('#' + divId).html(__waitHTML);
    $.ajax({
        url: href, type: "GET", dataType: "html", cache: false,
        //beforeSend: function() { $('#' + divId).hide(); },
        complete: function() { $('#' + divId).fadeIn("fast"); },
        success: function(responseHtml) {
            // 设置结果，并设置表格样式
            $('#' + divId).html(responseHtml).FindAndSetGridStyle();

            // 重新绑定事件
			$('#' + divId + ' a[autoRedire=true]').click(function() { ShowUserControlInDiv($(this).attr("href"), divId); return false; });
            $('#' + divId + ' select[autoRedire=true]').removeAttr("onchange").change(function() { 
				if( $(this).val().length > 0 ) 	ShowUserControlInDiv($(this).val(), divId); 
				return false; 
			});
			$('#' + divId + ' :text[autoRedire=true]').removeAttr("onchange").unbind('change').change(function(){
				var num = parseInt($(this).val());
				if( num > 0 && num <= parseInt($(this).attr("max")) ) {
					var url = UrlCombine( $(this).attr("baseUrl") ,  $(this).attr("param") + "=" + num.toString());
					ShowUserControlInDiv(url, divId); 
				}
				return false;
			})
			.unbind('keypress').keypress(function(e){
				if( e.keyCode == 13 || e.keyCode == 10 ){
					$(this).change();
					return false;
				}
			});
        }
    });
}


function ShowSelectedItem(divId, txtName, hiddenId) {
    var jselected = $("#" + divId + " table.GridView :radio").filter(":checked");
    if (jselected.length != 1) return false;

    var itemId = jselected.attr("sid").substring(7);
    var name = $("#" + divId + " span[sid=itemName_" + itemId + "]").text();
    $("#" + txtName).val($.trim(name));
    $("#" + hiddenId).val(itemId);	
    return true;
}


