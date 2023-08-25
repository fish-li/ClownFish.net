<%@ Page Language="C#" %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);
        this.Response.ContentType = "text/plain";

        if( this.Request.IsAuthenticated ) {
            string name = this.Request.QueryString["name"];

            HttpCookie cookie = this.Request.Cookies[name];
            string value = cookie == null ? "NULL" : cookie.Value;

            this.Response.Write(string.Format("Cookie: {0} = {1}", name, value));
        }
        else {
            this.Response.StatusCode = 403;
            this.Response.Write("当前页面仅限登录用户才能访问。");
        }

    }

</script>