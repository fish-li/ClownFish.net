namespace ClownFish.UnitTest.Http.Mock;

public class MockRequestData
{
    public string HttpMethod { get; set; }

    public Uri Url { get; set; }

    public NameValueCollection Headers { get; set; }

    public byte[] Body { get; set; }


    public string GetHeader(string name) => this.Headers?.Get(name);

    public Stream InputStream {
        get {
            if( _inputStream != null )
                return _inputStream;

            if( this.Body == null )
                return null;

            //return new MemoryStream(this.Body, false);
            return new MockNetworkStream(this.Body);
        }
    }

    private Stream _inputStream;

    public void SetInputStream(Stream stream)
    {
        _inputStream = stream;
    }


    public string ToText()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.HttpMethod).Append(" ")
            .Append(this.Url.AbsoluteUri).AppendLine(" HTTP/1.1");

        if( this.Headers != null ) {
            foreach( string key in this.Headers.AllKeys ) {
                string[] values = this.Headers.GetValues(key);
                foreach( string value in values ) {
                    sb.Append(key).Append(": ").AppendLine(value);
                }
            }

            sb.AppendLine();
        }

        if( this.Body != null )
            sb.Append(Encoding.UTF8.GetString(this.Body));

        return sb.ToString();
    }


    public static MockRequestData FromText(string requestText)
    {
        if( requestText.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(requestText));

        MockRequestData data = new MockRequestData();
        data.Headers = new NameValueCollection();

        using( StringReader reader = new StringReader(requestText.Trim()) ) {

            string requestLine = reader.ReadLine();
            string[] items = requestLine.ToArray(' ');
            if( items.Length != 3 )
                throw new ArgumentException("请求行格式不正确。");

            data.HttpMethod = items[0];
            data.Url = new Uri(items[1]);

            string line = null;
            while( (line = reader.ReadLine()) != null ) {
                if( line.Length == 0 )
                    break;
                else {
                    int p = line.IndexOf(':');
                    if( p <= 0 )
                        throw new ArgumentException("请求头格式不正确。");

                    string name = line.Substring(0, p).Trim();
                    string value = line.Substring(p + 1).Trim();
                    data.Headers[name] = value;
                }
            }

            // 读取请求体数据
            string postText = reader.ReadToEnd();
            if( string.IsNullOrEmpty(postText) == false )
                data.Body = Encoding.UTF8.GetBytes(postText);
            else
                data.Body = Array.Empty<byte>();
        }

        return data;
    }


    //public static RequestData FromText(string requestText)
    //{
    //    if( requestText.IsNullOrEmpty() )
    //        throw new ArgumentNullException(nameof(requestText));

    //    HttpOption httpOption = HttpOption.FromRawText(requestText);

    //    RequestData data = new RequestData();
    //    data.HttpMethod = httpOption.Method;
    //    data.Url = new Uri(httpOption.Url);
    //    data.Headers = httpOption.Headers;

    //    if( HttpUtils.RequestHasBody(httpOption.Method) ) {
    //        if( httpOption.Data != null ) {
    //            data.Body = httpOption.Data.ToString().GetBytes();
    //        }
    //    }

    //    return data;
    //}

}
