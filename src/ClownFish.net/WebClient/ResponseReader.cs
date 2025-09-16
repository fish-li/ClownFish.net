using ClownFish.Base.Json;

namespace ClownFish.WebClient;

/// <summary>
/// 读取HttpWebResponse的工具类
/// </summary>
public sealed class ResponseReader : IDisposable
{
    private readonly HttpWebResponse _response;

    private long _maxLimitLen;

    private Stream _responseStream;

    /// <summary>
    /// 是否需要自动关闭Response流
    /// </summary>
    private bool _autoCloseResponseStream = true;   // 当返回类型为 Stream 时设置为 false

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="response">HTTP响应对象</param>
    /// <param name="maxLimitLen">最大允许的响应体长度，可以不指定</param>
    public ResponseReader(HttpWebResponse response, long maxLimitLen = 0)
    {
        if( response == null )
            throw new ArgumentNullException(nameof(response));

        _response = response;
        _maxLimitLen = maxLimitLen;
    }

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

    /// <summary>
    /// 获取指定类型的结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Read<T>()
    {
        _responseStream = GetResponseStream(typeof(T));

        Type resultType = typeof(T);

        // 先判断是不是 HttpResult<T> 的子类型
        if( resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(HttpResult<>) ) {
            Type argType = resultType.GetGenericArguments()[0];
            MethodInfo method = this.GetType()
                                    .GetMethod(nameof(GetHttpResult000), BindingFlags.Instance | BindingFlags.NonPublic)
                                    .MakeGenericMethod(argType);
            return (T)method.FastInvoke(this, null);
        }
        else {
            return GetResult<T>();
        }
    }

    private Stream GetResponseStream(Type returnType)
    {
        Stream responseStream = _response.GetResponseStream();

        bool isBinaryDataType = returnType == typeof(byte[]) || returnType == typeof(Stream)
                              || returnType == typeof(HttpResult<byte[]>) || returnType == typeof(HttpResult<Stream>);

        bool autoDecompress = isBinaryDataType ? false : true;   // 如果返回结果是二进制数据，就不做“自动解压缩”

        if( autoDecompress ) {
            string contentEncoding = _response.ContentEncoding;
            if( contentEncoding.HasValue() ) {
                return responseStream.CreateCompressionStream(contentEncoding, CompressionMode.Decompress, false);
            }
        }

        return responseStream;
    }

    private HttpResult<T> GetHttpResult000<T>()
    {
        int statusCode = (int)_response.StatusCode;
        var header = _response.GetAllHeaders();
        var body = GetResult<T>();

        return new HttpResult<T>(statusCode, header, body);
    }

    private T GetResult<T>()
    {
        if( typeof(T) == typeof(byte[]) ) {
            long maxLimitLen = CheckMaxLimitLen();
            // 二进制，就直接读取，忽略字符编码
            return (T)(object)ReadResponseAsBytes(_responseStream, maxLimitLen);
        }

        if( typeof(T) == typeof(Stream) ) {
            // 二进制，就直接返回
            return (T)(object)ReadResponseAsStream();   // 这里不读取响应流内容
        }

        // 按文本方式读取流，并根据contentType执行相关的转换
        string contentType = _response.ContentType ?? string.Empty;
        return ReturnResultFromTextStream<T>(_responseStream, contentType);
    }


    internal long CheckMaxLimitLen()
    {
        if( _maxLimitLen <= 0 )
            return 0;

        // 这里也有可能读不到长度（值：-1），例如：Transfer-Encoding: chunked
        long contentLength = _response.ContentLength;

        // 先尝试直接根据ContentLength请求头 检查 maxAllowLen
        if( contentLength > 0 ) {
            if( contentLength > _maxLimitLen )
                throw new ResponseBodyTooLargeException(_maxLimitLen);
            else
                _maxLimitLen = -1;   // ContentLength请求头存在，并且没有触发上面的异常检查，就不需要再检查了
        }
        // else 
        // Transfer-Encoding: chunked 场景，需要在读取响应体的时候执行长度检查

        return _maxLimitLen;
    }

    internal static byte[] ReadResponseAsBytes(Stream responseStream, long maxLimitLen = 0)
    {
        if( maxLimitLen <= 0 ) {   // 不检查长度
            return responseStream.ToArray();
        }


        // 读取流，并检查最大了限制长度
        using( MemoryStream ms2 = MemoryStreamPool.GetStream() ) {
            using( ByteBuffer byteBuffer = new ByteBuffer(1024) ) {
                byte[] buffer = byteBuffer.Buffer;
                int len = 0;
                long sumLen = 0L;

                while( (len = responseStream.Read(buffer, 0, buffer.Length)) > 0 ) {
                    sumLen += len;
                    if( sumLen > maxLimitLen ) {
                        throw new ResponseBodyTooLargeException(maxLimitLen);
                    }
                    ms2.Write(buffer, 0, len);
                }
            }

            return ms2.ToArray();
        }
    }

    private Stream ReadResponseAsStream()
    {
        _autoCloseResponseStream = false;
        return _responseStream;
    }


    internal static T ReturnResultFromTextStream<T>(Stream responseStream, string contentType)
    {
        HttpUtils.ParseContentType(contentType, out string mediaType, out Encoding encoding);

        // 1, 优先选择直接用“流”做反序列化

        if( mediaType.Is(ResponseContentType.Json) && ReturnTypeIsObject<T>() ) {
            // json => object 比较常用，提前做特殊处理（直接读流做反序列化，不需要先生成responseText），可优化性能
            return ReturnObjectFromJsonStream<T>(responseStream, encoding);
        }

        if( mediaType.Is(ResponseContentType.Ndjson) && ReturnTypeIsList<T>() ) {
            // ndjson 用于大数据量返回，提前做特殊处理（直接读流做反序列化，不需要先生成responseText），可优化性能
            return ReturnListFromNdjsonStream<T>(responseStream, encoding);
        }

        if( mediaType.Is(ResponseContentType.Xml) && ReturnTypeIsObject<T>() ) {
            return ReturnObjectFromXmlStream<T>(responseStream, encoding);
        }

        // 2, 其它类型的结果，先得到字符串，再做判断处理
        string responseText = ReadResponseAsText(responseStream, mediaType, encoding);
        return ConvertResult<T>(responseText, mediaType, contentType);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T ReturnObjectFromJsonStream<T>(Stream responseStream, Encoding encoding)
    {
        JsonSerializerSettings settings = JsonSerializerSettingsUtils.Get(JsonStyle.None);
        JsonSerializer jsonSerializer = settings.CreateJsonSerializer();

        using StreamReader reader = new StreamReader(responseStream, (encoding ?? Encoding.UTF8), true, 1024, true);

        using( JsonTextReader reader2 = new JsonTextReader(reader) ) {
            return (T)jsonSerializer.Deserialize(reader, typeof(T));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static LIST ReturnListFromNdjsonStream<LIST>(Stream responseStream, Encoding encoding)
    {
        using StreamReader reader = new StreamReader(responseStream, (encoding ?? Encoding.UTF8), true, 1024, true);

        return (LIST)NdJsonExtensions.LoadListFromNdjson(reader, typeof(LIST));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ReturnTypeIsList<T>()
    {
        return typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ReturnTypeIsObject<T>()
    {
        // 此方法用于判断 “返回值T” 是不是用于“反序列化”的类型
        // 有些2B 站点/服务，会一直设置响应头 Content-Type: application/json，但是 response-body 就是 "普通字符串"，并不是JSON字符串
        // 所以，最终以“返回值T” 为准来判断要不要做反序列化

        return typeof(T).IsSuitableDeserialize();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T ReturnObjectFromXmlStream<T>(Stream responseStream, Encoding encoding)
    {
        XmlSerializer mySerializer = new XmlSerializer(typeof(T));

        using StreamReader reader = new StreamReader(responseStream, (encoding ?? Encoding.UTF8), true, 1024, true);

        return (T)mySerializer.Deserialize(reader);
    }


    internal static T ConvertResult<T>(string responseText, string mediaType, string contentType)
    {
        // 优先判断 “返回值类型” 可以起到【纠错】的作用
        // 有些2B 站点/服务，会一直设置响应头 Content-Type: application/json，但是 response-body 就是 "普通字符串"，并不是JSON字符串
        // 所以，最终以“返回值T” 为准来判断要不要做反序列化

        if( typeof(T) == typeof(string) )   // 忽略 contentType，永远按字符串返回
            return (T)(object)responseText;


        // 如果响应结果为空，就直接返回类型的默认值（NULL）
        if( string.IsNullOrEmpty(responseText) )
            return default(T);

        if( string.IsNullOrEmpty(mediaType) )    // 响应头没有指定 Content-Type 按 text/plain 处理
            return (T)StringConverter.ChangeType(responseText, typeof(T));


        if( mediaType.Is(ResponseContentType.Json) )
            return JsonExtensions.FromJson<T>(responseText);


        if( mediaType.Is(ResponseContentType.Xml) )
            return XmlHelper.XmlDeserialize<T>(responseText);


        if( mediaType.Is(ResponseContentType.Text) )
            return (T)StringConverter.ChangeType(responseText, typeof(T));

        throw new NotSupportedException($"不支持将 Content-Type: {contentType} 的响应流转成 {typeof(T).FullName} 类型！");
    }


    internal static string ReadResponseAsText(Stream responseStream, string mediaType, Encoding encoding, long maxLimitLen = 0)
    {
        // 共有 4 种场景
        // 1，contentType is null  , 按 UTF-8 方式读取
        // 2，contentType: xxxxx; charset=encoding  , 按 encoding 方式读取
        // 3，contentType: xxxxx  , 按 UTF-8 方式读取
        // 4，contentType: text/html ，先解析html，找到 charset，再重新按charset的编码方式读取，此时可能会需要读取2次，因此需要引入一个临时流
        // 说明：如果响应内容是文本，场景2是规范的，其它都是不规范的！


        if( mediaType.IsNullOrEmpty() )    // 场景 1
            return ReadText(responseStream, Encoding.UTF8, maxLimitLen);


        if( encoding != null )  // 场景 2
            return ReadText(responseStream, encoding, maxLimitLen);


        bool isHtml = mediaType.Is(ResponseContentType.Html);
        if( isHtml == false )     // 场景 3
            return ReadText(responseStream, Encoding.UTF8, maxLimitLen);
        else
            return ReadHtml(responseStream, Encoding.UTF8, out Encoding htmlEncoding);    // 场景 4, html
    }


    internal static string ReadText(Stream stream, Encoding encoding, long maxLimitLen = 0)
    {
        if( stream.CanSeek )
            stream.Position = 0;

        if( stream.CanRead == false )
            return string.Empty;

        if( maxLimitLen <= 0 ) {   // 不检查长度
            using( StreamReader reader = new StreamReader(stream, encoding, true, 1024, true) ) {
                return reader.ReadToEnd();
            }
        }

        // 读取流，并检查最大了限制长度
        StringBuilder sb = new StringBuilder();
        using( StreamReader reader = new StreamReader(stream, encoding, true, 1024, true) ) {
            string line = null;
            while( (line = reader.ReadLine()) != null ) {
                if( sb.Length + line.Length + 2 > maxLimitLen ) {  // 2 = 换行符 \r\n 长度
                    throw new ResponseBodyTooLargeException(maxLimitLen);
                }
                if( sb.Length > 0 )
                    sb.AppendLineRN();

                sb.Append(line);
            }
        }
        return sb.ToString();
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
            return ReadText(ms, htmlEncoding);  // html 不会非常大，所以不检查长度
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
            return EncodingUtils.GetEncoding(charset);
        }
        return null;
    }

}
