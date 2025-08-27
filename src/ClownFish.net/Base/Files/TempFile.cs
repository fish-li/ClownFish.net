namespace ClownFish.Base;

/// <summary>
/// 封装一个临时文件对象，在当前对象释放时会自动删除临时文件。可以理解为：给 filePath-string 实现了 IDisposable 接口
/// </summary>
public sealed class TempFile : IDisposable
{
    /// <summary>
    /// 临时文件的存放路径
    /// </summary>
    public string FilePath { get; private set; }

    private TempFile() { }

    /// <inheritdoc/>
    ~TempFile()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    private void Dispose(bool disposing)
    {
        // 这里没有非托管资源，所以简化处理

        if( this.FilePath != null ) {
            DeleteFile(this.FilePath);
            this.FilePath = null;
        }
    }


    /// <summary>
    /// 创建一个TempFile实例，并生成一个临时文件路径，此时临时文件还没有创建，可用于后续执行写入操作
    /// </summary>
    /// <param name="extName">文件扩展名，例如：".dat"</param>
    /// <param name="prefix">文件名前缀</param>
    /// <returns></returns>
    public static TempFile CreateFile(string extName = ".tmp", string prefix = null)
    {
        // 临时目录有可能会被删除，所以创建临时文件前先创建临时目录
        Directory.CreateDirectory(EnvUtils.GetTempPath());

        string filePath = GenTempFileFullName(extName, prefix);
        return new TempFile { FilePath = filePath };
    }


    /// <summary>
    /// 创建一个TempFile实例，它记录了一个临时文件路径。方法本身并不会创建文件，但会在对象释放时删除文件。
    /// </summary>
    /// <param name="filePath">一个文件路径</param>
    /// <returns></returns>
    public static TempFile Create(string filePath)
    {
        if( filePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filePath));

        // 如果参数是一个相对路径，就将文件放在临时目录下
        string filePath2 = Path.Combine(EnvUtils.GetTempPath(), filePath);

        // 这里不检查文件路径的目录是否存在

        return new TempFile { FilePath = filePath2 };
    }


    /// <summary>
    /// 用指定的二进制数据 创建一个临时文件
    /// </summary>
    /// <param name="data">需要写入临时文件的数据</param>
    /// <param name="extName">文件扩展名，例如：".dat"</param>
    /// <param name="prefix">文件名前缀</param>
    /// <returns></returns>
    public static TempFile CreateFile(byte[] data, string extName = ".tmp", string prefix = null)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        string filePath = GenTempFileFullName(extName, prefix);

        // 临时目录有可能会被删除，所以创建临时文件前先创建临时目录
        Directory.CreateDirectory(EnvUtils.GetTempPath());

        RetryFile.WriteAllBytes(filePath, data);

        //System.Threading.Thread.Sleep(50);

        // 确认文件是否已写入磁盘
        CheckFileLength(filePath, data.Length);

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
    /// <param name="data">需要写入临时文件的数据</param>
    /// <param name="extName">文件扩展名，例如：".dat"</param>
    /// <param name="prefix">文件名前缀</param>
    /// <returns></returns>
    public static TempFile CreateFile(Stream data, string extName = ".tmp", string prefix = null)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( data.CanSeek )
            data.Position = 0;

        // 临时目录有可能会被删除，所以创建临时文件前先创建临时目录
        Directory.CreateDirectory(EnvUtils.GetTempPath());

        string filePath = GenTempFileFullName(extName, prefix);

        using( FileStream fileStream = RetryFile.Create(filePath) ) {
            data.CopyTo(fileStream);
        }

        if( data.CanSeek ) {
            CheckFileLength(filePath, data.Length);
        }

        return new TempFile { FilePath = filePath };
    }


    /// <summary>
    /// 删除一个文件，如果一次失败会继续重试
    /// </summary>
    /// <param name="filePath">需要删除的文件全路径</param>
    /// <param name="tryCount">尝试次数</param>
    /// <param name="errorWaitMs">出现异常后的重试等待时间，单位：毫秒</param>
    /// <returns>成功删除文件返回 1，失败返回 0</returns>
    public static int DeleteFile(string filePath, int tryCount = 10, int errorWaitMs = 300)
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
                    System.Threading.Thread.Sleep(errorWaitMs);
                }
            }
        }

        return 0;
    }


    /// <summary>
    /// 获取一个新的临时文件全路径名称
    /// </summary>
    /// <param name="extName">文件扩展名，例如：".dat"</param>
    /// <param name="prefix">文件名前缀</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GenTempFileFullName(string extName, string prefix = null)
    {
        string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N");
        return Path.Combine(EnvUtils.GetTempPath(), prefix + filename + extName);
    }
}
