<%@ Control Language="C#" Inherits="MyUserControlView<string>"  %>

<div id="webSiteLogo">
		<a href="/" title="回到网站首页"><img src="/static/Images/MyLab-logo.png" /></a></div>
<div id="currentPageTitle">
	<span><%= this.Model %></span></div>
<div id="topRightBar">
	<b class="appName">ClownFish.Web-DEMO</b>
	<a href="javascript:window.location = window.location;" title="刷新本页面" class="easyui-linkbutton" plain="true">
		<img src="/static/Images/refresh.gif" alt="refresh" /></a>
</div>
