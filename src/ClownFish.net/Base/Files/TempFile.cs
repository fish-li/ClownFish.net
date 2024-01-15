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
    /// 创建临时文件
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public static TempFile CreateFile(byte[] body)
    {
        if( body == null )
            throw new ArgumentNullException(nameof(body));

        string filePath = GenTempFileFullName(".tmp");

        File.WriteAllBytes(filePath, body);

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
    /// 创建临时文件
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static TempFile CreateFile(Stream stream)
    {
        if( stream == null )
            throw new ArgumentNullException(nameof(stream));

        if( stream.CanSeek )
            stream.Position = 0;

        string filePath = GenTempFileFullName(".tmp");

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
        // 删除临时文件
        if( File.Exists(this.FilePath) ) {

            for( int i = 0; i < 10; i++ ) {
                try {
                    File.Delete(this.FilePath);
                    return;
                }
                catch {
                    // 文件有可能没有及时关闭
                    // 忽略所有异常
                    System.Threading.Thread.Sleep(300);
                }
            }
        }
    }


    /// <summary>
    /// 获取一个新的临时文件全路径名称
    /// </summary>
    /// <param name="extName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GenTempFileFullName(string extName)
    {
        return Path.Combine(EnvUtils.GetTempPath(), OprLog.GetNewId() + extName);
    }
}
