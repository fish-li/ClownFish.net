
var g_MsgBoxTitle = "http://www.cnblogs.com/fish-li";


$(function(){
	// 设置Ajax操作的默认设置
	$.ajaxSetup({
		cache: false, 
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			if( typeof(errorThrown) != "undefined" )
				alert(errorThrown);
			else{
				var error =  XMLHttpRequest.status + "  " + XMLHttpRequest.statusText + "\r\n";
				var start = XMLHttpRequest.responseText.indexOf("<title>");
				var end = XMLHttpRequest.responseText.indexOf("</title>");
				if( start > 0 && end > start )
					error += XMLHttpRequest.responseText.substring(start + 7, end);					
				alert(error);
			}
		}
	});
});



String.prototype.JsonDateToString = function () {
	var date = new Date(parseInt(this.substr(6)));
	//return date.toString();
	var month = (date.getMonth() + 1) + "";
	if (month.length < 2)
		month = "0" + month;
	var day = date.getDate() + "";
	if (day.length < 2)
		day = "0" + day;
	var hour = date.getHours() + "";
	if (hour.length < 2)
		hour = "0" + hour;
	var minute = date.getMinutes() + "";
	if (minute.length < 2)
		minute = "0" + minute;
	var second = date.getSeconds() + "";
	if (second.length < 2)
		second = "0" + second;

	return date.getFullYear() + '-' + month + '-' + day + ' ' + hour + ':' + minute + ':' + second;
};

