namespace ClownFish.Base.WebClient;

/// <summary>
/// 读取HttpWebResponse的工具类
/// </summary>
public sealed class ResponseReader : IDisposable
{
    private readonly HttpWebResponse _response;

    private readonly bool _autoDecompress;

    private Stream _responseStream;

    private string _contentType;

    /// <summary>
    /// 是否需要自动关闭Response流
    /// </summary>
    private bool _autoCloseResponseStream = true;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="response"></param>
    /// <param name="autoDecompress"></param>
    public ResponseReader(HttpWebResponse response, bool autoDecompress = false)
    {
        if( response == null )
            throw new ArgumentNullException("response");

        _response = response;
        _autoDecompress = autoDecompress;
    }

    /// <summary>
    /// 获取指定类型的结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Read<T>()
    {
        _responseStream = GetResponseStream();

        _contentType = _response.ContentType;

        Type resultType = typeof(T);

        // 先判断是不是 HttpResult<T> 的子类型
        if( resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(HttpResult<>) ) {
            Type argType = resultType.GetGenericArguments()[0];
            MethodInfo method = this.GetType()
                                    .GetMethod(nameof(GetHttpResult), BindingFlags.Instance | BindingFlags.NonPublic)
                                    .MakeGenericMethod(argType);
            try {
                return (T)method.FastInvoke(this, null);
            }
            catch( TargetInvocationException ex1 ) {
                throw ex1.InnerException;
            }
        }
        else {
            return GetResult<T>();
        }
    }


    private T GetResult<T>()
    {
        if( typeof(T) == typeof(byte[]) ) {
            // 二进制，就直接读取，忽略字符编码
            return (T)(object)ReadResponseAsBytes();
        }
        else if( typeof(T) == typeof(Stream) ) {
            // 二进制，就直接返回
            return (T)(object)ReadResponseAsStream();
        }
        else {
            // 其它类型的结果，先得到字符串，再做反序列化处理
            string responseText = ReadResponseAsText(_responseStream, _contentType);

            // 转换结果
            return ConvertResult<T>(responseText, _contentType);
        }
    }


    private HttpResult<T> GetHttpResult<T>()
    {
        int statusCode = (int)_response.StatusCode;
        var header = _response.GetAllHeaders();
        var body = GetResult<T>();

        return new HttpResult<T>(statusCode, header, body);
    }


    private Stream GetResponseStream()
    {
        Stream responseStream = _response.GetResponseStream();

        if( _autoDecompress ) {

            // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Content-Encoding
            string contentEncoding = _response.ContentEncoding;
            if( contentEncoding.HasValue() ) {
                return HttpStreamReader.WrapperCompressionStream(responseStream, contentEncoding, CompressionMode.Decompress);
            }
            // else 没有指定 “Content-Encoding”，也就是没有使用压缩格式
        }

        return responseStream;
    }

    private byte[] ReadResponseAsBytes()
    {
        return _responseStream.ToArray();
    }

    private Stream ReadResponseAsStream()
    {
        _autoCloseResponseStream = false;
        return _responseStream;
    }

    internal static T ConvertResult<T>(string responseText, string contentType)
    {
        if( typeof(T) == typeof(string) )
            return (T)(object)responseText;


        // 如果响应结果为空，就直接返回类型的默认值（NULL）
        if( string.IsNullOrEmpty(responseText) )
            return default(T);


        if( contentType.IndexOfIgnoreCase(ResponseContentType.Json) >= 0 )
            return JsonExtensions.FromJson<T>(responseText);

        else if( contentType.IndexOfIgnoreCase(ResponseContentType.Xml) >= 0 )
            return XmlHelper.XmlDeserialize<T>(responseText);

        else
            //return (T)Convert.ChangeType(responseText, typeof(T));
            return (T)StringConverter.ChangeType(responseText, typeof(T));
    }


    internal static string ReadResponseAsText(Stream responseStream, string contentType)
    {
        // 共有 4 种场景
        // 1，contentType is null  , 按 UTF-8 方式读取
        // 2，contentType: xxxxx; charset=encoding  , 按 encoding 方式读取
        // 3，contentType: xxxxx  , 按 UTF-8 方式读取
        // 4，contentType: text/html ，先解析html，找到 charset，再重新按charset的编码方式读取，此时可能会需要读取2次，因此需要引入一个临时流
        // 说明：如果响应内容是文本，场景2是规范的，其它都是不规范的！


        if( contentType.IsNullOrEmpty() )    // 场景 1
            return ReadText(responseStream, Encoding.UTF8);


        Encoding encoding = GetEncodingFromContentType(contentType);
        if( encoding != null )  // 场景 2
            return ReadText(responseStream, encoding);


        bool isHtml = contentType.StartsWithIgnoreCase(ResponseContentType.Html);
        if( isHtml == false )     // 场景 3
            return ReadText(responseStream, Encoding.UTF8);
        else
            return ReadHtml(responseStream, Encoding.UTF8, out Encoding htmlEncoding);    // 场景 4, html
    }


    /// <summary>
    /// 按 tryEncoding 的编码读取流，并在读取的过程中检查 【HTML头部】有没有指定 charset，
    /// 如果 有 指定，则按 charset对应的编码重新读取，
    /// 如果 没有 指定，继续按tryEncoding读取整个流。
    /// </summary>
    /// <param name="responseStream"></param>
    /// <param name="tryEncoding"></param>
    /// <param name="htmlEncoding">HTML页面中meta指示的编码</param>
    /// <returns></returns>
    internal static string ReadHtml(Stream responseStream, Encoding tryEncoding, out Encoding htmlEncoding)
    {
        htmlEncoding = null;
        bool outOfHtmlHead = false;

        string line = null;
        //StringBuilder html = new StringBuilder(1024*4);            

        // 为了保证流能支持 2 次读取，先把“响应流”转换(复制)成 MemoryStream
        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {
            responseStream.CopyToMemoryStream(ms);

            StringBuilder html = StringBuilderPool.Get();
            try {
                // 按 tryEncoding 的编码方式读取，也有可能一直读到结束
                using( StreamReader reader = new StreamReader(ms, tryEncoding, true, 1024, leaveOpen: true) ) {
                    while( (line = reader.ReadLine()) != null ) {

                        html.AppendLine(line);

                        if( outOfHtmlHead == false && line.IndexOfIgnoreCase("</head>") >= 0 ) {
                            outOfHtmlHead = true;

                            // 检查HTML头的元数据值
                            Encoding headerEncoding = GetEncodingFromHtmlHeader(html.ToString());
                            if( headerEncoding != null && headerEncoding != tryEncoding ) {

                                // 停止当前读取过程，需要使用新的 charset编码来读取
                                htmlEncoding = headerEncoding;
                                break;

                            }
                            // else 
                            // 如果HTML头中没有指定编码 或者 默认的编码和HTML中的编码一致，那么就不用切换编码，一直读取到结束
                        }
                    }
                }

                if( htmlEncoding == null )
                    return html.ToString();
            }
            finally {
                StringBuilderPool.Return(html);
            }

            // 按新的编码再次读取
            return ReadText(ms, htmlEncoding);
        }
    }


    internal static string ReadText(Stream stream, Encoding encoding)
    {
        if( stream.CanSeek )
            stream.Position = 0;

        if( stream.CanRead == false )
            return string.Empty;

        using( StreamReader reader = new StreamReader(stream, encoding, true, 1024, true) ) {
            return reader.ReadToEnd();
        }
    }


    // <meta http-equiv="charset"  content="iso-8859-1">
    private static readonly Regex s_htmlCharsetRegex = new Regex(
                @"<meta\s+http-equiv=[\'\#]charset[\'\#]\s+content=[\'\#](?<chartset>[\w-]+)[\'\#]\s*\/?>".Replace('#', '\"'),
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // <meta charset="utf-8">
    private static readonly Regex s_htmlCharsetRegex2 = new Regex(
                @"<meta\s+charset=[\'\#](?<chartset>[\w-]+)[\'\#]\s*\/?>".Replace('#', '\"'),
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    private static readonly Regex s_htmlContentTypeRegex = new Regex(
                @"<meta\s+http-equiv=[\'\#]content-Type[\'\#]\s+content=[\'\#][\w\/]+;\s*charset=(?<chartset>[\w-]+)[\'\#]\s*\/?>".Replace('#', '\"'),
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Content-Type: text/html; charset=utf-8
    private static readonly Regex s_httpHeaderContentTypeRegex = new Regex(
                @"^[\w\/]+;\s*charset=\""?(?<chartset>[\w-]+)\""?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);



    internal static Encoding GetEncodingFromHtmlHeader(string text)
    {
        if( string.IsNullOrEmpty(text) )
            return null;

        Match m = s_htmlCharsetRegex.Match(text);
        if( m.Success == false )
            // 再匹配一次
            m = s_htmlCharsetRegex2.Match(text);

        if( m.Success == false )
            // 再匹配一次
            m = s_htmlContentTypeRegex.Match(text);

        if( m.Success ) {
            string charset = m.Groups["chartset"].Value;
            return GetEncodingFromString(charset);
        }
        return null;
    }


    internal static Encoding GetEncodingFromContentType(string contentType)
    {
        // 说明：直接使用 response.CharacterSet 不靠谱！
        //      因为如果响应头不指定编码，它就默认返回 "ISO-8859-1"，最后也不知道是不是真的是"ISO-8859-1"编码，所以干脆不用这个属性。

        if( string.IsNullOrEmpty(contentType) )
            return null;

        Match m = s_httpHeaderContentTypeRegex.Match(contentType);
        if( m.Success ) {
            string charset = m.Groups["chartset"].Value;
            return GetEncodingFromString(charset);
        }
        return null;
    }


    internal static Encoding GetEncodingFromString(string encodingName)
    {
        if( string.IsNullOrEmpty(encodingName) )
            return null;

        try {
            return Encoding.GetEncoding(encodingName);
        }
        catch {
            /* 忽略无效的 charset 值 */
            return null;
        }
    }



#region IDisposable 成员

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    void IDisposable.Dispose()
    {
        if( _autoCloseResponseStream ) {
            if( _responseStream != null ) {
                _responseStream.Dispose();
                _responseStream = null;
            }
        }
    }

#endregion
}
