<%@ Page Language="C#" %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);
        this.Response.ContentType = "text/plain";

        if( this.Request.IsAuthenticated ) {
            string name = this.Context.User.Identity.Name;
            this.Response.Write(string.Format("hello {0}, welcome!", name));
        }
        else {
            this.Response.StatusCode = 403;
            this.Response.Write("当前页面仅限登录用户才能访问。");
        }

    }

</script>