#if NETCOREAPP
using System.Net.Http;


namespace ClownFish.Http.Utils;
/// <summary>
/// Response相关扩展工具类
/// </summary>
public static partial class ResponseUtils
{
    private static readonly object s_lock = new object();
    private static bool s_inited = false;

    private static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    Init0();
                    s_inited = true;
                }
            }
        }

    }

    [UnconditionalSuppressMessage("Trimming", "IL2026: DynamicMethodFactory")]
    [DynamicDependency("_httpResponseMessage", typeof(HttpWebResponse))]
    private static void Init0()
    {
        s_field = typeof(HttpWebResponse).GetField("_httpResponseMessage", BindingFlags.Instance | BindingFlags.NonPublic);
        if( s_field == null )
            throw new NotSupportedException("Field HttpWebResponse._httpResponseMessage not found!");

        if( EnvArgs0.IsAot == false ) {
            s_httpResponseMessageGetter = DynamicMethodFactory.CreateFieldGetter(s_field);
        }
    }

    private static FieldInfo s_field;
    private static GetValueDelegate s_httpResponseMessageGetter;

    /// <summary>
    /// 获取HttpWebResponse对象中的原始HttpResponseMessage对象
    /// </summary>
    /// <param name="httpWebResponse"></param>
    /// <returns></returns>
    public static HttpResponseMessage ToResponseMessage(this HttpWebResponse httpWebResponse)
    {
        if( httpWebResponse == null )
            throw new ArgumentNullException(nameof(httpWebResponse));

        Init();

        HttpResponseMessage responseMessage = (s_httpResponseMessageGetter != null)
           ? (HttpResponseMessage)s_httpResponseMessageGetter.Invoke(httpWebResponse)
           : (HttpResponseMessage)s_field.GetValue(httpWebResponse);

        if( responseMessage == null )
            throw new ObjectDisposedException(nameof(HttpWebResponse));

        return responseMessage;
    }


    /// <summary>
    /// 获取HttpWebResponse实例中的所有响应头。注意此操作将会读取所有响应头并生成一个新的NameValueCollection对象。
    /// </summary>
    /// <param name="httpWebResponse"></param>
    /// <returns></returns>
    public static NameValueCollection GetAllHeaders(this HttpWebResponse httpWebResponse)
    {
        if( httpWebResponse == null )
            throw new ArgumentNullException(nameof(httpWebResponse));

        // 注意：
        // 虽然HttpWebResponse提供了Headers属性，但是它的实现有 BUG ！
        // 当有2个同名的响应头时，它们的值会合并在一起，原因在于里面调用了 GetHeaderValueAsString(header.Value) 这个方法，它会做合并操作
        // 所以，没办法了，只能自己重新实现。

        HttpResponseMessage message = httpWebResponse.ToResponseMessage();
        return message.CloneAllHeaders();
    }



    /// <summary>
    /// 将HttpResponseMessage中的所有响应头合并在一起。注意此操作将会读取所有响应头并生成一个新的NameValueCollection对象。
    /// 说明：HttpResponseMessage中的响应头分散在二处，很SB的设计，使用起来非常别扭。
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <returns></returns>
    public static NameValueCollection CloneAllHeaders(this HttpResponseMessage responseMessage)
    {
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));

        NameValueCollection headers = new NameValueCollection();

        // 注意：这里有个问题，可搜索："Docker/24.0.6 (linux)"，有几个测试用例

        foreach( KeyValuePair<string, IEnumerable<string>> kv in responseMessage.Headers ) {

            // ##### 如里修改了这里，要 同步 调整 HttpResponseSerializer.ToLoggingText 方法

            if( kv.Key.Is("Server") ) {
                headers.Add(kv.Key, responseMessage.Headers.Server.ToString());
            }
            else if( kv.Key.Is("Vary") ) {
                headers.Add(kv.Key, responseMessage.Headers.Vary.ToString());
            }
            else {
                foreach( string value in kv.Value ) {
                    headers.Add(kv.Key, value);
                }
            }
        }
        if( responseMessage.Content != null ) {
            foreach( KeyValuePair<string, IEnumerable<string>> kv2 in responseMessage.Content.Headers ) {
                foreach( string value in kv2.Value ) {
                    headers.Add(kv2.Key, value);
                }
            }
        }

        return headers;
    }

    /// <summary>
    /// 获取响应头 Content-Type 的内容
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetContentType(this HttpResponseMessage responseMessage)
    {
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));

        if( responseMessage.Content != null && responseMessage.Content.Headers.TryGetValues(HttpHeaders.Response.ContentType, out var values) ) {
            return values.FirstOrDefault();
        }

        return null;
    }

    ///// <summary>
    ///// 读取指定的响应头
    ///// </summary>
    ///// <param name="responseMessage"></param>
    ///// <param name="name"></param>
    ///// <returns></returns>
    //public static string GetHeader(this HttpResponseMessage responseMessage, string name)
    //{
    //    if( responseMessage == null )
    //        throw new ArgumentNullException(nameof(responseMessage));
    //    if( name.IsNullOrEmpty() )
    //        throw new ArgumentNullException(nameof(name));

    //    if( responseMessage.Headers.TryGetValues(name, out var values) ) {
    //        return string.Join(", ", values);    // 有些头并不是用 逗号 来拼接（用空格），所以这个方法不再使用！
    //    }

    //    if( responseMessage.Content != null ) {
    //        if( responseMessage.Content.Headers.TryGetValues(name, out var values2) ) {
    //            return string.Join(", ", values2);
    //        }
    //    }

    //    return null;
    //}

    /// <summary>
    /// 读取指定的响应头
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="name">响应头名称</param>
    /// <returns></returns>
    internal static IEnumerable<string> GetHeaders(this HttpResponseMessage responseMessage, string name)
    {
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        foreach( KeyValuePair<string, IEnumerable<string>> kv in responseMessage.Headers ) {
            if( kv.Key.Is(name) )
                return kv.Value;
        }

        if( responseMessage.Content != null ) {
            foreach( KeyValuePair<string, IEnumerable<string>> kv2 in responseMessage.Content.Headers ) {
                if( kv2.Key.Is(name) )
                    return kv2.Value;
            }
        }
        return null;
    }



    /// <summary>
    /// 读取HttpResponseMessage对象的响应体，以文本方式返回
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static async Task<string> ReadBodyAsTextAsync(this HttpResponseMessage responseMessage, Encoding encoding = null)
    {
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));

        string contentEncoding = responseMessage.Content.Headers.ContentEncoding.FirstOrDefault();
        using( Stream responseStream = await responseMessage.Content.ReadAsStreamAsync() ) {

            HttpStreamReader reader = new HttpStreamReader(responseStream, contentEncoding);
            return await reader.ReadAllTextAsync(encoding);
        }
    }


#if NET5_0_OR_GREATER

    /// <summary>
    /// 读取HttpResponseMessage对象的响应体，以文本方式返回
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string ReadBodyAsText(this HttpResponseMessage responseMessage, Encoding encoding = null)
    {
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));

        string contentEncoding = responseMessage.Content.Headers.ContentEncoding.FirstOrDefault();
        using( Stream responseStream = responseMessage.Content.ReadAsStream() ) {   // ReadAsStream 这个方法是 net5 中引入的

            HttpStreamReader reader = new HttpStreamReader(responseStream, contentEncoding);
            return reader.ReadAllText(encoding);
        }
    }

#endif


}

#endif