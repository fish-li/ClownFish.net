namespace ClownFish.Log;

/// <summary>
/// 用于记录致命异常的日志，例如：程序初始化，写日志时失败。
/// 默认行为：错误消息固定写入到系统临时目录下，文件名: {sys-temp-directory}\_ClownFish_net_error_{date}.txt
/// </summary>
public static class FatalErrorLogger
{
    /// <summary>
    /// 当调用 LogException 方法时引发的事件，这表示有一个异常已发生。
    /// </summary>
    public static event EventHandler<ExceptionEventArgs> OnExceptionRecived;

    /// <summary>
    /// 记录一个致命的异常
    /// </summary>
    /// <param name="ex"></param>
    public static void LogException(Exception ex)
    {
        WriteFile(ex, "_ClownFish_net_error_");
        ClownFishCounters.Logging.FatalErrorCount.Increment();

        try {
            EventHandler<ExceptionEventArgs> eventHandler = OnExceptionRecived;
            if( eventHandler != null )
                eventHandler(null, new ExceptionEventArgs(ex));
        }
        catch( Exception ex2 ) {
            /* 当事件订阅时，再出异常就不能处理了，否则会形成循环调用。 */
            Console2.Error(ex2);
        }
    }


    internal static void WriteFile(Exception ex, string filenamePrefix)
    {
        Console2.Error(ex);

        // 尽量将日志写到指定的目录，否则写到系统临时目录
        string tempPath = EnvUtils.GetTempPath();
        string errorFile = Path.Combine(tempPath, $"{filenamePrefix}{DateTime.Now.ToDateString()}.txt");
        string message = $@"
=====================================================
Time: {DateTime.Now.ToTimeString()}
{ex.ToString()}

";
        try {
            RetryFile.AppendAllText(errorFile, message, Encoding.UTF8);
        }
        catch {
            // 这里只能吃掉异常。
        }
    }

}
