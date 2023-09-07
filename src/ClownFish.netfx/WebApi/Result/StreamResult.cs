namespace ClownFish.WebApi.Result;

/// <summary>
/// 表示一个二进制的Action执行结果，可用于实现文件下载。
/// </summary>
public sealed class StreamResult : IActionResult
{
    private readonly byte[] _buffer;
    private readonly string _contentType;
    private readonly string _filename;


    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="buffer">文件内容的字节数组</param>
    public StreamResult(byte[] buffer)
        : this(buffer, null)
    {
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="buffer">文件内容的字节数组</param>
    /// <param name="contentType">文档类型，允许为空</param>
    public StreamResult(byte[] buffer, string contentType)
        : this(buffer, contentType, null)
    {
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="buffer">文件内容的字节数组</param>
    /// <param name="contentType">文档类型，允许为空</param>
    /// <param name="filename">下载对话框显示的文件名</param>
    public StreamResult(byte[] buffer, string contentType, string filename)
    {
        if( buffer == null || buffer.Length == 0 )
            throw new ArgumentNullException("buffer");

        _buffer = buffer;
        _filename = filename;


        if( string.IsNullOrEmpty(contentType) )
            _contentType = ResponseContentType.OctetStream;
        else
            _contentType = contentType;
    }


    /// <summary>
    /// 设置浏览器下载对话框中显示的文件名
    /// </summary>
    /// <param name="context"></param>
    /// <param name="filename"></param>
    internal static void SetDownloadFileName(NHttpContext context, string filename)
    {
        if( string.IsNullOrEmpty(filename) == false ) {

            // 文件名编码这块不知道未来会不会有问题，
            // 为了便于以后可以快速改进编码问题，且不修改这里的代码，这里定义一个类型和虚方法留着未来去重写。

            string headerValue = DownloadFileNameEncoder.Instance.GetFileNameHeader(filename, context.Request.UserAgent);

            if( string.IsNullOrEmpty(headerValue) == false )
                context.Response.SetHeader("Content-Disposition", headerValue);
        }
    }

    /// <summary>
    /// 实现IActionResult接口，执行输出
    /// </summary>
    /// <param name="context"></param>
    public void Ouput(NHttpContext context)
    {
        // 设置当前响应的文档类型
        context.Response.ContentType = _contentType;

        // 设置浏览器下载对话框中的保存文件名称
        SetDownloadFileName(context, _filename);

        // 将流内容输出到浏览器
        context.Response.OutputStream.Write(_buffer, 0, _buffer.Length);
    }

}




