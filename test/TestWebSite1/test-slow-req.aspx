<%@ Page Language="C#"  %>

<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        // 故意休眠，让请求堆积在IIS队列中
        System.Threading.Thread.Sleep(500);

        this.Response.AddHeader("x-status", "OK");
    }


</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <h2>Hello test-slow-req!</h2>

</body>
</html>
