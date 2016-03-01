
var g_MsgBoxTitle = "http://www.cnblogs.com/fish-li";
var __waitHTML = '<div style="padding: 20px;"><img src="/static/Images/progress_loading.gif" /><span style="font-weight: bold;padding-left: 10px; color: #FF66CC;">请稍后......</span></div>';
var g_deleteButtonFilter = "a[title='删除']";

$(function(){
	// 设置Ajax操作的默认设置
	$.ajaxSetup({
		cache: false, 
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			if( typeof(errorThrown) != "undefined" )
				$.messager.alert(g_MsgBoxTitle, "调用服务器失败。<br />" + errorThrown ,'error');
			else{
				var error = "<b style='color: #f00'>" + XMLHttpRequest.status + "  " + XMLHttpRequest.statusText + "</b>";
				var start = XMLHttpRequest.responseText.indexOf("<title>");
				var end = XMLHttpRequest.responseText.indexOf("</title>");
				if( start > 0 && end > start )
					error += "<br /><br />" + XMLHttpRequest.responseText.substring(start + 7, end);
					
				$.messager.alert(g_MsgBoxTitle, "调用服务器失败。<br />" + error ,'error');
			}
		}
	});
	
	// 让EasyUI的菜单看起来更舒服。
	$("#mainMenuBar span.m-btn-downarrow").remove();
	$("div.menu-item").css("height", "25px")
		.find("div.menu-text").css({"display": "block", "width": "100%"})
		.find("a").css({"display": "block", "padding-top": "4px", "color": "#444"});
});



// 显示Ajax操作时的提示窗口
function ShowWaitMessageDialog(dlgTitle) {
	if( typeof(dlgTitle ) != "string" ) 
		dlgTitle = "请求处理中";
		
	var j_dialog = $(__waitHTML);
	j_dialog.appendTo('body').show().dialog({
        height: 120, width: 350, modal: true, resizable: false, closable: false, title: dlgTitle
    });
	return j_dialog;
}

// 关闭Ajax操作时的提示窗口
function HideWaitMessageDialog(j_dialog) {
	if( j_dialog == null )
		return;	
	j_dialog.dialog('close');
	j_dialog.remove();
	j_dialog = null;
}

// 设置数据网格样式
jQuery.fn.SetGridStyle = function(){
	return this.each(function(){ 
		$(this).find('>tbody>tr')
			//.filter(':not(.GridView_SelectedRowStyle)')
			.removeClass()
			.filter(':first').addClass("GridView_HeaderStyle").end()	// 如果使用thead，则可以不用这种处理
			.filter(':gt(0)')
			.filter(':odd').addClass("GridView_AlternatingRowStyle").end()
			.filter(':even').addClass("GridView_RowStyle");		//.end()
			//.filter(':last').has("div.pagination").removeClass().addClass("GridView_FooterStyle");	//如果使用tfoot，则可以不用这种处理 
	});
}

function UrlCombine(str1, str2){
	var flag = (str1.indexOf('?') >= 0 ? '&' : '?');
	return (str1 + flag + str2);
}


// 检查文本框是否已有输入内容
function CheckTextboxIsInputed(textbox, errorMessage){
	if( $.trim($("#" + textbox).val()).length == 0 ) {
		$.messager.alert(g_MsgBoxTitle, errorMessage,'warning', function(){$("#" + textbox).focus();});
		return false;
	}
	else
		return true;
}

// 将文本框“改造”成“有搜索对话框功能”的控件
function SetSearchTextbox(textboxId, hiddenId, pickButtonClick) {
    var j_text = $('#' + textboxId);
	if( j_text.attr("readonly") == "readonly" || j_text.attr("disabled") == "disabled" ) 
		return false;
	
	var width = j_text.width();
	var height = j_text.height() - 2;
	
    var j_div = $("<div></div>").insertBefore(j_text).addClass(j_text.attr("class")).css("width", width).css("padding", "1px");
    
    j_text.removeClass("myTextbox").addClass("myTextboxReadonly").css("width", (width-42)).css("float", "left").css("border", "0px").css("height", height).attr("readonly", "readonly");
    j_div.append(j_text);
    
    $("<a href='javascript:;;'></a>").attr("title", "选择").addClass("floatButton").addClass("searchButton").appendTo(j_div).click(pickButtonClick);
    $("<a href='javascript:;;'></a>").attr("title", "清除").addClass("floatButton").addClass("clearButton").appendTo(j_div).click(function(){
		j_text.val("").change();
		$("#" + hiddenId).val("");
	});
}




// 解析一个字符串中的日期
function parseDate(str){
  if(typeof(str) == 'string'){
    var results = str.match(/^\s*0*(\d{4})-0?(\d{1,2})-0?(\d{1,2})\s*$/);
    if(results && results.length >3)
      return new Date(parseInt(results[1]), parseInt(results[2]) -1, parseInt(results[3]));
  }
  return null;
}

// 根据二个字符串，返回一个日期范围。
function GetDateRange2(txtStart, txtEnd){
	var _date1 = $("#" + txtStart).datebox("getValue");
	var _date2 = $("#" + txtEnd).datebox("getValue");
	
	var _d1 = parseDate(_date1);
	var _d2 = parseDate(_date2);
	if( _date1.length > 0 && _d1 == null ){
		alert("日期格式输入无效。"); $("#" + txtStart).focus(); return null;
	}
	if( _date2.length > 0 && _d2 == null ){	
		alert("日期格式输入无效。"); $("#" + txtEnd).focus(); return null;
	}
	if( _date1.length > 0 && _date2.length > 0 && _d1 > _d2 ){
		alert("日期范围输入无效。"); $("#" + txtEnd).focus(); return null;
	}
	var obj = {StartDate: _date1, EndDate: _date2};
	return obj;
}


function ValidateControl(expression, message){
	if( $.trim($(expression).val()).length == 0 ){
		$.messager.alert(g_MsgBoxTitle, message, 'warning');
		return false;
	}
	return true;
}

function CommonDeleteRecord(){
	if( confirm('确定要删除此记录吗？？') ) {
		$.ajax({
			url: $(this).attr("href"),
			success: function(responseText){
				$('#grid1').datagrid('reload');
			}
		});
	}
	// 无论如何，都返回false
	return false;
}

String.prototype.HtmlEncode = function(){
    var div = document.createElement("div");
    div.appendChild(document.createTextNode(this));
    return div.innerHTML;
};

String.prototype.JsonDateToString = function(){
	var date = new Date(parseInt(this.substr(6)));
	//return date.toString();
	var month = (date.getMonth()+1) + "";
	if( month.length < 2 )
		month = "0" + month;
	var day = date.getDate() + "";
	if( day.length < 2 )
		day = "0" + day;
	var hour = date.getHours() + "";
	if( hour.length < 2 )
		hour = "0" + hour;
	var minute = date.getMinutes() + "";
	if( minute.length < 2 )
		minute = "0" + minute;
	var second = date.getSeconds() + "";
	if( second.length < 2 )
		second = "0" + second;
	
	return date.getFullYear() + '-' + month + '-' + day + ' ' + hour + ':' + minute + ':' + second;
};

Number.prototype.MoneyToString = function(){
	if( this == 0 ) return "";
    //return this.toString();
	var str = Math.round(this * 100).toString();
	return str.substr(0, str.length -2) + "." + str.substr(str.length -2);
};

