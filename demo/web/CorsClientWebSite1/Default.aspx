<%@ Page Language="VB" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>AJAX跨域演示站点</title>
    <style type="text/css">
        * {
            font-family: Consolas, '微软雅黑', '宋体';
            font-size: 12px;
        }
        ul {
            margin: 0px;
            padding: 0px 20px;
        }

        li {
            border-style: dashed none dashed none;
            border-width: 1px;
            border-color: #999999;
            padding: 5px 0px;
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
    <p><b>演示页面链接：</b></p>
    <p></p>
    <ul>
        <li><a href="/TestCorsAjax1.html" target="_blank">TestCorsAjax1.html</a><span>经过服务端中转的跨域调用</span></li>
        <li><a href="/TestCorsAjax2.html" target="_blank">TestCorsAjax2.html</a><span>前端AJAX直接跨域调用（IE8+, FireFox, Opera, Chrome, Safari）</span></li>
        <li><a href="/TestCorsAjax3.html" target="_blank">TestCorsAjax3.html</a><span>前端JSONP跨域调用</span></li>
   </ul>


    <p>&nbsp;</p>
    <hr />
    <p>&nbsp;</p><p>&nbsp;</p>

    <p><b>测试说明：</b></p>

<pre>
请确保在测试前，在本机创建三个域名，
可以在 C:\Windows\System32\drivers\etc\hosts 中指定：

127.0.0.1       <a href="http://www.fish-ajax-cors.com" target="_blank">www.fish-ajax-cors.com</a>
127.0.0.1       <a href="http://www.fish-mvc-demo.com" target="_blank">www.fish-mvc-demo.com</a>
</pre>


</body>
</html>
