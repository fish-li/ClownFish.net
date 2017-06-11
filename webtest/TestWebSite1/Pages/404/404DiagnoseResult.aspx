<%@ Page Language="C#" Inherits="MyPageView<ClownFish.Web.Debug404.DiagnoseResult>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Oh, 404 Error !</title>
    <%= UiHelper.RefCssFileHtml("/Pages/404/404DiagnoseResult.css", true)%>
</head>
<body>


<p id="title">Oh, 404 Error !</p>

<hr />


<div class="error">
<p><b>当前请求相关的错误信息：</b></p>
<ul>
<% foreach( string error in this.Model.ErrorMessages ) { %>
<li><%= error.HtmlEncode() %></li>
<% } %>
</ul>
</div>

<% if( this.Model.UrlActionInfo != null ) { %>
<div class="error">
<p><b>根据当前请求URL解析的基本信息：</b></p>
<table border="1" cellpadding="0" cellspacing="0">
<tr class="header"><td>Name</td><td>Value</td></tr>
<tr><td>UrlType</td><td><%= this.Model.UrlActionInfo.UrlType%></td></tr>
<tr><td>Namesapce</td><td><%= this.Model.UrlActionInfo.Namesapce%></td></tr>
<tr><td>ClassName</td><td><%= this.Model.UrlActionInfo.ClassName%></td></tr>
<tr><td>MethodName</td><td><%= this.Model.UrlActionInfo.MethodName%></td></tr>
<tr><td>ExtName</td><td><%= this.Model.UrlActionInfo.ExtName%></td></tr>
<tr><td>RoutePattern</td><td><%= this.Model.UrlActionInfo.RoutePattern%></td></tr>

<tr><td>Controller</td><td><%= this.Model.UrlActionInfo.Controller %></td></tr>
<tr><td>Action</td><td><%= this.Model.UrlActionInfo.Action%></td></tr>
<tr><td>Params</td><td>-------------------</td></tr>
<% if( this.Model.UrlActionInfo.Params != null && this.Model.UrlActionInfo.Params.Count > 0 ) { %>
<% foreach(string key in this.Model.UrlActionInfo.Params.AllKeys ){ %>
<tr><td>key</td><td><%= this.Model.UrlActionInfo.Params[key].HtmlEncode() %></td></tr>
<% } %>
<% } %>
</table>
</div>
<% } %>


<% if( this.Model.AssemblyList != null && this.Model.AssemblyList.Count > 0 ) { %>
<div class="error">
<p><b>程序当前加载的包含Action的程序集清单：</b></p>
<ul>
<% foreach( string asm in this.Model.AssemblyList ) { %>
<li><%= asm %></li>
<% } %>
</ul>
</div>
<% } %>



<% if( this.Model.PageUrlTestResult != null && this.Model.PageUrlTestResult.Count > 0 ){ %>
<div class="error">
<p><b>所有PageUrlAttribute的匹配测试结果：</b></p>
<table border="1" cellpadding="0" cellspacing="0">
<tr class="header"><td>PageUrl</td><td>IsPass</td></tr>

<% foreach( var result in this.Model.PageUrlTestResult ) { %>
<tr><td><%= result.Text.HtmlEncode() %></td><td><%= result.IsPass ? "PASS" : "no" %></td></tr>
<% } %>

</table>
</div>
<% } %>




<% if( this.Model.PageRegexUrlTestResult != null && this.Model.PageRegexUrlTestResult.Count > 0 ) { %>
<div class="error">
<p><b>所有PageRegexUrlAttribute的匹配测试结果：</b></p>
<table border="1" cellpadding="0" cellspacing="0">
<tr class="header"><td>Regex</td><td>IsPass</td></tr>

<% foreach( var result in this.Model.PageRegexUrlTestResult ) { %>
<tr><td><%= result.Text.HtmlEncode() %></td><td><%= result.IsPass ? "PASS" : "no" %></td></tr>
<% } %>

</table>
</div>
<% } %>





<% if( this.Model.NamespaceMapTestResult != null && this.Model.NamespaceMapTestResult.Count > 0 ) { %>
<div class="error">
<p><b>所有NamespaceMapAttribute的匹配测试结果：</b></p>
<table border="1" cellpadding="0" cellspacing="0">
<tr class="header"><td>ShortName</td><td>IsPass</td></tr>

<% foreach( var result in this.Model.NamespaceMapTestResult ) { %>
<tr><td><%= result.Text.HtmlEncode() %></td><td><%= result.IsPass ? "PASS" : "no" %></td></tr>
<% } %>

</table>
</div>
<% } %>






<% if( this.Model.RouteTestResult != null && this.Model.RouteTestResult.Count > 0 ) { %>
<div class="error">
<p><b>所有Route的匹配测试结果：</b></p>
<table border="1" cellpadding="0" cellspacing="0">
<tr class="header"><td>Pattern</td><td>IsPass</td></tr>

<% foreach( var result in this.Model.RouteTestResult ) { %>
<tr><td><%= result.Text.HtmlEncode() %></td><td><%= result.IsPass ? "PASS" : "no" %></td></tr>
<% } %>

</table>
</div>
<% } %>





<% if( this.Model.ControllerTestResult != null && this.Model.ControllerTestResult.Count > 0 ) { %>
<div class="error">
<p><b>所有Service Controller的匹配测试结果：</b></p>
<table border="1" cellpadding="0" cellspacing="0">
<tr class="header"><td>Controller</td><td>IsPass</td></tr>

<% foreach( var result in this.Model.ControllerTestResult ) { %>
<tr><td><%= result.Text.HtmlEncode() %></td><td><%= result.IsPass ? "PASS" : "no" %></td></tr>
<% } %>

</table>
</div>
<% } %>








<% if( this.Model.ActionTestResult != null && this.Model.ActionTestResult.Count > 0 ) { %>
<div class="error">
<p><b>当前请求与 <%= this.Model.ControllerType %>匹配，<br />所有Action的匹配测试结果：</b></p>
<table border="1" cellpadding="0" cellspacing="0">
<tr class="header"><td>Controller</td><td>IsPass</td><td>Reason</td></tr>

<% foreach( var result in this.Model.ActionTestResult ) { %>
<tr><td><%= result.Text.HtmlEncode() %></td><td><%= result.IsPass ? "PASS" : "no" %></td><td><%= result.Reason %></td></tr>
<% } %>

</table>
</div>
<% } %>






</body>
</html>
