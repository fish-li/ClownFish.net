namespace ClownFish.Http.Pipleline;

/// <summary>
/// 定义一些与HttpRequest相关的扩展方法
/// </summary>
public partial class NHttpRequest : ILoggingObject
{
    private string _bodyText;

    /// <summary>
    /// 获取请求体字符串。
    /// 此方法会缓存结果，支持多次读取。
    /// </summary>
    /// <returns></returns>
    public string GetBodyText()
    {
        if( _bodyText == null ) {
            _bodyText = this.ReadBodyAsText();
        }
        return _bodyText;
    }


    /// <summary>
    /// 获取请求体字符串。
    /// 此方法会缓存结果，支持多次读取。
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetBodyTextAsync()
    {
        if( _bodyText == null ) {
            _bodyText = await this.ReadBodyAsTextAsync();
        }
        return _bodyText;
    }


    // 说明：下面几个 ReadBodyAsXxxx 方法都把异常吃掉了，因为：
    // 这里出现异常意味着请求流不能读，很不正常的事情，只能吃掉
    // 典型异常：
    // 1, Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException: Unexpected end of request content.
    //    https://github.com/dotnet/aspnetcore/issues/26278
    //    https://github.com/dotnet/aspnetcore/issues/23949
    //    这个异常基本上无解，有可能是客户端的问题，也有可能是网络问题，，还有可能是服务端的线程调度问题，例如经常会伴随出现如下警告
    //       warn: Microsoft.AspNetCore.Server.Kestrel[22]
    //             As of "07/27/2023 03:47:51 +00:00", the heartbeat has been running for "00:00:01.0234029" which is longer than "00:00:01". This could be caused by thread pool starvation.
    // 2, Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException: Request body too large. The max request body size is xxxxxxxxxx bytes.


    /// <summary>
    /// 按 字符串 形式读取请求体内容。
    /// 【##### 此方法不做结果缓存，因此不要多次调用 #####】
    /// 如果请求体数据是压缩格式，在读取时会自动解压缩，
    /// 如果请求体数据 不是 文本格式，会得到乱码字符串的结果！ 
    /// 可以事先调用 HttpUtils.RequestBodyIsText(request.ContentType) 来判断请求体是否为文本格式。
    /// </summary>
    /// <returns>输入的流</returns>
    public virtual string ReadBodyAsText()
    {
        if( this.HasBody == false || this.InputStream == null || this.InputStream.CanRead == false )
            return string.Empty;

        Encoding encoding = this.GetEncoding();
        string contentEncoding = this.Header(HttpHeaders.Request.ContentEncoding);

        try {
            HttpStreamReader reader = new HttpStreamReader(this.InputStream, contentEncoding);
            return reader.ReadAllText(encoding);
        }
        catch( Exception ex ) {
            Console2.Warnning("ReadBodyAsText ERROR: " + ex.ToString());
            // 吃异常的原因请参考上面注释
            return string.Empty;
        }
    }


    /// <summary>
    /// 按 字符串 形式读取请求体内容。
    /// 【##### 此方法不做结果缓存，因此不要多次调用 #####】
    /// 如果请求体数据是压缩格式，在读取时会自动解压缩，
    /// 如果请求体数据 不是 文本格式，会得到乱码字符串的结果！ 
    /// 可以事先调用 HttpUtils.RequestBodyIsText(request.ContentType) 来判断请求体是否为文本格式。
    /// </summary>
    /// <returns>输入的流</returns>
    public virtual async Task<string> ReadBodyAsTextAsync()
    {
        if( this.HasBody == false || this.InputStream == null || this.InputStream.CanRead == false )
            return string.Empty;

        Encoding encoding = this.GetEncoding();
        string contentEncoding = this.Header(HttpHeaders.Request.ContentEncoding);

        try {
            HttpStreamReader reader = new HttpStreamReader(this.InputStream, contentEncoding);
            return await reader.ReadAllTextAsync(encoding);
        }
        catch( Exception ex ) {
            Console2.Warnning(ex);
            // 吃异常的原因请参考上面注释
            return string.Empty;
        }
    }


    /// <summary>
    /// 按 二进制 形式读取请求体内容（不判断是否压缩格式）。
    /// 【##### 此方法不做结果缓存，因此不要多次调用 #####】
    /// </summary>
    /// <returns></returns>
    public virtual byte[] ReadBodyAsBytes()
    {
        if( this.HasBody == false || this.InputStream == null || this.InputStream.CanRead == false )
            return Empty.Array<byte>();

        try {
            return this.InputStream.ToArray();
        }
        catch( Exception ex ) {
            Console2.Warnning(ex);
            // 吃异常的原因请参考上面注释
            return Empty.Array<byte>();
        }
    }


    /// <summary>
    /// 按 二进制 形式读取请求体内容（不判断是否压缩格式）。
    /// 【##### 此方法不做结果缓存，因此不要多次调用 #####】
    /// </summary>
    /// <returns></returns>
    public virtual async Task<byte[]> ReadBodyAsBytesAsync()
    {
        if( this.HasBody == false || this.InputStream == null || this.InputStream.CanRead == false )
            return Empty.Array<byte>();

        try {
            return await this.InputStream.ToArrayAsync();
        }
        catch( Exception ex ) {
            Console2.Warnning(ex);
            // 吃异常的原因请参考上面注释
            return Empty.Array<byte>();
        }
    }


    /// <summary>
    /// 在记录请求日志时，是否记录请求体内容。
    /// 说明：在ASP.NETCORE中，记录请体还需要将开启请求缓冲。
    /// </summary>
    public bool LogRequestBody { get; set; } = LoggingOptions.Http.LogRequestBody;


    /// <summary>
    /// 获取请求对应的日志文本
    /// </summary>
    /// <returns></returns>
    public string ToLoggingText()
    {
        return this.ToRawText(this.LogRequestBody);
    }


    /// <summary>
    /// 将请求转换成符合HTTP协议描述的文本格式
    /// </summary>
    /// <returns></returns>
    internal string ToRawText(bool includeRequestBody)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append(this.HttpMethod)
            .Append(' ')
            .Append(this.FullUrl)
            .AppendLineRN(" HTTP/1.1");

            AccessHeaders((k, v) => sb.Append(k).Append(": ").Append(v).AppendLineRN());

            if( includeRequestBody ) {

                sb.AppendLineRN();  // 分隔行

                try {
                    string postData = this.GetBodyText();
                    //string postData = ReadBodyWithSafeFilter();
                    sb.Append(postData);
                }
                catch( Exception ex ) {
                    sb.Append("### ResponseBody 不能读取，原因：" + ex.ToString());
                    // 有可能就是没法读取，
                    // 例如：System.ObjectDisposedException: Cannot access a disposed object.
                    // Object name: 'FileBufferingReadStream'.
                    // at Microsoft.AspNetCore.WebUtilities.FileBufferingReadStream.ThrowIfDisposed()
                }
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    //private string ReadBodyWithSafeFilter()
    //{
    //    // 如果当前请求不包含请求体，就直接返回
    //    if( this.HasBody == false || HttpUtils.ContentIsText(this.ContentType) == false )
    //        return null;


    //    // 注意：详细的日志内容中可能会导致密码泄露，
    //    // 例如，一个登录请求在执行时发生异常，如果把请求体的全部内容记录到异常日志，后台人员就可以看到用户的密码。
    //    // 所以这里在记录请求体时，会检查一些特殊的标记，如果它们存在，就不再记录请求体

    //    // 注意：为了安全，密码不应该放在URL中。

    //    if( this.HttpContext?.PipelineContext?.IsLoginAction ?? false )
    //        return "## 当前请求包含敏感信息，已忽略请求体内容。";

    //    return this.GetBodyText();
    //}





}
