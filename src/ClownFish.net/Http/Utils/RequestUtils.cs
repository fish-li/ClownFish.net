#if NETCOREAPP
using System.Net.Http;

namespace ClownFish.Http.Utils;

/// <summary>
/// 
/// </summary>
public static class RequestUtils
{
    /// <summary>
    /// 从HttpRequestMessage中读取一个请求头。
    /// </summary>
    /// <param name="request"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetHeader(this HttpRequestMessage request, string name)
    {
        if( request.Headers.TryGetValues(name, out var values) ) {
            return string.Join(", ", values);
        }

        if( request.Content != null && request.Content.Headers != null ) {
            if( request.Content.Headers.TryGetValues(name, out var values2) ) {
                return string.Join(", ", values2);
            }
        }

        return null;
    }
}

#endif
