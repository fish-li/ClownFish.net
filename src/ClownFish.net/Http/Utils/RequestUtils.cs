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


    /// <summary>
    /// 将一个 key/value 存储到 request.Options or request.Properties
    /// </summary>
    /// <param name="request"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static void SetOptionValue(this HttpRequestMessage request, string name, object value)
    {
        if( request == null )
            throw new ArgumentNullException(nameof(request));
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(request));

#if NET6_0_OR_GREATER
        IDictionary<string, object> dict = request.Options;
#else
        IDictionary<string, object> dict = request.Properties;
#endif
        dict[name] = value;
    }


    /// <summary>
    /// 从 request.Options or request.Properties 读取指定的键值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T GetOptionValue<T>(this HttpRequestMessage request, string name)
    {
        if( request == null )
            return default(T);

        IDictionary<string, object> dict = null;

#if NET6_0_OR_GREATER
        // 下面这个 Options 属性访问会导致创建一个 HttpRequestOptions 对象，其实是个很SB的设计，
        // MS应该提供一个 TryGet 之类的设计的，免得在读取时白白创建一个对象。

        FieldInfo field = typeof(HttpRequestMessage).GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
        if( field != null ) {
            dict = (HttpRequestOptions)field.FastGetValue(request);
        }
        else {
            dict = request.Options;
        }
#else
        dict = request.Properties;
#endif

        if( dict == null )
            return default(T);

        if( dict.TryGetValue(name, out object value) && value is T val )
            return val;
        else
            return default(T);
    }

}

#endif
