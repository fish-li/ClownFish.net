namespace ClownFish.Web.Utils;

public static class TempFileExtensions
{
    /// <summary>
    /// 创建一个临时文件，并将指定的数据写入到临时文件中
    /// </summary>
    /// <param name="httpContext">NHttpContext实例</param>
    /// <param name="body">需要写入到临时文件的数据</param>
    /// <returns>已创建的临时文件的文件路径</returns>
    public static string CreateTempFile(this NHttpContext httpContext, byte[] body)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        TempFile tempfile = TempFile.CreateFile(body);
        httpContext.RegisterForDispose(tempfile);
        return tempfile.FilePath;
    }


    /// <summary>
    /// 创建一个临时文件，并将指定的数据写入到临时文件中
    /// </summary>
    /// <param name="httpContext">NHttpContext实例</param>
    /// <param name="stream">需要写入到临时文件的数据</param>
    /// <returns>已创建的临时文件的文件路径</returns>
    public static string CreateTempFile(this NHttpContext httpContext, Stream stream)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        TempFile tempfile = TempFile.CreateFile(stream);
        httpContext.RegisterForDispose(tempfile);
        return tempfile.FilePath;
    }


    /// <summary>
    /// 将上传文件的内容写入到服务端的本地临时文件中，当HTTP请求结束时会自动删除此临时文件
    /// </summary>
    /// <param name="httpContext">NHttpContext实例</param>
    /// <param name="file">上传文件</param>
    /// <returns>已创建的临时文件的文件路径</returns>
    public static string CreateTempFile(this NHttpContext httpContext, IFormFile file)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));
        if( file == null )
            throw new ArgumentNullException(nameof(file));

        using( Stream upStream = file.OpenReadStream() ) {
            TempFile tempfile = TempFile.CreateFile(upStream);
            httpContext.RegisterForDispose(tempfile);
            return tempfile.FilePath;
        }        
    }

    
}
