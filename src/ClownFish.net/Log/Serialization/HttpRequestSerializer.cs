﻿#if NETCOREAPP
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
        sb.Append(request.Method).Append(' ')
                .Append(request.RequestUri.AbsoluteUri)
                .Append(" HTTP/").AppendLineRN(request.Version.ToString());

        if( request.Headers != null ) {
            foreach( var x in request.Headers ) {
                foreach( var v in x.Value ) {
                    sb.AppendLineRN($"{x.Key}: {v}");
                }
            }
        }

        if( request.Content != null && request.Content.Headers != null ) {
            foreach( var x in request.Content.Headers ) {
                foreach( var v in x.Value ) {
                    sb.AppendLineRN($"{x.Key}: {v}");
                }
            }
        }

        if( request.Content != null && request.RequestBodyCanLog() ) {

            // 大多数情况下，request.Content 的内容放在 MemoryStream 中，但是有可能在这个时候被 Dispose 了，所以再按非常规方式来读取
            string body = ReadBody(request.Content)
                          ?? TryReadBodyFromMemoryStream(request.Content);

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
    internal static bool RequestBodyCanLog(this HttpRequestMessage request)
    {
        if( LoggingOptions.HttpClient.LogClientRequestBody
            && request.Content != null
            && request.Content.Headers != null
            && request.HasBody()
            && request.Content.BodyIsText()
            && request.LoggingIgnoreBody() == false
            && request.Content.GetBodySize().IsBetween(1, LoggingLimit.HttpBodyMaxLen)
            ) {

            return true;
        }

        return false;
    }

    internal static string TryReadBodyFromMemoryStream(HttpContent content)
    {
        if( content == null )
            return null;

        StreamContent content2 = content as StreamContent;
        if( content2 == null )
            return null;

        FieldInfo filed1 = typeof(StreamContent).GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic);
        if( filed1 != null ) {
            MemoryStream ms = filed1.GetValue(content2) as MemoryStream;
            if( ms != null ) {
                try {
                    byte[] bytes = ms.ToArray();
                    return Encoding.UTF8.GetString(bytes);
                }
                catch {
                    // 不能读取就不读
                }
            }
        }
        return null;
    }

    internal static string ReadBody(HttpContent content)
    {
        try {
            return content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        catch {
            // 不能读取就算了~~~
        }
        return null;
    }

    internal static bool HasBody(this HttpRequestMessage request)
    {
        return HttpUtils.RequestHasBody(request.Method.ToString());
    }

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

    internal static long GetBodySize(this HttpContent content)
    {
        if( content.Headers.TryGetValues(HttpHeaders.Request.ContentLength, out IEnumerable<string> values) ) {
            string contentLength = values.FirstOrDefault();
            return contentLength.TryToLong();
        }
        else {
            return 0L;
        }
    }

    internal static bool LoggingIgnoreBody(this HttpRequestMessage request)
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
