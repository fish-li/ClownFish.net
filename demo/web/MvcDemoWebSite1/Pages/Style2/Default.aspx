<%@ Page Title="About ClownFish.Web" Language="C#" MasterPageFile="MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table border="0"><tr>
	<td style="vertical-align: top; min-width: 220px">
		<h1>关于本网站</h1>
		<p>网站演示了我的MVC框架。</p>
		<p></p>
		<p>采用简化版的Northwind业务场景</p>
		<p>数据库直接使用XML文件。</p>
		<p></p>
		<p>网站提供了二种不同的操作风格。</p>
		<p>您可以点击右边的图片来切换风格，</p>
		<p>也可以随时点击右上角图标来切换：</p>
		<p><img src="/static/Images/SetStyle.png" /></p>
	</td>
	<td>
		<p><a class="btnSetStyle hotLink" ps="Style2" href="javascript:;">
			<img src="/static/Images/Style2.png" title="点击此处切换到风格２" /></a>
		</p>
		
		<p> </p>
		<p><a class="btnSetStyle hotLink" ps="Style1" href="javascript:;">
			<img src="/static/Images/Style1.png" title="点击此处切换到风格１" /></a>
		</p>
	</td>
</tr>
</table>




</asp:Content>

