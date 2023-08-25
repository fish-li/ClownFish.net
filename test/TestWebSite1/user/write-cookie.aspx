<%@ Page Language="C#" %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);
        this.Response.ContentType = "text/plain";

        if( this.Request.IsAuthenticated ) {
            string name = this.Request.Form["name"];
            string value = this.Request.Form["value"];

            HttpCookie cookie = new HttpCookie(name, value);
            cookie.Expires = DateTime.Now.AddDays(5);
            this.Response.Cookies.Add(cookie);

            // 多写一个
            HttpCookie cookie2 = new HttpCookie("xtime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.Response.Cookies.Add(cookie2);

            this.Response.Write(string.Format("Write Cookie OK: {0} = {1}", name, value));
        }
        else {
            this.Response.StatusCode = 403;
            this.Response.Write("当前页面仅限登录用户才能访问。");
        }

    }

</script>