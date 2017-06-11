<%@ Page Async="true" Language="C#" Inherits="MyPageView<DEMO.Models.PageData.TestTaskPageViewData>" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        *{ 
            font-family: Consolas, '微软雅黑', '宋体';
            font-size: 12px;
        }
        textarea{
            width: 90%;
            height: 150px;
        }
        div.result-item {
            padding: 3px;
            border-top: dashed 1px #808080;
        }
        div.url{
            font-weight: bold;
            padding: 3px 0;
        }
        div.title{
            padding-bottom: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <p><b>请输入要查询页面标题的URL地址，一行一个URL</b></p>
        <p><textarea name="urls" rows="20" cols="50"><%= this.Model.Input %></textarea>
            <br />
            <input type="submit" name="submit" value="提交" />
        </p>

 <% if( this.Model.Result != null ){ %>
        <hr />

        <p><b>查询结果：</b></p>
        <% foreach(var item in this.Model.Result ){ %>
            <div class="result-item">
                <div class="url"><%= item.Key.HtmlEncode() %></div>
                <div class="title"><%= item.Value.HtmlEncode() %></div>
            </div>
        <% } %>
<% } %>

    </form>
</body>
</html>
