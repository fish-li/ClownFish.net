namespace ClownFish.WebClient;

/// <summary>
/// 表示一个HTTP的调用结果，包含响应头和响应内容
/// </summary>
/// <typeparam name="T">响应内容的类型参数</typeparam>
public sealed class HttpResult<T> : IToAllText
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int StatusCode { get; private set; }

    /// <summary>
    /// 从服务端返回响应头集合
    /// </summary>
    public NameValueCollection Headers { get; private set; }

    /// <summary>
    /// 响应体中的结果
    /// </summary>
    public T Result { get; private set; }

    /// <summary>
    /// 响应头中的 Content-Type
    /// </summary>
    public string ContentType {
        get => this.Headers[HttpHeaders.Response.ContentType];
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="headers"></param>
    /// <param name="result"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpResult(int statusCode, NameValueCollection headers, T result) 
    {
        if( headers == null )
            throw new ArgumentNullException(nameof(headers));

        this.StatusCode = statusCode;
        this.Headers = headers;
        this.Result = result;
    }

   
    /// <summary>
    /// 将一个对象的所有信息全部转成文本形式输出
    /// </summary>
    /// <returns></returns>
    public string ToAllText()
    {
        return ToAllText(true);
    }

    /// <summary>
    /// 将HttpResult&lt;string&gt;实例转成可读文本
    /// </summary>
    /// <param name="includeBody">是否包含请求体部分</param>
    /// <returns></returns>
    public string ToAllText(bool includeBody)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {

            string status = ((HttpStatusCode)this.StatusCode).ToString();
            sb.Append("HTTP/1.1 ").Append(this.StatusCode.ToString()).Append(' ').AppendLineRN(status);

            foreach( string name in this.Headers.Keys ) {
                string[] values = this.Headers.GetValues(name);
                foreach( string value in values )
                    sb.Append(name).Append(": ").AppendLineRN(value);
            }

            sb.AppendLineRN();

            if( includeBody ) {
                sb.Append(this.Result);
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

}
