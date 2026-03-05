<%@ Page Language="C#" %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        this.Response.ContentType = this.Request.Headers["Content-Type"];

        string len = this.Request.Headers["Content-Length"];
        if( string.IsNullOrEmpty(len) == false ) {
            this.Response.Headers.Add("Content-Length", len.ToString());
        }

        this.Request.InputStream.CopyTo(this.Response.OutputStream);
    }


</script>
