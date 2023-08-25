<%@ Page Language="C#"  %>

<script runat="server">

    //protected override void OnPreLoad(EventArgs e)
    //{
    //    base.OnPreLoad(e);
    //}

    private Dictionary<string, string> GetRequestProperties()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict["HttpMethod"] = this.Request.HttpMethod;
        dict["FilePath"] = this.Request.FilePath;
        dict["Path"] = this.Request.Path;
        dict["PhysicalPath"] = this.Request.PhysicalPath;
        dict["Url"] = this.Request.Url.AbsoluteUri;
        dict["RawUrl"] = this.Request.RawUrl;
        dict["PathInfo"] = this.Request.PathInfo;
        dict["User-Agent"] = this.Request.UserAgent;
        dict["Accept"] = string.Join(",", this.Request.AcceptTypes ?? new string[0]);
        dict["UserLanguages"] = string.Join(",",  this.Request.UserLanguages ?? new string[0]);
        dict["UrlReferrer"] = this.Request.UrlReferrer == null ? "NULL" : this.Request.UrlReferrer.OriginalString;
        dict["Content-Type"] = this.Request.ContentType;
        dict["Content-Length"] = this.Request.ContentLength.ToString();
        dict["Content-Encoding"] = this.Request.ContentEncoding.ToString();
        dict["IsAuthenticated"] = this.Request.IsAuthenticated.ToString();
        return dict;
    }

</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <h2>Request Properties</h2>
    <ul>
        <% foreach( var x in this.GetRequestProperties() ) { %>
            <li><b><%: x.Key %></b> <span><%: x.Value %></span></li>
        <% } %>
    </ul>
    <hr />

    <h2>QueryString</h2>
    <ul>
        <% foreach( string x in this.Request.QueryString.Keys ) { %>
            <li><b><%: x %></b> <span><%: this.Request.QueryString[x] %></span></li>
        <% } %>
    </ul>
    <hr />


    <h2>Headers</h2>
    <ul>
        <% foreach( string x in this.Request.Headers.Keys ) { %>
            <li><b><%: x %></b> <span><%: this.Request.Headers[x] %></span></li>
        <% } %>
    </ul>
    <hr />

    <h2>Cookies</h2>
    <ul>
        <% foreach( string x in this.Request.Cookies.Keys ) { %>
            <li><b><%: x %></b> <span><%: this.Request.Cookies[x] %></span></li>
        <% } %>
    </ul>
    <hr />

    <h2>Form</h2>
    <ul>
        <% foreach( string x in this.Request.Form.Keys ) { %>
            <li><b><%: x %></b> <span><%: this.Request.Form[x] %></span></li>
        <% } %>
    </ul>
    <hr />

    <h2>Files</h2>
    <ul>
        <% foreach( string name in this.Request.Files.AllKeys ) {
                HttpPostedFile file = this.Request.Files[name];
                %>
            <li><b><%: name %></b> <span>FileName: <%: file.FileName %>, ContentLength: <%: file.ContentLength %>, ContentType: <%: file.ContentType %></span></li>
        <% } %>
    </ul>
    <hr />


</body>
</html>
