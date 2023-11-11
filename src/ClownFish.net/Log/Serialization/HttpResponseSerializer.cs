#if NETCOREAPP
using System.Net.Http;

namespace ClownFish.Log;
internal static class HttpResponseSerializer
{
    public static string ToLoggingText(this HttpResponseMessage response)
    {
        if( response == null )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            ToLoggingText(response, sb);
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    public static void ToLoggingText(this HttpResponseMessage response, StringBuilder sb)
    {
        int statusCode = (int)response.StatusCode;
        string statusMessage = HttpUtils.GetStatusReasonPhrase(statusCode);
        sb.Append("HTTP/1.1 ").Append(statusCode).Append(' ').Append(statusMessage).AppendLineRN();

        if( response.Headers != null ) {
            foreach( var x in response.Headers ) {
                foreach( var v in x.Value ) {
                    sb.AppendLineRN($"{x.Key}: {v}");
                }
            }
        }

        if( response.Content != null && response.Content.Headers != null ) {
            foreach( var x in response.Content.Headers ) {
                foreach( var v in x.Value ) {
                    sb.AppendLineRN($"{x.Key}: {v}");
                }
            }
        }

        if( response.Content != null && response.ResponseBodyCanLog() ) {

            // 有些情况下可能读不到数据~~~~~~~~~~
            string body = HttpRequestSerializer.ReadBody(response.Content);
            if( body != null ) {
                sb.AppendLineRN().AppendLineRN(body).AppendLineRN();
            }
        }
    }


    internal static bool ResponseBodyCanLog(this HttpResponseMessage response)
    {
        if( LoggingOptions.HttpClient.LogClientResponseBody
            && response.Content != null
            && response.Content.Headers != null
            && response.Content.BodyIsText()
            && response.LoggingIgnoreBody() == false
            && response.Content.GetBodySize().IsBetween(1, LoggingLimit.HttpBodyMaxLen)
            ) {

            return true;
        }

        return false;
    }


    internal static bool LoggingIgnoreBody(this HttpResponseMessage response)
    {
        HttpRequestMessage request = response.RequestMessage;

        if( request != null && request.Headers != null 
            && request.Headers.TryGetValues(LoggingIgnoreNames.HeaderName, out IEnumerable<string> values) ) {
            return values.Contains(LoggingIgnoreNames.IgnoreResponseBody);
        }
        else {
            return false;
        }
    }

}

#endif
