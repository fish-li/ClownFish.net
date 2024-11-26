using ClownFish.Http.Proxy;

namespace ClownFish.Http.Utils;

public static partial class ResponseUtils
{
    /// <summary>
    /// 将HttpWebResponse转换成HttpResult&lt;string&gt;实例
    /// </summary>
    /// <param name="httpWebResponse"></param>
    /// <returns></returns>
    public static HttpResult<string> GetResult(this HttpWebResponse httpWebResponse)
    {
        if( httpWebResponse == null )
            return null;

        ResponseReader reader = new ResponseReader(httpWebResponse);
        return reader.Read<HttpResult<string>>();
    }


    internal static int SetResponseHeaders(this NHttpResponse httpResponse, NameValueCollection headers)
    {
        int count = 0;

        if( headers == null || headers.Count == 0 )
            return count;


        string contentType = headers[HttpHeaders.Response.ContentType];
        if( contentType.IsNullOrEmpty() == false ) {
            httpResponse.ContentType = contentType;
            count++;
        }

        // 复制响应头
        foreach( string name in headers.AllKeys ) {
            if( HttpProxyModule.IgnoreResponseHeaders.Contains(name) )
                continue;

            string[] values = headers.GetValues(name);
            httpResponse.SetResponseHeaders(name, values);
            count++;
        }

        return count;
    }


    internal static int SetResponseHeaders(this NHttpResponse httpResponse, string name, string[] values)
    {
        if( values == null || values.Length == 0 )
            return 0;

        try {
            httpResponse.SetHeaders(name, values, true);
            return 1;
        }
        catch( Exception ex ) {
            Console2.Info($"SetResponseHeaders({name}) ERROR: " + ex.Message);

            // 防止出现不允许设置的请求头，未来可以增加日志记录
            return -1;
        }
    }


    internal static int SetResponseHeader(this NHttpResponse httpResponse, string name, string value)
    {
        if( string.IsNullOrEmpty(value) )
            return 0;

        try {
            httpResponse.SetHeader(name, value, true);
            return 1;
        }
        catch( Exception ex ) {
            Console2.Info($"SetResponseHeader({name}) ERROR: " + ex.Message);

            // 防止出现不允许设置的请求头，未来可以增加日志记录
            return -1;
        }
    }
}
