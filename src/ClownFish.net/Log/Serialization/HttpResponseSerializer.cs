#if NETCOREAPP
using System.Net.Http;

namespace ClownFish.Log;
internal static class HttpResponseSerializer
{
    internal static string ToLoggingText(this HttpResponseMessage response)
    {
        if( response == null )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            ToLoggingText(response, response.Content, true, sb);
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    public static void ToLoggingText(this HttpResponseMessage response, HttpContent content, bool checkBody, StringBuilder sb)
    {
        // HttpResponseMessage 写到日志有3个范围：
        // 1，记录 “响应行”， “响应头”，“响应体”    MustLogResponse == true && LogResponseBody == true
        // 2，记录 “响应行”， “响应头”             MustLogResponse == true && LogResponseBody == false
        // 3，记录 “响应行”                       MustLogResponse == false 

        int statusCode = (int)response.StatusCode;
        string statusMessage = HttpUtils.GetStatusReasonPhrase(statusCode);
        sb.Append("HTTP/1.1 ").Append(statusCode).Append(' ').Append(statusMessage).AppendLineRN();

        if( LoggingOptions.HttpClient.MustLogResponse == false )
            return;

        foreach( var x in response.Headers ) {

            // ##### 如里修改了这里，要 同步 调整 ResponseUtils6.CloneAllHeaders 方法

            if( x.Key.Is("Server") ) {
                sb.AppendLineRN($"{x.Key}: {response.Headers.Server.ToString()}");
            }
            else if( x.Key.Is("Vary") ) {
                sb.AppendLineRN($"{x.Key}: {response.Headers.Vary.ToString()}");
            }
            else {
                foreach( var v in x.Value ) {
                    sb.AppendLineRN($"{x.Key}: {v}");
                }
            }
        }

        // 1，content = null，表示 response-body 不支持日志，但是 header 是要记录到日志的（此时有下面3种可能）
        // 2，content != null，表示 response-body 支持日志

        // response.Content 有几种可能：
        // 1, System.Net.Http.EmptyContent
        // 2, System.Net.Http.DecompressionHandler+DecompressedContent
        // 3, [normal-internal-type]               

        // 当服务端返回 压缩数据 时，System.Net.Http.DecompressionHandler 会修改 response.Content
        // 并且比较坑的是 DecompressedContent 的派生类会删除2个头：Content-Length,Content-Encoding
        // 考虑到这里 response._disposed = true，所以就把原始的 response.Content 找出来（只读取响应头）

        if( content == null ) {
            TryGetRealContent(response.Content).LogContentHeaders(sb);
            return;
        }

        TryGetRealContent(content).LogContentHeaders(sb);

        //sb.Append("## response.Content: ").AppendLineRN(response.Content.GetType().FullName);

        if( checkBody && response.CanLogBody(content) == false )
            return;

        // 有些情况下可能读不到数据~~~~~~~~~~
        string body = content.ReadBodyAsText();
        if( body != null ) {
            sb.AppendLineRN().AppendLineRN(body).AppendLineRN();
        }
    }


    [UnconditionalSuppressMessage("Trimming", "IL2026: Assembly.GetType")]
    private static readonly Type s_type = typeof(HttpContent).Assembly.GetType("System.Net.Http.DecompressionHandler+DecompressedContent", false, false);


    [UnconditionalSuppressMessage("Trimming", "IL2080: s_type.GetField")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static HttpContent TryGetRealContent(HttpContent content)
    {
        if( content != null && s_type != null && content.GetType().IsSubclassOf(s_type) ) {
            FieldInfo field = s_type.GetField("_originalContent", BindingFlags.Instance | BindingFlags.NonPublic);
            if( field != null ) {
                return (HttpContent)field.GetValue(content);
            }
        }
        return content;
    }


    internal static bool CanLogBody(this HttpResponseMessage response, HttpContent content)
    {
        if( LoggingOptions.HttpClient.LogResponseBody == false )
            return false;

        if( LoggingLimit.HttpClient.MaxBodySize <= 0 )
            return false;

        if( content == null )
            return false;

        if( content.Headers.ContentEncoding.Count > 0 )   // Content-Encoding: gzip
            return false;

        if( response.Headers.TransferEncoding.Count > 0 )          // Transfer-Encoding: chunked
            return false;

        if( content.BodyIsText() == false )
            return false;

        if( response.IsIgnoreBody() )
            return false;

        long size = content.GetBodySize();
        if( size.IsBetween(1, LoggingLimit.HttpClient.MaxBodySize) == false )
            return false;

        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsIgnoreBody(this HttpResponseMessage response)
    {
        return response.RequestMessage?.GetOptionValue<string>(LoggingKeys.IgnoreResponseBody) == "1";
    }

}

#endif
