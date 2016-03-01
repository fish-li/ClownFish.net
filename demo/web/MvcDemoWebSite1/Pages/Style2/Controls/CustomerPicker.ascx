<%@ Control Language="C#" %>

<%= UiHelper.RefJsFileHtml("/static/js/MyPage2/CustomerPicker.js")%>

<table cellpadding="4" cellspacing="0" style="margin-bottom: 5px" >
<tr><td>按名称查找：</td>
	<td><input type="text" id="txt_CustomerPicker_NameFilter" class="myTextbox" style="width: 400px" maxlength="50" /></td>
	<td><a id="btnCustomerPickerFilterByName" href="javascript:;;">查找</a></td></tr>
</table>

<table id="grdCustomerPicker"></table>
