namespace ClownFish.Base.WebClient;

internal static class HttpOptionBuilder
{
    public static void SetRequestLine(this HttpOption httpOption, string requestLine)
    {
        int p1 = requestLine.IndexOf(' ');
        int p2 = requestLine.LastIndexOf(' ');

        if( p1 < 0 || p1 == p2 )
            throw new ArgumentException($"不能识别的请求文本格式，开始行：[{requestLine}]");

        // 设置请求方法，GET OR POST
        httpOption.Method = requestLine.Substring(0, p1);


        // 不使用HTTP协议版本，只做校验。
        string httpVersion = requestLine.Substring(p2 + 1);
        if( httpVersion.StartsWith("HTTP/", StringComparison.Ordinal) == false )
            throw new ArgumentException($"不能识别的请求文本格式，开始行：[{requestLine}]");

        httpOption.Url = requestLine.Substring(p1 + 1, p2 - p1 - 1);
    }



    public static void SetHeaders(this HttpOption httpOption, StringReader reader)
    {
        string line = null;
        while( (line = reader.ReadLine()) != null ) {
            if( line.Length > 0 ) {
                // 处理请求头
                int p3 = line.IndexOf(':');
                if( p3 > 0 ) {
                    string name = line.Substring(0, p3);

                    // 这个头直接丢弃，因为文本在计算二进制时会随着编码不同而变化
                    if( name.EqualsIgnoreCase("Content-Length") )
                        continue;

                    // 这里强制要求的请求头格式： "name: value" ，中间一个冒号加一个空格，如果格式不正确，有可能会出现异常！
                    string value = line.Substring(p3 + 2);
                    // line.Substring(p3 + 1).TrimTrimStart(' ')  这种写法会造成无意义的性能浪费，所以不采用！

                    if( name.Is("Connection") ) {
                        // Connection 头有二个可选值，Keep-Alive  or  close
                        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Connection
                        // 下面为了简单，只判断其中之一。
                        httpOption.KeepAlive = value.Is("Keep-Alive");
                        continue;
                    }

#if NET6_0_OR_GREATER
                    if( name == "--unix-socket" ) {
                        httpOption.UnixSocketEndPoint = value;
                        continue;
                    }
#endif
                    httpOption.Headers.Add(name, value);
                }
                else
                    throw new ArgumentException($"不能识别的请求文本格式，请求头：[{line}]");
            }
            else
                break;
        }
    }


    public static void FixHeaders(this HttpOption httpOption)
    {
        string contentType = httpOption.Headers[HttpHeaders.Request.ContentType];
        if( contentType != null ) {
            // 可能的格式：Content-Type: application/x-www-form-urlencoded; charset=gb2312
            // 此时，只需要获取 "application/x-www-form-urlencoded"

            int p = contentType.IndexOf(';');
            if( p > 0 ) {
                // 注意：这里丢弃了 charset 设置，因为 HttpClient 固定以 utf-8 编码方式发送请求！
                httpOption.Headers[HttpHeaders.Request.ContentType] = contentType.Substring(0, p);
            }
        }
    }


}
