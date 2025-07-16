namespace ClownFish.Base;

/// <summary>
/// 封装一个临时文件对象，在使用完后会自动清除
/// </summary>
public sealed class TempFile : IDisposable
{
    /// <summary>
    /// 临时文件的存放路径
    /// </summary>
    public string FilePath { get; private set; }

    private TempFile() { }

    /// <summary>
    /// 创建一个TempFile实例，它包含一个临时文件路径，此时临时文件还没有创建，可用于后续执行写入操作
    /// </summary>
    /// <param name="extName"></param>
    /// <returns></returns>
    public static TempFile CreateFile(string extName = ".tmp")
    {
        // 临时目录有可能会被删除，所以创建临时文件前先创建临时目录
        Directory.CreateDirectory(EnvUtils.GetTempPath());

        string filePath = GenTempFileFullName(extName);
        return new TempFile { FilePath = filePath };
    }


    /// <summary>
    /// 用指定的二进制数据 创建一个临时文件
    /// </summary>
    /// <param name="body"></param>
    /// <param name="extName">文件扩展名</param>
    /// <returns></returns>
    public static TempFile CreateFile(byte[] body, string extName = ".tmp")
    {
        if( body == null )
            throw new ArgumentNullException(nameof(body));

        string filePath = GenTempFileFullName(extName);

        // 临时目录有可能会被删除，所以创建临时文件前先创建临时目录
        Directory.CreateDirectory(EnvUtils.GetTempPath());

        RetryFile.WriteAllBytes(filePath, body);

        System.Threading.Thread.Sleep(50);

        // 确认文件是否已写入磁盘
        CheckFileLength(filePath, body.Length);

        return new TempFile { FilePath = filePath };
    }

    /// <summary>
    /// 确认文件是否已写入磁盘
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="length"></param>
    private static void CheckFileLength(string filePath, long length)
    {
        for( int i = 0; i < 6; i++ ) {

            FileInfo fileInfo = new FileInfo(filePath);
            if( fileInfo.Length < length )
                System.Threading.Thread.Sleep(300);
            else
                break;
        }
    }

    /// <summary>
    /// 用指定的二进制数据 创建一个临时文件
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="extName">文件扩展名</param>
    /// <returns></returns>
    public static TempFile CreateFile(Stream stream, string extName = ".tmp")
    {
        if( stream == null )
            throw new ArgumentNullException(nameof(stream));

        if( stream.CanSeek )
            stream.Position = 0;

        // 临时目录有可能会被删除，所以创建临时文件前先创建临时目录
        Directory.CreateDirectory(EnvUtils.GetTempPath());

        string filePath = GenTempFileFullName(extName);

        using( FileStream fileStream = File.Create(filePath) ) {
            stream.CopyTo(fileStream);
        }

        if( stream.CanSeek ) {
            CheckFileLength(filePath, stream.Length);
        }

        return new TempFile { FilePath = filePath };
    }


    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    void IDisposable.Dispose()
    {
        DeleteFile(this.FilePath);
    }


    /// <summary>
    /// 删除一个文件，如果一次失败会继续重试
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="tryCount"></param>
    /// <returns></returns>
    public static int DeleteFile(string filePath, int tryCount = 10)
    {
        if( File.Exists(filePath) ) {

            for( int i = 0; i < tryCount; i++ ) {
                try {
                    File.Delete(filePath);
                    return 1;
                }
                catch {
                    // 文件有可能没有及时关闭
                    // 忽略所有异常
                    System.Threading.Thread.Sleep(300);
                }
            }
        }

        return 0;
    }


    /// <summary>
    /// 获取一个新的临时文件全路径名称
    /// </summary>
    /// <param name="extName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GenTempFileFullName(string extName)
    {
        string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N");
        return Path.Combine(EnvUtils.GetTempPath(), filename + extName);
    }
}
