<%@ Control Language="C#" %>

<%= UiHelper.RefJsFileHtml("/static/js/MyPage2/ProductPicker.js")%>

<table cellpadding="4" cellspacing="0" style="margin-bottom: 5px" >
<tr><td>商品类别：</td>
	<td><select id="ProductPicker_ddlCurrentCategory" combobox="not-editable" autoRedire="true" style="width:180px;"></select>
	</td>
<td>按名称查找：</td>
	<td><input type="text" id="txt_ProductPicker_NameFilter" class="myTextbox" style="width: 270px" /></td>
	<td><a id="btnProductPickerFilterByName" href="javascript:;;">查找</a></td></tr>
</table>

<table id="grdProductPicker"></table>
