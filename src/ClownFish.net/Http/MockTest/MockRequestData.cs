namespace ClownFish.Http.MockTest;

/// <summary>
/// 
/// </summary>
public class MockRequestData
{
    /// <summary>
    /// 
    /// </summary>
    public string HttpMethod { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Uri Url { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public NameValueCollection Headers { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public byte[] Body { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetHeader(string name) => this.Headers?.Get(name);

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public void SetInputStream(Stream stream)
    {
        _inputStream = stream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestText"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
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
                throw new FormatException("开始行格式不正确!");

            data.HttpMethod = items[0];
            data.Url = new Uri(items[1]);

            string line = null;
            while( (line = reader.ReadLine()) != null ) {
                if( line.Length == 0 )
                    break;
                else {
                    int p = line.IndexOf(':');
                    if( p <= 0 )
                        throw new FormatException("请求头格式不正确!");

                    string name = line.Substring(0, p).Trim();
                    string value = line.Substring(p + 1).Trim();
                    data.Headers[name] = value;
                }
            }

            // 读取请求体数据
            string postText = reader.ReadToEnd();
            if( string.IsNullOrEmpty(postText) == false )
                data.Body = Encoding.UTF8.GetBytes(postText);
            //else
            //    data.Body = Array.Empty<byte>();
        }

        return data;
    }



}
