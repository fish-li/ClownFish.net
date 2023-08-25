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

    /// <summary>
    /// 分隔行
    /// </summary>
    public static readonly string SeparatedLine = "--------------------------------------------------";


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
        Console.WriteLine($"[ERROR] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");

        if( ex != null )
            Console.WriteLine(ex.ToString());
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

        string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
        Console.WriteLine($"[ERROR] {DateTime.Now.ToTime23String()} [thread={threadId}]: {ex.ToString2()}");
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
        Console.WriteLine($"[WARN] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");
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
        Console.WriteLine($"[INFO] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");
    }


    /// <summary>
    /// 输出一条消息到控制台
    /// </summary>
    /// <param name="message"></param>
    public static void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    /// <summary>
    /// 在Console上输出一个分隔行
    /// </summary>
    public static void WriteSeparatedLine()
    {
        Console.WriteLine(SeparatedLine);
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
        Console.WriteLine($"[DEBUG] {DateTime.Now.ToTime23String()} [thread={threadId}]: {message}");
    }



    /// <summary>
    /// 在控制台中显示一次HTTP的调用过程
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <param name="success"></param>
    public static void ShowHTTP(HttpOption request, HttpResult<string> response, bool success)
    {
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
        System.Threading.Thread.Sleep(500);
    }

}
