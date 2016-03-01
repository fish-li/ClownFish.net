<%@ Page Language="C#" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>演示文件下载</title>
    <style type="text/css">
        *{ 
            font-family: Consolas, '微软雅黑', '宋体';
            font-size: 12px;
        }
        fieldset{
            margin: 20px;
            padding: 20px;
        }
        legend{
            font-weight: bold;
        }
        form, div{
            line-height: 300%;
        }
    </style>

    <script type="text/javascript" src="/static/js/jquery/jquery-1.4.4.min.js"></script>
</head>
<body>
    <fieldset>
        <legend>表单提交下载</legend>
        <form action="/ajax/ns/TestFile/Download1.aspx" method="post">
            指定下载文件名：<input type="text" name="filename" value="中文 汉字,无乱码~`!@#$%^&*()_-+-=[]{}|:;',.<>?¥◆≠∞µαβπ™■.dat" style="width: 500px" />
            <input type="submit" name="submit" value="下载文件" />
        </form>
    </fieldset>


    <fieldset>
        <legend>链接包含文件的下载方式</legend>
        <div>
            指定下载文件名：<input type="text" id="filename" value="中文 汉字,无乱码~`!@#$%^&*()_-+-=[]{}|:;',.<>?¥◆≠∞µαβπ™■.dat" style="width: 500px" /><br />
            <button id="btnCreateLink">生成下载链接</button>
            <a id="linkDownload" href="#" target="_blank">下载文件</a>

        </div>
    </fieldset>

    <script type="text/javascript">
        $("#btnCreateLink").click(function () {
            var filename = encodeURIComponent($("#filename").val());
            $("#linkDownload").attr("href", "/file-download/demo1/" + filename);
        });

        $(function () {
            $("#btnCreateLink").click();
        });

    </script>




</body>
</html>
