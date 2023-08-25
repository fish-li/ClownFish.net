<%@ Page Language="C#"  %>

<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        string xa = this.Request.Headers["X-a"];
        string xb = this.Request.Headers["X-b"];
        this.Response.Headers.Add("X-add-result", xa + xb);
    }

</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <h2>Hello Test headers!</h2>
</body>
</html>
