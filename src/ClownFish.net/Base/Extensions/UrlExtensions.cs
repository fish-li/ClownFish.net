namespace ClownFish.Base;

/// <summary>
/// 提供一些与URL有关的扩展方法
/// </summary>
public static class UrlExtensions
{

    /// <summary>
    /// 等同于：System.Web.HttpUtility.UrlEncode(text)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string UrlEncode(this string text)
    {
        if( string.IsNullOrEmpty(text) )
            return text;

        return System.Web.HttpUtility.UrlEncode(text);
    }


    /// <summary>
    /// 等同于：System.Web.HttpUtility.HtmlDecode(text)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string HtmlDecode(this string text)
    {
        if( string.IsNullOrEmpty(text) )
            return text;

        return System.Web.HttpUtility.HtmlDecode(text);
    }


    /// <summary>
    /// 等同于：System.Web.HttpUtility.HtmlEncode(text)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string HtmlEncode(this string text)
    {
        if( string.IsNullOrEmpty(text) )
            return text;

        return System.Web.HttpUtility.HtmlEncode(text);
    }


    /// <summary>
    /// 等同于：System.Web.HttpUtility.UrlDecode(text)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string UrlDecode(this string text)
    {
        if( string.IsNullOrEmpty(text) )
            return text;

        return System.Web.HttpUtility.UrlDecode(text);
    }


    /// <summary>
    /// 给URL增加查询参数
    /// </summary>
    /// <param name="url">一个有效的URL，例如：http://www.abc.com/aa/bb/cc.aspx</param>
    /// <param name="name">参数名，例如：name</param>
    /// <param name="value">参数值，例如：中文汉字无需要编码</param>
    /// <returns>返回一个完整的URL，例如：http://www.abc.com/aa/bb/cc.aspx?name=%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97%e6%97%a0%e9%9c%80%e8%a6%81%e7%bc%96%e7%a0%81</returns>
    public static string AddUrlQueryArgs(this string url, string name, string value)
    {
        if( url.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(url));

        if( name.IsNullOrEmpty() )
            return url;

        string args = name + "=" + (value ?? string.Empty).UrlEncode();

        if( url.IndexOf('?') < 0 )
            return url + "?" + args;
        else
            return url + "&" + args;
    }


    /// <summary>
    /// 给URL增加查询参数
    /// </summary>
    /// <param name="url"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string AddUrlQueryArgs(this string url, IEnumerable<NameValue> list)
    {
        if( list == null )
            return url;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( var item in list ) {
                if( sb.Length > 0 )
                    sb.Append('&');

                sb.Append(System.Web.HttpUtility.UrlEncode(item.Name))
                    .Append('=')
                    .Append(System.Web.HttpUtility.UrlEncode(item.Value));
            }

            if( sb.Length == 0 )
                return url;

            if( url.IndexOf('?') < 0 )
                return url + "?" + sb.ToString();
            else
                return url + "&" + sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    /// <summary>
    /// 确保返回一个符合 http(s)://xxxx 的格式
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string EnsureUrlRoot(this string url)
    {
        if( url.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(url));

        string urlRoot = url.TrimEnd('/');

        if( urlRoot.StartsWith0("http://") == false && urlRoot.StartsWith0("https://") == false )
            urlRoot = "http://" + urlRoot;

        return urlRoot;
    }

    /// <summary>
    /// 获取网址的开头部分
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static string GetUrlRoot(this Uri uri)
    {
        if( uri == null )
            throw new ArgumentNullException(nameof(uri));

        return uri.Scheme + "://" + uri.Authority;
    }
}
