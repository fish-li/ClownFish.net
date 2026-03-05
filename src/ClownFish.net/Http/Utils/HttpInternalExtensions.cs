namespace ClownFish.Http.Utils;

/// <summary>
/// 框架内部使用的工具类
/// </summary>
public static class HttpInternalExtensions
{

#pragma warning disable IDE1006 // 命名样式
    private static readonly MethodInfo s_AddMethod;
    private static readonly ActionWrapper<WebHeaderCollection, string, string> s_AddMethodWrapper;
#pragma warning restore IDE1006 // 命名样式

    static HttpInternalExtensions()
    {
        // 使用这个内部方法写HTTP头会比较方便，
        // 因为有些头不允许直接添加，需要通过属性来设置，那样就需要一大堆的判断，写起来很麻烦。
        MethodInfo method = typeof(WebHeaderCollection).GetMethod(
            "AddWithoutValidate",
            BindingFlags.Instance | BindingFlags.NonPublic, null,
            new Type[] { typeof(string), typeof(string) }, null);

        s_AddMethod = method;

        if( EnvArgs0.IsAot == false ) {
            s_AddMethodWrapper = new ActionWrapper<WebHeaderCollection, string, string>();
            s_AddMethodWrapper.BindMethod(method);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static void InternalAdd(this WebHeaderCollection headers, string name, string value)
    {
        try {
            if( s_AddMethodWrapper != null ) {
                s_AddMethodWrapper.Call(headers, name, value);
            }
            else {
                s_AddMethod.Invoke(headers, new object[] { name, value });
            }
        }
        catch( Exception ex ) {
            throw new ApplicationException($"添加请求头失败：[{name}={value}]", ex);
        }
    }
}
