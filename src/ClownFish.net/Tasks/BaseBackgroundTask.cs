namespace ClownFish.Tasks;
#if NET6_0_OR_GREATER
/// <summary>
/// BackgroundTask基类，此类型仅供框架内部使用。
/// </summary>
public abstract class BaseBackgroundTask : BaseTaskObject
{
    internal bool ExitFlag { get; private set; } = false;

    /// <summary>
    /// 任务的执行次数
    /// </summary>
    internal ValueCounter ExecuteCount = new ValueCounter("ExecuteCount");

    /// <summary>
    /// 任务执行失败的次数
    /// </summary>
    internal ValueCounter ErrorCount = new ValueCounter("ErrorCount");


    /// <summary>
    /// 运行状态。 0: 等待中，1：运行中，2：已停止
    /// </summary>
    internal volatile int Status;        

    /// <summary>
    /// 最近运行状态。 0：成功， 1：运行中，2：失败
    /// </summary>
    internal volatile int LastStatus;

    

    /// <summary>
    /// 最近启动时间
    /// </summary>
    internal ValueCounter LastRunTime = new ValueCounter();


    /// <summary>
    /// 下次启动时间
    /// </summary>
    internal ValueCounter NextRunTime = new ValueCounter();


    /// <summary>
    /// BgTaskExecuteContext
    /// </summary>
    public BgTaskExecuteContext Context { get; internal set; }


    /// <summary>
    /// true表示需要在启动任务线程后立即执行一次，false表示启动后等到到达执行周期才会执行。
    /// 默认值：false
    /// </summary>
    public virtual bool FirstRun => false;

    /// <summary>
    /// 获取休眠秒数，用于描述“周期任务”的间隔时间，例如：每5秒执行一次。
    /// 注意：同步版本不支持时间跨度太久的休眠间隔。
    /// </summary>
    public virtual int? SleepSeconds {
        get => null;
    }

    /// <summary>
    /// 获取一个Cron表达式，用于描述“周期任务”的间隔时间。
    /// 这里使用的是 Quartz 支持的 Cron 格式，在线工具：https://www.pppet.net/
    /// 注意：同步版本不支持时间跨度太久的休眠间隔。
    /// </summary>
    public virtual string CronValue {
        get => null;
    }



    /// <summary>
    /// 执行任务前的初始化。
    /// 说明：执行当前方法时框架不做异常处理，如果产生异常会导致进程崩溃。
    /// </summary>
    /// <returns>如果 return false, 表示初始化失败，将中止任务</returns>
    public virtual bool Init()
    {
        return true;
    }


    internal void OnError0(Exception ex)
    {
        try {
            OnError(ex);
        }
        catch( Exception ex2 ) {
            Console2.Error(ex);
            Console2.Error(ex2);
        }
    }

    /// <summary>
    /// 异常处理方法。
    /// 默认行为：如果启用日志就不做任何处理，否则输出到Console
    /// </summary>
    /// <param name="ex"></param>
    public virtual void OnError(Exception ex)
    {
        if( this.EnableLog == false )
            Console2.Error(ex);
    }

    /// <summary>
    /// 请求结束后台任务的执行
    /// </summary>
    protected void ExitTask()
    {
        this.ExitFlag = true;
    }


    /// <summary>
    /// 检查等待后的唤醒时间是否与期望存在差距
    /// </summary>
    /// <param name="time"></param>
    protected void CheckWakeTime(DateTime time)
    {
        TimeSpan span2 = DateTime.Now - time;
        if( span2.TotalMilliseconds > 2000 ) {
            ShowWarnning("任务唤醒的时间晚于指定时间，共延迟：" + span2.ToString());
        }
    }

    internal void ShowWarnning(string message)
    {
        Console2.Warnning($"[{this.GetType().Name}] {message}");
    }



    /// <summary>
    /// 提前结束等待，立即执行任务
    /// </summary>
    internal virtual void StopWait()
    {
        //throw new NotImplementedException();
    }


    internal void OnAppExit()
    {
        this.ExitTask();
        this.StopWait();

        Console2.WriteLine("Application exit, stop BackgroundTask: " + this.GetType().FullName);
    }
}
#endif
