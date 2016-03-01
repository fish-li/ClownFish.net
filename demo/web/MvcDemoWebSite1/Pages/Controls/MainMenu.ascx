<%@ Control Language="C#"  %>

<div id='mainMenuBar'>
	<a href='javascript:;' id='mb1' class='easyui-menubutton' menu='#mm1' iconCls='icon-edit'>基础数据</a>
	<a href='javascript:;' id='mb2' class='easyui-menubutton' menu='#mm2' iconCls='icon-add'>订单操作</a>
	<a href='javascript:;' id='mb3' class='easyui-menubutton' menu='#mm3' iconCls='icon-help'>其它操作</a>
</div>
<div id='mm1' style='width:180px;'>
	<div><a href='/Pages/Categories.aspx'>商品分类管理</a></div>
	<div class='menu-sep'></div>
	<div><a href='/Pages/Customers.aspx'>客户管理</a></div>
	<div class='menu-sep'></div>
	<div><a href='/Pages/Products.aspx'>商品管理</a></div>
</div>
<div id='mm2' style='width:180px;'>
	<div><a href='/Pages/AddOrder.aspx'>新增订单</a></div>
	<div class='menu-sep'></div>
	<div><a href='/Pages/Orders.aspx'>订单管理</a></div>
</div>

<div id='mm3' style='width:180px;'>
	<div><a href='/Pages/CodeExplorer.aspx'>查看源代码</a></div>
	<div><a href="/Pages/Demo/">杂项演示</a></div>
    <div><a href="/AjaxPK/Default.htm" target="_blank">AJAX PK</a></div>
    <div><a href="/BigPipe/Default.aspx" target="_blank">BigPipe</a></div>
</div>
