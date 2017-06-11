<%@ Page Language="C#" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ClownFish.Web DEMO</title>
    <style type="text/css">
        * {
            font-family: Consolas, '微软雅黑', '宋体';
            font-size: 12px;
        }
        h1{
            font-size: 14px;
        }
        ul {
            margin: 0px;
            padding: 0px 20px;
        }

        li {
            border-style: dashed none dashed none;
            border-width: 1px;
            border-color: #999999;
            padding: 10px 0px;
        }
        a{
            text-decoration:none;
        }

        li:hover {
            background-color: #FFE6CC;
        }

        li a {
            display: block;
            float: left;
            width: 260px;
        }

        li span {
            display: block;
            float: left;
        }
        pre{
            line-height: 180%;
        }
    </style>
</head>
<body>
    <h1>链接入口</h1>
    <p></p>
    <ul>
        <li><a href="/Pages/CodeExplorer.aspx" target="_blank">/Pages/CodeExplorer.aspx</a><span>在线查看源代码</span></li>
        <li><a href="/AjaxPK/Default.htm" target="_blank">/AjaxPK/Default.htm</a><span>AJAX PK （博客示例）</span></li>
        <li><a href="/Pages/Default.aspx" target="_blank">/Pages/Default.aspx</a><span>杂项演示</span></li>
        <li><a href="http://www.fish-ajax-cors.com/" target="_blank">www.fish-ajax-cors.com</a><span>AJAX跨域演示</span></li>
        <li><a href="http://www.fish-reverse-proxy.com/" target="_blank">www.fish-reverse-proxy.com</a><span>反向代理演示</span></li>
    </ul>



</body>
</html>
