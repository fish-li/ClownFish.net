#if NETFRAMEWORK

namespace ClownFish.Base;

/// <summary>
/// Response相关扩展工具类
/// </summary>
public static partial class ResponseUtils
{
    /// <summary>
    /// 获取HttpWebResponse实例中的所有响应头。
    /// 本方法仅用于 .net framework 版本。
    /// </summary>
    /// <param name="httpWebResponse"></param>
    /// <returns></returns>
    public static NameValueCollection GetAllHeaders(this HttpWebResponse httpWebResponse)
    {
        if( httpWebResponse == null )
            throw new ArgumentNullException(nameof(httpWebResponse));

        // 注意：在 .net4.5以及之前 HttpWebResponse 有个BUG，有些响应头是允许重复指定的，但是通过 HttpWebResponse.Headers 读取时，结果会合并这些响应头
        //      虽然 Headers 提供了 GetValues(name) 方法，但它却会解析里面的值，结果导致原本是单一的标头会折成二行，结果仍然是错误的，
        //      例如，从服务端删除Cookie时，响应头： Set-Cookie: mvc-user=; expires=Mon, 11-Oct-1999 16:00:00 GMT; path=/; HttpOnly
        //      由于中间有个逗号，调用 GetValues("Set-Cookie") 会返回二行的数组：
        //      [0]: "mvc-user=; expires=Mon"
        //      [1]: "11-Oct-1999 16:00:00 GMT; path=/; HttpOnly"

        //  然而，webResponse.Headers 内部的 InnerCollection 属性的保存结果是正确的，
        //       调用它的GetValues(name) 方法，在这种情况下得到的结果是正确的
        //  所以，这里就直接用反射的方式拿到 InnerCollection ，用它获取所需的结果，避开微软错误的实现方式。

        //PropertyInfo propInfo = httpWebResponse.Headers.GetType().GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        //return (NameValueCollection)propInfo.FastGetValue(httpWebResponse.Headers);

        return httpWebResponse.Headers;
    }



}
#endif