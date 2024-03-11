using Newtonsoft.Json.Linq;

namespace ClownFish.Http.Utils;

/// <summary>
/// Url相关处理的工具类
/// </summary>
public static class Urls
{
    private static readonly Regex s_urlRootRegex = new Regex(@"\w+://[^/]+", RegexOptions.Compiled);

    /// <summary>
    /// 从一个URL中获取网站的根地址，也就是提取：http://xxx.xxxx.com 部分。
    /// </summary>
    /// <param name="absoluteUrl"></param>
    /// <returns></returns>
    public static string GetWebSiteRoot(string absoluteUrl)
    {
        if( absoluteUrl.IsNullOrEmpty() )
            return absoluteUrl;

        Match m = s_urlRootRegex.Match(absoluteUrl);

        if( m.Success )
            return m.Groups[0].Value;
        else
            return string.Empty;
    }


    /// <summary>
    /// URL合并
    /// </summary>
    /// <param name="root">站点的根网址，例如：http://www.abc.com</param>
    /// <param name="path">站点内的路径部分，例如： /aa/bb/cc.aspx</param>
    /// <returns>返回一个完整的URL，例如：http://www.abc.com/aa/bb/cc.aspx</returns>
    public static string Combine(string root, string path)
    {
        if( root.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(root));

        if( path.IsNullOrEmpty() )
            return root;

        if( path.StartsWith('/') ) {
            if( root.EndsWith('/') )
                return root.TrimEnd('/') + path;
            else
                return root + path;
        }
        else {
            if( root.EndsWith('/') )
                return root + path;
            else
                return root + "/" + path;
        }
    }


    /// <summary>
    /// 从一个URL中去掉开头部分，例如：http://wwww.abc.com/aa/bb.aspx  结果：/aa/bb.aspx
    /// </summary>
    /// <param name="absoluteUrl"></param>
    /// <returns></returns>
    public static string RemoveHost(string absoluteUrl)
    {
        if( absoluteUrl.IsNullOrEmpty() )
            return absoluteUrl;

        if( absoluteUrl.StartsWithIgnoreCase("http://") ) {
            return UrlSubstring(absoluteUrl, "http://");
        }
        else if( absoluteUrl.StartsWithIgnoreCase("https://") ) {
            return UrlSubstring(absoluteUrl, "https://");
        }
        else {
            // 正确的格式，不做处理
            return absoluteUrl;
        }

        string UrlSubstring(string absoluteUrl, string prefix)
        {
            int p = absoluteUrl.IndexOf('/', prefix.Length);
            if( p < 0 ) {
                // 格式不正确，强制跳转到 "/"  例如：https://wwww.abc.com
                return "/";
            }
            else {
                // 强制去掉HOST部分
                return absoluteUrl.Substring(p);
            }
        }
    }

    
}
