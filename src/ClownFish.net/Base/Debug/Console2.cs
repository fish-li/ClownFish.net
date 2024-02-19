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

    private static readonly object s_lock = new object();

    /// <summary>
    /// 分隔行
    /// </summary>
    public static readonly string SeparatedLine = "--------------------------------------------------";

    private static StringBuilder s_listenLines;

    /// <summary>
    /// 输出一条消息到控制台
    /// </summary>
    /// <param name="message"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void WriteLine(string message)
    {
        Console.WriteLine(message);

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

        string text = s_listenLines.ToString();
        s_listenLines = null;

        if( filename == null )
            filename = "_ConsoleWrite.log";

        string filePath = Path.Combine(EnvUtils.GetTempPath(), filename);
        RetryFile.WriteAllText(filePath, text);
        return filePath;
    }

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

        // 确保 “二行” 文本 **紧挨** 在一起
        lock( s_lock ) {
            Console2.WriteLine($"[EROR] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");

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
        Console2.WriteLine($"[EROR] {DateTime.Now.ToTime23String()} [thread={threadId}]: {ex.ToString2()}");
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
        Console2.WriteLine($"[WARN] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");
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
        Console2.WriteLine($"[INFO] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");
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
        Console2.WriteLine($"[DEBUG] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");
    }



    /// <summary>
    /// 在控制台中显示一次HTTP的调用过程
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <param name="success"></param>
    public static void ShowHTTP(HttpOption request, HttpResult<string> response, bool success)
    {
        // 确保 多行文本 **紧挨** 在一起
        lock( s_lock ) {
            Console2.WriteLine("================================ Request =============================================");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console2.WriteLine(request.ToRawText());

            Console.ResetColor();
            Console2.WriteLine("================================ Response ============================================");

            if( success ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console2.WriteLine(response?.ToAllText(true));
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console2.WriteLine(response?.ToAllText(true));
            }

            Console.ResetColor();
            Console2.WriteLine("================================ Response END ============================================");
        }
        System.Threading.Thread.Sleep(500);
    }

}
