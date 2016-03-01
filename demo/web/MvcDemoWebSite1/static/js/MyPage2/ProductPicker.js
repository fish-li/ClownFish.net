
$(function(){
	$('#grdProductPicker').datagrid({
		width: 820, height: 400, nowrap: false, striped: true, collapsible:false,
		idField:'ProductID', rownumbers:false, singleSelect: false, border: true, pagination:true,
		//url:'/ajax/ns/Product/List.aspx',	
		columns:[[
			{field:'ck',checkbox:true},
			{title:'商品名称',field:'ProductName',width:530},
			{title:"单位", field:"Unit", width:50, align: 'center'},
			{title:"单价", field:"UnitPrice", width:90, align: 'right',
				formatter:function(value,row){ return row.UnitPrice.MoneyToString(); }
			},
			{title:"数量", field:"Quantity", width:70, align: 'right'}
		]]
	});

});



function ProductPicker_Init(){

	var setPaginationEvent = function(){
		var grid = $('#grdProductPicker');
		grid.datagrid("clearSelections");
		
		var pager = grid.datagrid("getPager");
		$(pager).pagination({
			onSelectPage:function(pageNumber, pageSize){
				$('#grdProductPicker').datagrid("clearSelections")
					.datagrid('reload', {PageIndex: pageNumber, PageSize: pageSize});			
			}
		});	
	};
	
	if( $("#btnProductPickerFilterByName").attr("class").length > 0 ){
		$('#grdProductPicker').datagrid("unselectAll");
		return;
	}
	
	
	// 第一次初始化。	
	$("#btnProductPickerFilterByName").linkbutton().click(function(){
		var str = $('#txt_ProductPicker_NameFilter').val();
		$('#grdProductPicker').datagrid({ queryParams: {PageIndex: 1, SearchWord: str} });
		setPaginationEvent();
	});
	
	$("#ProductPicker_ddlCurrentCategory").combobox({
		panelHeight: 350, editable: false, valueField:'CategoryID', textField:'CategoryName'
	});
	
	$.ajax({
		url: '/ajax/ns/Category/GetList.aspx', dataType: "json",
		success: function(data){
			if(data.length > 0){
				var list = [];
				list.push({CategoryID: 0, CategoryName: "全部商品", selected: true});
				list = list.concat(data);
				
				$("#ProductPicker_ddlCurrentCategory").combobox("loadData", list);
				
				$("#ProductPicker_ddlCurrentCategory").combobox({ onSelect: function(){
					$('#grdProductPicker').datagrid({ queryParams: {PageIndex: 1}, 
							url: '/ajax/ns/Product/List.aspx?CategoryId=' + $("#ProductPicker_ddlCurrentCategory").combobox("getValue") });
					setPaginationEvent();
				}});
				
				$('#grdProductPicker').datagrid({url: '/ajax/ns/Product/List.aspx' });
				setPaginationEvent();
			}
		}
	});
	
}


function GetProductPickerResult(){
	var result = [];
	var rows = $('#grdProductPicker').datagrid('getSelections');	
	for(var i=0; i<rows.length; i++)
		result.push({ id: rows[i].ProductID, name: rows[i].ProductName });
	return result;
}



