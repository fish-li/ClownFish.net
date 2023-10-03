using ClownFish.Log.Writers;

namespace ClownFish.Log;


/// <summary>
/// 日志记录的工具类
/// </summary>
public static class LogHelper
{
    /// <summary>
    /// 写日志时出现异常不能被处理时引用的事件
    /// </summary>
    public static event EventHandler<ExceptionEventArgs> OnError;

    /// <summary>
    /// 消息写入前的过滤检查回调委托
    /// </summary>
    private static Func<object, bool> s_filterDelegate;



    /// <summary>
    /// 注册过滤器委托
    /// </summary>
    /// <param name="filter"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RegisterFilter(Func<object, bool> filter)
    {
        if( filter == null )
            throw new ArgumentNullException(nameof(filter));

        if( LogConfig.IsInited )
            throw new InvalidOperationException($"日志组件已初始化完成，不允许再调用当前方法。");

        if( s_filterDelegate != null )
            throw new InvalidOperationException($"不允许多次调用 {nameof(RegisterFilter)} 方法。");

        s_filterDelegate = filter;
    }


    internal static bool InitCheck<T>(T info)
    {
        // 检查并确保日志组件初始化
        if( LogConfig.IsInited == false )
            throw new InvalidOperationException("日志组件还没有执行初始化！");

        // 如果禁用日志写入就直接返回
        if( LogConfig.Instance.Enable == false )
            return false;

        // 执行过滤委托
        if( s_filterDelegate != null && s_filterDelegate(info) == false )
            return false;

        // 所有需要记录到日志的数据类型必须配置，否则不知道以什么方式执行写入！
        if( WriterFactory.IsSupport(typeof(T)) == false )
            throw new NotSupportedException("不支持未配置的数据类型：" + typeof(T).Name);

        return true;
    }


    /// <summary>
    /// 记录指定的日志信息
    /// 说明：此方法是一个异步版本，内部维护一个缓冲队列，每 XXms 执行一次写入动作
    /// </summary>
    /// <typeparam name="T">日志信息的类型参数</typeparam>
    /// <param name="info">要写入的日志信息</param>
    public static void Write<T>(T info) where T : class, IMsgObject
    {
        if( info == null )
            return;

        if( InitCheck(info) == false )
            return;


        ClownFishCounters.Logging.WriteCount.Increment();

        // 获取指定消息类型的写入队列，并将日志信息放入队列
        ICacheQueue queue = CacheQueueManager.GetCacheQueue<T>();
        queue.Add(info);
    }



    /// <summary>
    /// 直接将一个异常对象写入到异常日志
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="addition"></param>
    public static void Write(Exception ex, string addition = null)
    {
        if( ex == null )
            return;

        OprLog log = OprLog.CreateErrLog(ex);
        log.SetMCA(2);
        log.Addition = addition;

        LogHelper.Write(log);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void RaiseErrorEvent(Exception ex)
    {
        if( ClownFishInit.AppExitToken.IsCancellationRequested )
            return;

        FatalErrorLogger.WriteFile(ex, "_ClownFish_log_error_");
        ClownFishCounters.Logging.WriterErrorCount.Increment();

        try {
            EventHandler<ExceptionEventArgs> handler = OnError;
            if( handler != null ) {
                handler(null, new ExceptionEventArgs(ex));
            }
        }
        catch( Exception ex2 ) {
            /* 当事件订阅时，再出异常就不能处理了，否则会形成循环调用。 */
            Console2.Error(ex2);
        }
    }


}
