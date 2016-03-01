
$(function(){
	$('#grdCustomerPicker').datagrid({
		width: 820, height: 400, nowrap: false, striped: true, collapsible:false,
		idField:'CustomerID', rownumbers:false, singleSelect: true, border: true, pagination:true,
		//url:'/ajax/ns/Customer/List.aspx',	
		columns:[[
			{field:'ck',checkbox:true},
			{title:'客户名称',field:'CustomerName',width:280},
			{title:"地址", field:"Address", width:330},
			{title:"电话", field:"Tel", width:150}
		]]
	});

});


function CustomerPicker_Init(){

	var setPaginationEvent = function(){
		var pager = $('#grdCustomerPicker').datagrid("getPager");
		$(pager).pagination({
			onSelectPage:function(pageNumber, pageSize){
				//alert('pageNumber:'+pageNumber+',pageSize:'+pageSize);
				$('#grdCustomerPicker').datagrid('reload', {PageIndex: pageNumber, PageSize: pageSize});			
			}
		});	
	};
		
	if( $("#btnCustomerPickerFilterByName").attr("class").length > 0 ){
		$('#grdCustomerPicker').datagrid("unselectAll");
		return ;
	}
	
	
	// 第一次初始化。	
	$("#btnCustomerPickerFilterByName").linkbutton().click(function(){
		var str = $('#txt_CustomerPicker_NameFilter').val();
		$('#grdCustomerPicker').datagrid({ queryParams: {PageIndex: 1, SearchWord: str} });
		setPaginationEvent();
	});
	
	
	$('#grdCustomerPicker').datagrid({url: '/ajax/ns/Customer/List.aspx' });
	setPaginationEvent();
	
}


function GetCustomerPickerResult(){
	var result = [];
	var rows = $('#grdCustomerPicker').datagrid('getSelections');
	for(var i=0; i<rows.length; i++)
		result.push({ id: rows[i].CustomerID, name: rows[i].CustomerName });
	return result;
}

