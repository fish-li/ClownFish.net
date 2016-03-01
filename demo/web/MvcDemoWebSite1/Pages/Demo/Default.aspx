<%@ Page Title="杂项测试" Language="C#" MasterPageFile="MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
ul{ margin: 0px; padding: 0px 20px; }
li {
	border-style: dashed none dashed none;
	border-width: 1px;
	border-color: #999999;
	padding: 5px 0px;
}
li:hover {
	background-color: #FFE6CC;
}
li a{
	display: block;
	float: left;
	width: 260px;
}
li span{
	display: block;
	float: left;
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p><b>演示页面链接：</b></p>
<p></p>
<ul>
    <li><a href="/Pages/Demo/StdAjaxDemo.htm" target="_blank">StdAjaxDemo.htm</a><span>标准的AJAX用法（需要在URL指定命名空间或者命名空间别名）</span></li>
    <li><a href="/Pages/Demo/TestCustomerType.htm" target="_blank">TestCustomerType.htm</a><span>演示Action使用自定义的数据类型</span></li>
    <li><a href="/Pages/Demo/TestAuthorize/Default.aspx" target="_blank">TestAuthorize</a><span>演示身份认证与权限验证</span></li>
    <li><a href="/Pages/Demo/TestEnum.htm" target="_blank">TestEnum.htm</a><span>演示枚举类型的使用</span></li>
    <li><a href="/Pages/Demo/TestOutputCache.aspx" target="_blank">TestOutputCache.aspx</a><span>演示使用Action输出缓存</span></li>
    <li><a href="/Pages/Demo/TestAutoFindAction.htm" target="_blank">TestAutoFindAction.htm</a><span>演示根据submit按钮自动匹配Action</span></li>
    <li><a href="/Pages/Demo/TestSerializer.htm" target="_blank">TestSerializer.htm</a><span>演示XML, JSON序列化</span></li>
    <li><a href="/Pages/Demo/TestMvcRouting.htm" target="_blank">TestMvcRouting.htm</a><span>演示使用AP.NET Routing的使用</span></li>
    <li><a href="/Pages/Demo/404Page.aspx" target="_blank">404Page.aspx</a><span>演示404诊断页面</span></li>
    <li><a href="/Pages/Demo/TestFilePost.htm" target="_blank">TestFilePost.htm</a><span>演示上传文件</span></li>
    <li><a href="/Pages/Demo/TestFileDownload.aspx" target="_blank">TestFileDownload.aspx</a><span>演示下载文件</span></li>
    <li><a href="/Pages/Demo/TestDataConvert.html" target="_blank">TestDataConvert.htm</a><span>演示自定义的类型转换器，用于Action接收参数时类型转换</span></li>
    <li><a href="/Pages/Demo/TestTaskPage.aspx" target="_blank">TestTaskPage.aspx</a><span>演示基于Task的异步页</span></li>
</ul>




</asp:Content>

