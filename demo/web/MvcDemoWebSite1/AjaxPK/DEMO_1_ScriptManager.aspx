<%@ Page Language="C#"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>第一代技术：生成客户端代理脚本调用服务端</title>
    <link type="text/css" href="/AjaxPK/css/StyleSheet.css" rel="Stylesheet" />
</head>
<body>
<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server">
		<Services>
			<asp:ServiceReference Path="/AjaxPK/WebService1.asmx" InlineScript="true" />
		</Services>
	</asp:ScriptManager>
	
	<div>
		<input type="button" onclick="Call_Add()" value="计算 1 + 2" />
	</div>
	<hr />
	
	<div>
		<p><b>Add Customer</b></p>
		<span>Name: </span><input type="text" id="txtName" value="abc" /><br />
		<span>Age: </span><input type="text" id="txtAge" value="20" /><br />
		<span>Address: </span><input type="text" id="txtAddress" value="武汉" /><br />
		<span>Tel:</span> <input type="text" id="txtTel" value="12345678" /><br />
		<span>Email: </span><input type="text" id="txtEmail" value="test@163.com" /><br />
		<input type="button" onclick="Call_AddCustomer()" value="保存客户资料" />
	</div>
</form>

<hr />

<div style="display: none;">
<p><b>服务器返回的结果：</b></p>
<textarea id="output" cols="20" rows="50" style="width: 90%; height: 200px"></textarea>
</div>


<p><b>这种方式过于古老，不推荐使用，这里不做演示，如果要查看调用方式，请参考页面中的 JavaScipt 代码。</b></p>


<script type="text/javascript">
function Call_Add(){
	WebService1.Add(1,2, ShowResult);
}

function Call_AddCustomer(){
	var customer = {Name: document.getElementById("txtName").value, 
					Age: document.getElementById("txtAge").value, 
					Address: document.getElementById("txtAddress").value, 
					Tel: document.getElementById("txtTel").value, 
					Email: document.getElementById("txtEmail").value};
	WebService1.AddCustomer(customer, ShowResult);
}

function ShowResult(result){
	document.getElementById("output").value = result;
}

</script>
</body>
</html>
