<%@ Page Language="C#"  %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        StringBuilder sb = new StringBuilder();

        foreach(var kv in this.GetRequestProperties())
            sb.Append(kv.Key).Append(": ").AppendLine(kv.Value);

        FillCollection(this.Request.QueryString, "QueryString", sb);
        FillCollection(this.Request.Headers, "Headers", sb);
        FillCollection(this.Request.Form, "Form", sb);

        if( this.Request.Cookies.Count > 0 ) {
            sb.AppendLine().AppendLine()
                .AppendLine("-------------------------------------------------------")
                .AppendLine("Cookies")
                .AppendLine("-------------------------------------------------------");
            foreach( var key in this.Request.Cookies.AllKeys ) {
                var cookie = this.Request.Cookies[key];
                sb.AppendFormat("{0} = {1}\r\n", key, cookie.Value);
            }
        }


        if( this.Request.Files.Count > 0 ) {
            sb.AppendLine().AppendLine()
                .AppendLine("-------------------------------------------------------")
                .AppendLine("Files")
                .AppendLine("-------------------------------------------------------");

            foreach( string name in this.Request.Files.AllKeys ) {
                HttpPostedFile file = this.Request.Files[name];
                sb.AppendFormat("{0}: {1}, {2}, {3}\r\n", name, file.FileName, file.ContentLength, GetFileMd5(file, name));
            }
        }
        else {
            sb.AppendLine().AppendLine()
               .AppendLine("-------------------------------------------------------")
               .AppendLine("Files  not found!")
               .AppendLine("-------------------------------------------------------");
        }


        sb.AppendLine().AppendLine()
            .AppendLine("-------------------------------------------------------")
            .AppendLine("Body")
            .AppendLine("-------------------------------------------------------")
            .AppendLine(GetRequestBody());


        this.Response.ContentType = "text/plain";
        this.Response.Write(sb.ToString());
    }


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

    private void FillCollection(NameValueCollection collection, string name, StringBuilder sb)
    {
        if( collection.Count == 0 )
            return;

        sb.AppendLine().AppendLine()
            .AppendLine("-------------------------------------------------------")
            .AppendLine(name)
            .AppendLine("-------------------------------------------------------");

        foreach(string key in collection.Keys ) {
            string[] values = collection.GetValues(key);
            foreach(var x in values ) {
                sb.AppendFormat("{0} = {1}\r\n", key, x);
            }
        }
    }

    private string GetFileMd5(HttpPostedFile file, string name)
    {
        var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        var bytes = new byte[file.ContentLength];
        file.InputStream.Read(bytes, 0, bytes.Length);

        string result = BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", "");

        this.Response.Headers.Add("x-FileMd5-" + name, result);
        return result;
    }


</script>