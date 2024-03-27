#if NETCOREAPP
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace ClownFish.Log;
internal static class HttpRequestSerializer
{
    public static string ToLoggingText(this HttpRequestMessage request)
    {
        if( request == null )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            ToLoggingText(request, sb );
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    public static void ToLoggingText(this HttpRequestMessage request, StringBuilder sb)
    {
        // HttpRequestMessage 写到日志有3个范围：
        // 1，记录 “请求行”， “请求头”，“请求体”    MustLogRequest == true && LogRequestBody == true
        // 2，记录 “请求行”， “请求头”             MustLogRequest == true && LogRequestBody == false
        // 3，记录 “请求行”                       MustLogRequest == false 

        sb.Append(request.Method).Append(' ')
                .Append(request.RequestUri.AbsoluteUri)
                .Append(" HTTP/").AppendLineRN(request.Version.ToString());

        if( LoggingOptions.HttpClient.MustLogRequest == false )
            return;

        // 说明：request.Headers, response.Headers, content.Headers 永远不为null，所以代码中不做判断。

        foreach( var x in request.Headers ) {
            foreach( var v in x.Value ) {
                sb.AppendLineRN($"{x.Key}: {v}");
            }
        }

        if( request.Content == null )
            return;

        foreach( var x in request.Content.Headers ) {
            foreach( var v in x.Value ) {
                sb.AppendLineRN($"{x.Key}: {v}");
            }
        }

        //sb.Append("## request.Content: ").AppendLineRN(request.Content.GetType().FullName);

        if( request.CanLogBody() ) {

            string body = request.Content.ReadBodyAsText();
            if( body != null ) {
                sb.AppendLineRN().AppendLineRN(body).AppendLineRN();
            }
        }
    }


    /// <summary>
    /// 判断一个请求，它的body是否符合 “日志记录”  的要求： 1，有 body，2，body是文本格式，3，body大小在限定范围内
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static bool CanLogBody(this HttpRequestMessage request)
    {
        if( LoggingOptions.HttpClient.LogRequestBody == false )
            return false;

        if( LoggingOptions.RequestBodyBufferSize <= 0)
            return false;

        if( request.Content == null )
            return false;

        if( request.HasBody() == false )
            return false;

        if( request.Content.Headers.ContentEncoding.Count > 0 )   // Content-Encoding: gzip
            return false;

        if( request.Content.BodyIsText() == false )
            return false;

        if( request.IsIgnoreBody() )
            return false;

        long size = request.Content.GetBodySize();
        if( size.IsBetween(1, LoggingOptions.RequestBodyBufferSize) == false )
            return false;

        return true;
    }

    


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool HasBody(this HttpRequestMessage request)
    {
        return HttpUtils.RequestHasBody(request.Method.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool BodyIsText(this HttpContent content)
    {
        if( content.Headers.TryGetValues(HttpHeaders.Request.ContentType, out IEnumerable<string> values) ) {
            string contentType = values.FirstOrDefault();
            return HttpUtils.RequestBodyIsText(contentType);
        }
        else {
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long GetBodySize(this HttpContent content)
    {
        if( content.Headers.TryGetValues(HttpHeaders.Request.ContentLength, out IEnumerable<string> values) ) {
            string contentLength = values.FirstOrDefault();
            return contentLength.TryToLong();
        }
        else {
            // 有些情况就是不指定 ContentLength ，例如： Transfer-Encoding: chunked
            return -1L;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsIgnoreBody(this HttpRequestMessage request)
    {
        return request.GetRequestOption<string>(LoggingIgnoreNames.IgnoreRequestBody) == "1";
    }


    internal static T GetRequestOption<T>(this HttpRequestMessage request, string name)
    {
        if( request == null )
            return default(T);

        // 下面这个 Options 属性访问会导致创建一个 HttpRequestOptions 对象，其实是个很SB的设计，
        // MS应该提供一个 TryGet 之类的设计的，免得在读取时白白创建一个对象。

        IDictionary<string, object> dict = request.Options;

        if( dict.TryGetValue(name, out object value) && value is T val )
            return val;
        else
            return default(T);
    }
}

#endif
