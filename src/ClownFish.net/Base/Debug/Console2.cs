namespace ClownFish.Base;

/// <summary>
/// 控制台相关的方法集合
/// </summary>
public static class Console2
{
    /// <summary>
    /// 是否启用 INFO 级别的日志输出
    /// </summary>
    public static bool InfoEnabled = true;

#if NET9_0_OR_GREATER
    private static readonly Lock s_lock = new Lock();
#else
    private static readonly object s_lock = new object();
#endif

    private static IConsole s_console = new SysConsoleImpl();

    /// <summary>
    /// 分隔行
    /// </summary>
    public static readonly string SeparatedLine = "--------------------------------------------------";

    private static StringBuilder s_listenLines;

    /// <summary>
    /// 将所有控制台输出写到指定的文件中
    /// </summary>
    /// <param name="outFilePath">一个文件路径，随后程序的所有控制台输出都将写入此文件。 如果指定的文件不存在，程序会自动创建，如果指定的文件存在，文件将清空。</param>
    /// <param name="maxFileLength">文件的最大长度。超过最大长度后，文件内容会清空，然后继续写入。如果此参数小于等于零，则不检查文件长度，此时有可能会把磁盘写爆。</param>
    /// <param name="syncSysConsole">是否同时将输出消息写入 System.Console</param>
    /// <param name="appendMode">第一次打开文件时，是否使用追加模式，否则会创建一个空文件</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void SetOutToFile(string outFilePath, long maxFileLength, bool syncSysConsole = false, bool appendMode = false)
    {
        if( outFilePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(outFilePath));

        s_console.Dispose();

        s_console = new FileConsoleImpl(outFilePath, maxFileLength, syncSysConsole, appendMode);
    }

    internal static void ResetOut()  // for UnitTest
    {
        s_console.Dispose();
        s_console = new SysConsoleImpl();
    }

    /// <summary>
    /// 输出一条消息到控制台
    /// </summary>
    /// <param name="message"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void WriteLine(string message)
    {
        s_console.WriteLine(message);

        if( s_listenLines != null )
            s_listenLines.AppendLine(message);
    }

    /// <summary>
    /// 开始监听所有对 Console 的输出调用，并记录到内存中。
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void BeginListen()
    {
        if( s_listenLines == null )
            s_listenLines = new StringBuilder(1024 * 16);
    }

    /// <summary>
    /// 结束BeginListen()的监听，并将内存中的监听结果写入到临时文件
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static string EndListen(string filename = null)
    {
        if( s_listenLines == null )
            return null;

        if( filename == null )
            filename = "_ConsoleWrite.log";

        string filePath = Path.Combine(EnvUtils.GetTempPath(), filename);

        WriteLine("All startup log write to file: " + filePath);

        string text = s_listenLines.ToString();
        s_listenLines = null;

        RetryFile.WriteAllText(filePath, text);
        return filePath;
    }

    private static readonly string s_flag1 = ":";
    private static readonly string s_flag2 = " :";

    /// <summary>
    /// 输出一条消息到控制台
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ex"></param>
    public static void Error(string message, Exception ex = null)
    {
        if( message.IsNullOrEmpty() )
            return;

        ClownFishCounters.Console2.Error.Increment();

        string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
        string separator = threadId.Length == 1 ? s_flag2 : s_flag1;

        // 确保 “二行” 文本 **紧挨** 在一起
        lock( s_lock ) {
            Console2.WriteLine($"{Environment.NewLine}[EROR] {DateTime.Now.ToTime23String()} [thread={threadId}]{separator} {message}");

            if( ex != null )
                Console2.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// 显示一个异常对象消息到控制台
    /// </summary>
    /// <param name="ex"></param>
    public static void Error(Exception ex)
    {
        if( ex == null )
            return;

        ClownFishCounters.Console2.Error.Increment();

        if( ex is OutOfMemoryException )
            ClownFishCounters.Status.OomError.Increment();

        string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
        string separator = threadId.Length == 1 ? s_flag2 : s_flag1;

        Console2.WriteLine($"{Environment.NewLine}[EROR] {DateTime.Now.ToTime23String()} [thread={threadId}]{separator} {ex.ToString2()}");
    }

    /// <summary>
    /// 将异常对象做为警告消息显示到控制台
    /// </summary>
    /// <param name="ex"></param>
    public static void Warnning(Exception ex)
    {
        if( ex == null )
            return;

        Warnning(ex.ToString());
    }



    /// <summary>
    /// 输出一条消息到控制台
    /// </summary>
    /// <param name="message"></param>
    public static void Warnning(string message)
    {
        if( message.IsNullOrEmpty() )
            return;

        ClownFishCounters.Console2.Warnning.Increment();

        string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
        string separator = threadId.Length == 1 ? s_flag2 : s_flag1;

        Console2.WriteLine($"[WARN] {DateTime.Now.ToTime23String()} [thread={threadId}]{separator} {message}");
    }


    /// <summary>
    /// 输出一条消息到控制台
    /// </summary>
    /// <param name="message"></param>
    public static void Info(string message)
    {
        if( InfoEnabled == false )
            return;

        if( message.IsNullOrEmpty() )
            return;

        string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
        string separator = threadId.Length == 1 ? s_flag2 : s_flag1;

        Console2.WriteLine($"[INFO] {DateTime.Now.ToTime23String()} [thread={threadId}]{separator} {message}");
    }


    /// <summary>
    /// 在Console上输出一个分隔行
    /// </summary>
    public static void WriteSeparatedLine()
    {
        Console2.WriteLine(SeparatedLine);
    }


    /// <summary>
    /// 输出一条【调试消息】到控制台。仅当【开发】环境中调用有效。
    /// </summary>
    /// <param name="message"></param>
    public static void Debug(string message)
    {
        if( EnvUtils.IsDevEnv == false )
            return;

        if( message.IsNullOrEmpty() )
            return;

        string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
        string separator = threadId.Length == 1 ? s_flag2 : s_flag1;

        Console2.WriteLine($"[DBUG] {DateTime.Now.ToTime23String()} [thread={threadId}]{separator} {message}");
    }



    /// <summary>
    /// 在控制台中显示一次HTTP的调用过程
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <param name="success"></param>
    /// <param name="time"></param>
    public static void ShowHTTP(HttpOption request, HttpResult<string> response, bool success, TimeSpan? time = null)
    {
        // 确保 多行文本 **紧挨** 在一起
        lock( s_lock ) {
            Console2.WriteLine("================================ Request =============================================");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console2.WriteLine(request.ToRawText(3));

            Console.ResetColor();
            Console2.WriteLine("================================ Response ============================================ " + time?.ToString());

            if( response != null ) {
                if( success ) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console2.WriteLine(response.ToAllText(true));
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console2.WriteLine(response.ToAllText(true));
                }
            }

            Console.ResetColor();
            Console2.WriteLine("================================ Response END ============================================");
        }
        System.Threading.Thread.Sleep(500);
    }

}


internal interface IConsole : IDisposable
{
    void WriteLine(string line);
}

internal sealed class SysConsoleImpl : IConsole
{
    public void WriteLine(string line)
    {
        Console.WriteLine(line);
    }
    public void Dispose()
    {
    }
}

internal sealed class FileConsoleImpl : IConsole
{
    private static readonly object s_lock = new object();
    private static readonly byte[] s_bytes = Environment.NewLine.GetBytes();
    private static readonly ValueCounter s_counter = new ValueCounter();
    internal static long CheckInterval = 1000;

    private readonly string _filePath;
    private readonly long _maxFileLength;
    private FileStream _stream;
    private readonly bool _syncSysConsole;


    public FileConsoleImpl(string outFilePath, long maxFileLength, bool syncSysConsole, bool appendMode)
    {
        // 确保文件所在的目录是存在的，否则在创建文件时会出现异常
        string parentDirectory = Path.GetDirectoryName(outFilePath);
        Directory.CreateDirectory(parentDirectory);

        _syncSysConsole = syncSysConsole;
        _filePath = outFilePath;
        _maxFileLength = maxFileLength;

        OpenFile(appendMode);
    }

    private void OpenFile(bool appendMode)
    {
        FileMode fileMode = appendMode ? FileMode.Append : FileMode.Create;
        _stream = new FileStream(_filePath, fileMode, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan);
    }

    public void WriteLine(string line)
    {
        lock( s_lock ) {

            if( _maxFileLength > 0 && s_counter.Increment() % CheckInterval == 0 ) {
                if( _stream.Position > _maxFileLength ) {
                    _stream.Close();
                    Thread.Sleep(50);
                    OpenFile(false);  // 文件长度过大，创建新文件，重新开始
                }
            }

            byte[] data = line.GetBytes();
            _stream.Write(data, 0, data.Length);
            _stream.Write(s_bytes, 0, s_bytes.Length);
            _stream.Flush();

            if( _syncSysConsole ) {
                Console.WriteLine(line);
            }
        }
    }

    public void Dispose()
    {
        if( _stream != null ) {
            _stream.Dispose();
        }
    }
}