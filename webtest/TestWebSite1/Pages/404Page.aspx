<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
	string[] links = new string[] {
		"/aaaaaaaaaaaa.aspx",
		"/ajax/aaaaaaaaaaaaa.aspx",
		"/Ajax/aaaa/Demo3/GetMd5.aspx",
		"/Ajax/Demo3/GetMd5.aspx",
		"/Ajax/Demo/GetMd1.aspx",
		"/api/ns/Demo1/Test1.aspx",
		"/api/ns/Demo1/Test2.aspx",
		"/Ajax/TestAutoAction/submit.aspx?xxxx=1",
		"/Ajax/Fish.AA.Test/Add5.aspx",
		"/mvc-routing/DEMO.Controllers.Services/TestAutoAction/mmmmm.aspx",
		"/mvc-routing/DEMO.Controllers.Services/TestAutoAction/mmmmm",
		"/no-way/DEMO.Controllers.Services/TestAutoAction/mmmmm",
		"aaa/bbb/cccccc"
	
};
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>测试404诊断页</title>
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

            padding: 5px 0px;
        }

            li:hover {
                background-color: #FFE6CC;
            }


    </style>
</head>
<body>

<p><b>测试404诊断页</b></p>
<ul>
<% foreach( string link in links ) { %>
<li><a href="<%= link %>" target="_blank"><%= link %></a></li>
<% } %>
</ul>



</body>
</html>
