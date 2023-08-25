namespace ClownFish.Log;

/// <summary>
/// 一个简单的流式消息日志记录实现类，所有写入将会以同步方式写入文件。
/// </summary>
public class MessageLogger
{
    private readonly string _filePath;
    private readonly object _syncObject;
    private readonly long _maxLength;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="logFilePath">日志的保存文件路径</param>
    /// <param name="supportConcurrent">是否支持多线程的并发调用</param>
    /// <param name="maxLength">文件最大长度</param>
    public MessageLogger(string logFilePath, bool supportConcurrent = false, long maxLength = 0)
    {
        if( string.IsNullOrEmpty(logFilePath) )
            throw new ArgumentNullException(logFilePath);

        _filePath = logFilePath;
        _maxLength = maxLength;

        if( supportConcurrent ) {
            _syncObject = new object();
        }
    }



    /// <summary>
    /// 写入一条消息到日志文件。
    /// 说明：为了防止程序突然崩溃，写入消息时，不做任何缓冲处理，且每次写入时打开文件
    /// </summary>
    /// <param name="category">消息类别，默认：INFO</param>
    /// <param name="message">消息文本</param>
    public virtual string Write(string message, string category = null)
    {
        // 扩展点：如果希望在写文件时，同时将消息输出到控制台，可以重写这个方法。

        string line = GetLine(message, category);

        WriteToFile(line);

        return line;
    }

    /// <summary>
    /// 根据指定参数计算要写入文件的消息行文本
    /// </summary>
    /// <param name="category"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    protected virtual string GetLine(string message, string category = null)
    {
        if( string.IsNullOrEmpty(message) )
            return null;

        if( string.IsNullOrEmpty(category) )
            category = "INFO";

        string time = DateTime.Now.ToTimeString();
        Thread currentThread = Thread.CurrentThread;
        string threadId = currentThread.Name ?? "th_" + currentThread.ManagedThreadId.ToString();

        return $"{time} [{threadId}] [{category}] {message}";
    }

    /// <summary>
    /// 将消息文本写入文件
    /// </summary>
    /// <param name="line"></param>
    protected virtual void WriteToFile(string line)
    {
        if( line == null )
            return;

        try {
            if( _syncObject != null ) {
                lock( _syncObject ) {
                    FileHelper.AppendAllText(_filePath, line, true, _maxLength);
                }
            }
            else {
                FileHelper.AppendAllText(_filePath, line, true, _maxLength);
            }
        }
        catch { /* 忽略写日志时遇到的错误，例如：磁盘空间已满，文件被占用  */ }
    }

   
}
