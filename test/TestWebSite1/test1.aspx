<%@ Page Language="C#"  %>

<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        this.Response.AddHeader("x-status", "OK");

        HttpCookie cookie1 = new HttpCookie("c1", null);
        cookie1.Expires = DateTime.Now.AddDays(-3);
        this.Response.Cookies.Add(cookie1);

        HttpCookie cookie2 = new HttpCookie("c2", "xxxxxxx");
        cookie2.Expires = DateTime.Now.AddDays(3);
        this.Response.Cookies.Add(cookie2);

        string flag = this.Request.QueryString["x-result-CompressionMode"];
        if( flag == "gzip" ) {
            this.Response.Filter = new System.IO.Compression.GZipStream(this.Response.Filter, System.IO.Compression.CompressionLevel.Fastest);
            this.Response.AddHeader("Content-Encoding", "gzip");
        }
    }


</script><!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Hello Test!</title>
</head>
<body>
    <h2>Hello Test!</h2>

</body>
</html>
