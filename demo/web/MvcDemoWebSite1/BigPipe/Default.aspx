<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>BigPipe DEMO  - http://www.cnblogs.com/fish-li</title>
<style type="text/css">
p{
	line-height: 150%;
}
</style>
</head>
<body>

<p><b>关于BigPipe示例的说明</b></p>

<p>这是一个演示在ASP.NET中实现BigPipe的示例，<br />
通常说来，当使用BigPipe技术时，需要将页面划分了不同的Pagelet，<br />
每个Pagelet是一个独立的显示区域，它有自己的CSS, JS引用文件，<br />
最后由后端插入特定的JavaScript代码调用前端的BigPipe框架，<br />
当按照这种方法实现时，我发现比较麻烦，所以，在这个示例中，没有Pagelet的概念，<br />
我认为BigPipe的目标应该是实现前端与后端能并行工作，<br />
而并行工作的核心基础就是【块编码】，所以，我将BigPipe的实现做了简化，<br />
在后端用UserControl来代替了Pagelet，css, js则由页面统一加载（与传统的方式一致），<br />
为了让BigPipe的实现更容易，我使用了<a href="http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html" target="_blank">ClownFish.Web</a>框架。
</p>


<p>
<a href="BigPipeDemo.aspx" >点击此处查看演示效果</a>
</p>



</body>
</html>
