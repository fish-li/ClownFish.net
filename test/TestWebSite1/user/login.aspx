<%@ Page Language="C#" %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);
        this.Response.ContentType = "text/plain";

        string name = this.Request.Form["name"];

        if( string.IsNullOrEmpty(name) == false ) {
            System.Web.Security.FormsAuthentication.SetAuthCookie(name, true);

            HttpCookie cookie2 = new HttpCookie("LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.Response.Cookies.Add(cookie2);

            this.Response.Write("Login OK.");
        }
        else {
            this.Response.Write("Login failed.");
        }
    }

</script>