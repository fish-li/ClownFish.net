namespace ClownFish.Base.WebClient;

/// <summary>
/// 表示需要上传的文件数据结构
/// </summary>
[Serializable]
public sealed class HttpFile
{
    ///// <summary>
    ///// 获取上载文件的大小（以字节为单位）。
    ///// </summary>
    //public long ContentLength { get; set; }
    ///// <summary>
    ///// 获取客户端发送的文件的 MIME 内容类型。
    ///// </summary>
    //public string ContentType { get; set; }
    /// <summary>
    /// 获取客户端上的文件的完全限定名称，
    /// 上传时需要指定。
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 需要上传的文件内容，此类型的 FileBody, FileInfo, BodyStream 是三选一。
    /// </summary>
    public byte[] FileBody { get; set; }

    /// <summary>
    /// 需要上传的文件，此类型的 FileBody, FileInfo, BodyStream 是三选一。
    /// </summary>
    public FileInfo FileInfo { get; set; }

    /// <summary>
    /// 需要上传的文件内容流。此类型的 FileBody, FileInfo, BodyStream 是三选一。
    /// </summary>
    public Stream BodyStream { get; set; }



    /// <summary>
    /// 根据FileInfo对象创建HttpFile对象
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static HttpFile CreateFromFileInfo(FileInfo file)
    {
        if( file == null )
            throw new ArgumentNullException(nameof(file));

        HttpFile result = new HttpFile();
        result.FileName = file.FullName;
        //result.ContentLength = file.Length;

        //result.FileBody = RetryFile.ReadAllBytes(file.FullName);
        result.FileInfo = file;
        return result;
    }



    //#if NETFRAMEWORK
    //		internal static HttpFile CreateHttpFileFromHttpPostedFile(System.Web.HttpPostedFile file)
    //        {
    //			if( file == null )
    //				throw new ArgumentNullException(nameof(file));

    //			HttpFile result = new HttpFile();
    //            result.ContentLength = file.ContentLength;
    //            result.ContentType = file.ContentType;
    //            result.FileName = file.FileName;
    //			result.BodyStream = file.InputStream;

    //            return result;
    //        }
    //#endif

    internal void Validate()
    {
        if( this.FileName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.FileName));

        //if( this.ContentLength <= 0 )
        //	throw new ArgumentOutOfRangeException(nameof(this.ContentLength));

        if( this.FileBody == null && this.FileInfo == null && this.BodyStream == null )
            throw new ArgumentNullException(nameof(this.FileBody));
    }
}
