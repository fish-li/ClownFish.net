namespace ClownFish.Http.Utils;

/// <summary>
/// 计算用于下载文件的编码工具类
/// </summary>
public sealed class DownloadFileNameEncoder
{
    /// <summary>
    /// 单例引用
    /// </summary>
    public static readonly DownloadFileNameEncoder Instance = new DownloadFileNameEncoder();

    private DownloadFileNameEncoder() { }

    /// <summary>
    /// 根据指定的文件名，按照HTTP相关规范计算用于响应头可以接受的字符串
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public string GetFileNameHeader(string filename, string userAgent)
    {
        if( string.IsNullOrEmpty(filename) )
            return string.Empty;

        // 参考：
        // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Content-Disposition

        // http://greenbytes.de/tech/webdav/draft-reschke-rfc2231-in-http-latest.html
        //    3.2 Parameter Value Character Set and Language Information

        // 文件名的编码不能使用 HttpUtility.UrlEncode
        // 因为 HTTP 规范 RFC 5987 中的不转义字符和 HttpUtility.UrlEncode 不转义字符的范围不一样

        string headerValue = null;

        if( userAgent != null && userAgent.Contains("MSIE") ) {
            headerValue = string.Format("attachment; filename=\"{0}\"", EncodeFileName(filename));
        }
        else {
            // 符合新标准的浏览器（部分特殊字符仍然有问题，汉字没问题）
            headerValue = "attachment; filename*=UTF-8''" + EncodeFileName(filename);
        }

        return headerValue;
    }


    /// <summary>
    /// 对文件名按 RFC 5987 规范编码
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public string EncodeFileName(string filename)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            byte[] bytes = Encoding.UTF8.GetBytes(filename);

            foreach( byte b in bytes ) {
                if( IsSkipChar((char)b) ) {
                    sb.Append((char)b);
                }
                else {
                    sb.Append('%');
                    sb.Append(IntToHex(b >> 4 & 15));
                    sb.Append(IntToHex(b & 15));
                }
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    private char IntToHex(int n)
    {
        if( n <= 9 )
            return (char)(n + 48);

        return (char)(n - 10 + 97);
    }

    private bool IsSkipChar(char ch)
    {
        if( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') )
            return true;


        switch( ch ) {
            case '-':
            case '.':
            case '_':
            case '~':
            case ':':
            case '!':
            case '$':
            case '&':
            case '+':
                return true;
        }

        return false;
    }
}

