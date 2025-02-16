namespace ClownFish.Base;

/// <summary>
/// 线程相关工具类
/// </summary>
public static class ThreadUtils
{
    /// <summary>
    /// 以后台方式执行一段代码逻辑，内部使用 Task.Run(...)
    /// </summary>
    /// <param name="operatorName">操作名称，异常时记录到日志中</param>
    /// <param name="action">代码逻辑委托</param>
    /// <returns></returns>
    public static Task RunTask(string operatorName, Action action)
    {
        if( operatorName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(operatorName));
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        using( ExecutionContext.SuppressFlow() ) {
            return Task.Run(() => {
                try {
                    action.Invoke();
                }
                catch( Exception ex ) {
                    HandleException(operatorName, ex);
                }
            });
        }
    }

    /// <summary>
    /// 以后台方式执行一段【异步的】代码逻辑，内部使用 Task.Run(async () => { ... })
    /// </summary>
    /// <param name="operatorName">操作名称，异常时记录到日志中</param>
    /// <param name="action">代码逻辑委托</param>
    /// <returns></returns>
    public static Task RunAsync(string operatorName, Func<Task> action)
    {
        if( operatorName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(operatorName));
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        using( ExecutionContext.SuppressFlow() ) {
            return Task.Run(async () => {
                try {
                    await action.Invoke().ConfigureAwait(false);
                }
                catch( Exception ex ) {
                    HandleException(operatorName, ex);
                }
            });
        }
    }

    /// <summary>
    /// 以后台方式执行一段代码逻辑，内部使用 ThreadPool.QueueUserWorkItem(...)
    /// </summary>
    /// <param name="operatorName">操作名称，异常时记录到日志中</param>
    /// <param name="action">需要执行的逻辑过程</param>
    public static void Run(string operatorName, Action action)
    {
        if( operatorName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(operatorName));
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        using( ExecutionContext.SuppressFlow() ) {
            ThreadPool.QueueUserWorkItem((xx) => {
                try {
                    action.Invoke();
                }
                catch( Exception ex ) {
                    HandleException(operatorName, ex);
                }
            });
        }
    }

    /// <summary>
    /// 以后台方式执行一段代码逻辑，内部使用 ThreadPool.QueueUserWorkItem(...)
    /// </summary>
    /// <param name="operatorName">操作名称，异常时记录到日志中</param>
    /// <param name="action">需要执行的逻辑过程</param>
    /// <param name="args">需要传递给后台线程的启动参数</param>
    public static void Run(string operatorName, Action<object> action, object args)
    {
        if( operatorName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(operatorName));
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        using( ExecutionContext.SuppressFlow() ) {
            ThreadPool.QueueUserWorkItem((x) => {
                try {
                    action.Invoke(x);
                }
                catch( Exception ex ) {
                    HandleException(operatorName, ex);
                }
            }, args);
        }
    }


    /// <summary>
    /// 开启一个新的后台线程，并在其中执行 action 委托
    /// </summary>
    /// <param name="operatorName">操作名称，异常时记录到日志中</param>
    /// <param name="threadName">线程名称，必填</param>
    /// <param name="action">需要执行的逻辑过程</param>
    public static void Run2(string operatorName, string threadName, Action action)
    {
        if( operatorName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(operatorName));
        if( threadName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(threadName));
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        using( ExecutionContext.SuppressFlow() ) {
            Thread thread = new Thread(() => {
                try {
                    action.Invoke();
                }
                catch( Exception ex ) {
                    HandleException(operatorName, ex);
                }
            });
            thread.Name = threadName;
            thread.IsBackground = true;
            thread.Start();
        }
    }

    /// <summary>
    /// 开启一个新的后台线程，并在其中执行 action 委托
    /// </summary>
    /// <param name="operatorName">操作名称，异常时记录到日志中</param>
    /// <param name="threadName">线程名称，必填</param>
    /// <param name="action">需要执行的逻辑过程</param>
    /// <param name="args">需要传递给后台线程的启动参数</param>
    public static void Run2(string operatorName, string threadName, Action<object> action, object args)
    {
        if( operatorName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(operatorName));
        if( threadName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(threadName));
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        using( ExecutionContext.SuppressFlow() ) {
            Thread thread = new Thread(x => {
                try {
                    action.Invoke(x);
                }
                catch( Exception ex ) {
                    HandleException(operatorName, ex);
                }
            });
            thread.Name = threadName;
            thread.IsBackground = true;
            thread.Start(args);
        }
    }


    internal static void HandleException(string operatorName, Exception ex)
    {
        if( ClownFishInit.AppExitToken.IsCancellationRequested )
            return;

        // 注意：这个方法在【单元测试】环境中执行时，Console.Write 会不起作用（没任何输出），
        //      原因在于  ExecutionContext.SuppressFlow() 这个调用，
        //      所以在单元测试时，会修改 ExceptionHandler 这个委托 

        ExceptionHandler?.Invoke(operatorName, ex);
    }

    /// <summary>
    /// 当出现未处理异常时的处理方法
    /// </summary>
    public static Action<string, Exception> ExceptionHandler = LogException;
        

    internal static void LogException(string operatorName, Exception ex)
    {
        if( ClownFishOptions.ThreadUtilsShowErrorToConsole ) {
            Console2.Error(operatorName + " ERROR:", ex);
        }

        try {
            OprLog log = OprLog.CreateErrLog(ex);
            log.OprName = operatorName;
            log.Controller = nameof(ThreadUtils);

            LogHelper.Write(log);

            if( LoggingOptions.InvokeLogEnable ) {
                InvokeLog log2 = log.ToInvokeLog();
                LogHelper.Write(log2);
            }
        }
        catch(Exception ex2 ) {
            try {
                LogException2(operatorName, ex, ex2);
            }
            catch {  // 这里只能吃掉异常
            }
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogException2(string operatorName, Exception ex, Exception ex2)
    {
        string message = $@"ClownFish.Base.ThreadUtils has a new exception while handling an uncaught exception for {operatorName}：
{ex2.ToString()}
-------------------- original exception: --------------------
{ex.ToString()}
";
        Console2.Error(message);
    }



}
