<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        *{ 
            font-family: Consolas, '微软雅黑', '宋体';
            font-size: 12px;
        }
    </style>
</head>
<body>
    <p>
		当前用户名：<%= HttpContextHelper.UserIdentityName2 ?? "[未登录用户]" %>
    </p>
    <hr />


<form action="/user/Login.aspx" method="post">
<fieldset>
	<legend>登录操作</legend>
	用户名：	<input type="text" name="username" style="width: 200px" value="fish" /> 
    权限号： <input type="text" name="rightNo" style="width: 100px" value="11" /> 
	<input type="submit" name="Login" value="登录" />
</fieldset>
</form>


<form action="/user/Logout.aspx" method="post">
<fieldset>
	<legend>注销操作</legend>
	<input type="submit" name="Logout" value="注销" />
</fieldset>
</form>

<hr />

<p><a href="AllUser.aspx" target="outFrame">goto AllUser.aspx</a></p>
<p><a href="LoginUser.aspx" target="outFrame">goto LoginUser.aspx</a></p>
<p><a href="Fish.aspx" target="outFrame">goto Fish.aspx</a></p>
<p><a href="RightNo23.aspx" target="outFrame">goto RightNo23.aspx</a></p>

<iframe name="outFrame" style="width: 99%; height: 220px"></iframe>

</body>
</html>
