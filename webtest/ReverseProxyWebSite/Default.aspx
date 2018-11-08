<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    protected void SetSiteCookie(string siteAddress)
    {
        HttpCookie cookie = ClownFish.Web.Proxy.ReverseProxyModule.CreateProxySiteCookie(siteAddress);

        this.Response.Cookies.Add(cookie);
    }

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        string[] target = this.Request.QueryString.GetValues("target");
        if( target == null || target.Length == 0 )
            return;

        if( string.IsNullOrEmpty(target[0]) == false )
            SetSiteCookie(target[0]);
        else {
            if( string.IsNullOrEmpty(target[1]) == false )
                SetSiteCookie(target[1]);
        }

        this.Response.Redirect("/");
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>选择目标站点</title>
    <style type="text/css">
        * {
            font-family: Consolas, '微软雅黑', '宋体';
            font-size: 12px;
        }

        h1 {
            font-size: 22px;
            margin-top: 20px;
            margin-left: 10px;
            margin-bottom: 30px;
        }

        div.siteList {
            margin: 10px;
            width: 850px;
        }

        div.siteList a {
            display: table-cell;
            text-align: center;
            padding-top: 30px;
            width: 200px;
            height: 50px;
            background-color: #9fcef7;
            margin: 3px;
            float: left;
            text-decoration: none;
            font-size: 16px;
        }

        div.siteList a:hover {
            text-decoration: none;
            background-color: #b6ff00;
        }

        div.siteList a.incompatible {
            color: #808080;
        }

        div.clear {
            clear: both;
        }

        p.comment {
            margin-top: 35px;
            margin-left: 10px;
            font-size: 14px;
            line-height: 180%;
        }
    </style>
</head>
<body>
    <h1>请选择要浏览的站点</h1>

    <div class="siteList">
        
        <a href="http://www.fish-ajax-cors.com/">fish-ajax-cors.com</a>
        <a href="http://www.fish-web-demo.com/">fish-web-demo.com</a>
        <a href="http://cn.bing.com/">cn.bing.com</a>
        <a href="http://email.163.com/">email.163.com</a>
        <a href="http://www.aliyun.com/">www.aliyun.com</a>
        <a href="http://baike.baidu.com/">baike.baidu.com</a>
        <a href="http://www.dingtalk.com/">www.dingtalk.com</a>
        <a href="http://www.163.com/">www.163.com</a>
        <a href="http://www.hao123.com/">www.hao123.com</a>

    </div>

    <div class="clear"></div>

    <p class="comment">
        说明：<br />
        1、如果需要切换到其它站点，请关闭浏览器程序（APP要退出应用），然后再次打开本页面重新选择目标站点。<br />
        2、灰色字体的站点由于目标网站本身问题，生成了固定链接，不支持代理访问，所以有兼容性缺陷。

    </p>


    <script type="text/javascript" src="/jquery-1.8.1.js"></script>
    <script type="text/javascript">
        $(function () {
            $("div.siteList a").each(function (i) {
                var href = $(this).attr("href");
                var newAddress = "/?" + $.param({ target: href });
                $(this).attr("href", newAddress);
            });
        });

    </script>
</body>
</html>
