namespace ClownFish.Tasks;
#if NETCOREAPP
// 设计说明
// ===========================================
// BackgroundTask 不使用传统的调度模式，因为那种方式有以下缺点：
// 1，调度线程如果执行不及时（被阻塞或者没有获取CPU），会影响所有任务的【触发及时率】
// 2，任务在执行时，会进入线程池排队，它的【及时性】容易受线程池的影响，例如：线程池的线程不够用

// 所以，为了提高任务的【及时性】，BackgroundTask 采用独立的线程来执行。

// 补充说明：
// 1，AsyncBackgroundTask，使用线程池，而不是独立的线程，因此【及时性】不保证
// 2，如果对任务的【及时触发】要求不高，建议使用 AsyncBackgroundTask
// 3，AsyncBackgroundTask 支持较长的作业执行间隔，例如：一个月一次
// ------------------------------------------------------------------------------
// 4，BackgroundTask 不支持时间跨度太久的休眠间隔，因为那会是对线程资源的浪费！！
// 5，BackgroundTask，建议 仅用于 10 秒内的间隔任务



/// <summary>
/// 表示一个后台运行的任务，它的子类会在程序启动时自动创建并运行
/// </summary>
public abstract class BackgroundTask : BaseBackgroundTask
{
    internal void Run()
    {
        this.Status = 2;

        // 这个方法不做异常处理，因为有可能会包含一些初始化的操作。
        if( Init() == false )
            return;

        ClownFishInit.AppExitToken.Register(OnAppExit);

        if( this.CronValue.HasValue() ) {
            RunByCron();
        }
        else if( this.SleepSeconds.GetValueOrDefault() > 0 ) {
            RunWithSleepSeconds();
        }
        else {
            throw new InvalidCodeException("没有设置执行间隔属性：SleepSeconds 或者 CronValue ");
        }

        this.Status = 2;
    }

    private void RunWithSleepSeconds()
    {
        if( this.FirstRun ) {
            Execute0();
            if( this.ExitFlag )
                return;
        }

        while( true ) {

            // 属性值可以在运行时调整，所以放在循环中获取
            int sleepSeconds = this.SleepSeconds.GetValueOrDefault();
            if( sleepSeconds <= 0 ) {
                ShowWarnning("SleepSeconds 返回了一个无效值，任务就此结束执行。");
                return;
            }

            DateTime nextTime = DateTime.Now.AddSeconds(sleepSeconds);
            WaitUntil(nextTime);

            if( this.ExitFlag )
                return;

            Execute0();

            if( this.ExitFlag )
                return;
        }
    }

    private void RunByCron()
    {
        // Cron表达式的值不允许执行过程中修改，所以放在循环前只获取一次
        NbCronExpression cron = new NbCronExpression(this.CronValue);

        if( this.FirstRun ) {
            Execute0();
            if( this.ExitFlag )
                return;
        }

        while( true ) {

            // cron表达式的场景下，第一次执行前必须要“等待”，因为触发时间可能是“每天12点”之类的定点时间。
            DateTime? nextTime = cron.GetNextTime(DateTime.Now);
            if( nextTime.HasValue == false ) {
                ShowWarnning("不能根据 CronValue 计算下次执行时间，任务就此结束执行。");
                return;
            }

            WaitUntil(nextTime.Value);

            if( this.ExitFlag )
                return;

            Execute0();

            if( this.ExitFlag )
                return;
        }
    }


    private void Execute0()
    {
        this.Status = 1;
        this.LastStatus = 1;
        this.LastRunTime.Set(DateTime.Now.Ticks);

        try {
            using( this.Context = new BgTaskExecuteContext(this) ) {

                Exception lastEx = null;
                try {
                    this.ExecuteCount.Increment();
                    Execute();
                    this.LastStatus = 0;
                }
                catch( Exception ex ) {
                    lastEx = ex;
                    this.LastStatus = 2;
                    this.ErrorCount.Increment();
                    this.Context.SetException(ex);
                }

                if( lastEx != null ) {
                    OnError0(lastEx);
                }
            }
        }
        finally {
            this.Context = null;
            this.Status = 0;            
        }
    }


   

    /// <summary>
    /// 执行任务的主体过程。
    /// 说明：
    /// 1、如果需要多次调用，请【重写】Sleep()方法，休眠一段时间，并且不要调用默认行为。
    /// 2、当前方法在执行时，框架会做异常捕获，在异常时会调用 OnError方法。
    /// </summary>
    public abstract void Execute();


    /// <summary>
    /// 阻塞当前任务线程，直到某个时刻为止。
    /// 在等待的过程中，可以调用 StopWait() 方法提前结束等待。
    /// </summary>
    /// <param name="time"></param>
    private void WaitUntil(DateTime time)
    {
        this.NextRunTime.Set(time.Ticks);

        TimeSpan span1 = time - DateTime.Now;
        if( span1.TotalMilliseconds <= 0 )
            return;

        // 注意：同步版本不支持时间跨度太久的休眠间隔。
        // 原因：这样做太浪费线程了！！

        _manualResetEvent.Reset();
        _manualResetEvent.Wait(span1);

        CheckWakeTime(time);
    }

    private readonly ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim(false);

    internal override void StopWait()
    {
        _manualResetEvent.Set();
    }

}
#endif
