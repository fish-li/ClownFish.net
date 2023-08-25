<%@ Page Language="C#"  %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        StringBuilder sb = new StringBuilder();
        sb.Append("Content-Type: ").AppendLine(this.Request.ContentType);
        sb.Append("ContentLength: ").AppendLine(this.Request.ContentLength.ToString());
        sb.AppendLine();

        sb.AppendLine(GetRequestBody());

        this.Response.ContentType = "text/plain";
        this.Response.Write(sb.ToString());
    }


    private string GetRequestBody()
    {
        string method = this.Request.HttpMethod;
        string contentType = this.Request.ContentType.ToLower();

        bool hasBody = method == "POST" || method == "PUT";
        if( hasBody == false )
            return null;

        bool bodyIsText = contentType.StartsWith("text/")
                        || contentType.StartsWith("application/json")
                        || contentType.StartsWith("application/xml")
                        || contentType.StartsWith("application/x-www-form-urlencoded");

        if( bodyIsText ) {
            this.Request.InputStream.Position = 0;
            using( var reader = new System.IO.StreamReader(this.Request.InputStream, Encoding.UTF8) ) {
                return reader.ReadToEnd();
            }
        }
        else {
            byte[] bb = new byte[this.Request.InputStream.Length];
            this.Request.InputStream.Read(bb, 0, bb.Length);
            return Convert.ToBase64String(bb);
        }
    }

</script>