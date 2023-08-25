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

        string tempPath = Path.GetFullPath(Path.GetTempPath());
        string filePath = Path.Combine(tempPath, Guid.NewGuid().ToString("N") + ".tmp");


        File.WriteAllBytes(filePath, body);

        // 确认文件是否已写入磁盘
        for( int i = 0; i < 3; i++ ) {

            FileInfo fileInfo = new FileInfo(filePath);
            if( fileInfo.Length < body.Length )
                System.Threading.Thread.Sleep(300);
            else
                break;
        }

        return new TempFile { FilePath = filePath };
    }

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    void IDisposable.Dispose()
    {
        // 删除临时文件

        for( int i = 0; i < 10; i++ ) {
            try {
                File.Delete(this.FilePath);
            }
            catch {
                // 文件有可能没有及时关闭
                // 忽略所有异常
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
