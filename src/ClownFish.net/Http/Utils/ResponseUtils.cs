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


    internal static int CopyResponseHeaders(NameValueCollection headers, NHttpResponse httpResponse, HashSet<string> ignoreResponseHeaders = null)
    {
        int count = 0;

        if( headers == null || headers.Count == 0 )
            return count;

        if( ignoreResponseHeaders == null )
            ignoreResponseHeaders = HttpProxyModule.IgnoreResponseHeaders;

        string contentType = headers[HttpHeaders.Response.ContentType];
        if( contentType.IsNullOrEmpty() == false ) {
            httpResponse.ContentType = contentType;
            count++;
        }


        // 复制响应头
        foreach( string name in headers.AllKeys ) {
            if( ignoreResponseHeaders.Contains(name) )
                continue;


            string[] values = headers.GetValues(name);
            SetResponseHeader(httpResponse, name, values);
            count++;
        }
        return count;
    }


    internal static int SetResponseHeader(NHttpResponse httpResponse, string name, string[] values)
    {
        if( values == null || values.Length == 0 )
            return 0;

        try {
            httpResponse.SetHeaders(name, values, true);
            return 1;
        }
        catch( Exception ex ) {
            Console2.Info($"SetResponseHeader({name}) ERROR: " + ex.Message);

            // 防止出现不允许设置的请求头，未来可以增加日志记录
            return -1;
        }
    }
}
