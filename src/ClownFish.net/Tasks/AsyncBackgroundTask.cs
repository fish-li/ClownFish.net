namespace ClownFish.Tasks;
#if NETCOREAPP
/// <summary>
/// 表示一个后台运行的任务，它的子类会在程序启动时自动创建并运行
/// </summary>
public abstract class AsyncBackgroundTask : BaseBackgroundTask
{
    private static readonly int s_waitSecond60 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds1", 60);
    private static readonly int s_waitSecond66 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds2", 66);

    internal async Task RunAsync()
    {
        this.Status = 2;

        // 这个方法不做异常处理，因为有可能会包含一些初始化的操作。
        if( Init() == false )
            return;

        ClownFishInit.AppExitToken.Register(OnAppExit);

        if( this.CronValue.HasValue() ) {
            await RunByCronAsync();
        }
        else if( this.SleepSeconds.GetValueOrDefault() > 0 ) {
            await RunWithSleepSecondsAsync();
        }
        else {
            throw new InvalidCodeException("没有设置执行间隔属性：SleepSeconds 或者 CronValue ");
        }

        this.Status = 2;
    }


    private async Task RunWithSleepSecondsAsync()
    {
        if( this.FirstRun ) {
            await Execute0Async();
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
            await WaitUntilAsync(nextTime);

            if( this.ExitFlag )
                return;

            await Execute0Async();

            if( this.ExitFlag )
                return;
        }
    }

    private async Task RunByCronAsync()
    {
        // Cron表达式的值不允许执行过程中修改，所以放在循环前只获取一次
        NbCronExpression cron = new NbCronExpression(this.CronValue);

        if( this.FirstRun ) {
            await Execute0Async();
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

            await WaitUntilAsync(nextTime.Value);

            if( this.ExitFlag )
                return;

            await Execute0Async();

            if( this.ExitFlag )
                return;
        }
    }


    private async Task Execute0Async()
    {
        this.Status = 1;
        this.LastStatus = 1;
        this.LastRunTime.Set(DateTime.Now.Ticks);

        try {
            using( this.Context = new BgTaskExecuteContext(this) ) {

                Exception lastEx = null;
                try {
                    this.ExecuteCount.Increment();
                    await ExecuteAsync();
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
    public abstract Task ExecuteAsync();


    /// <summary>
    /// 阻塞当前任务线程，直到某个时刻为止。
    /// 在等待的过程中，可以调用 StopWait() 方法提前结束等待。
    /// </summary>
    /// <param name="time"></param>
    private async Task WaitUntilAsync(DateTime time)
    {
        this.NextRunTime.Set(time.Ticks);

        // 有可能一次等待的时间太长，例如：通过cron表达式配置的 【一个月执行一次】 的作业
        // 此时等待时间就超出 Task.Delay 允许的时间跨度
        while( true ) {
            TimeSpan span1 = time - DateTime.Now;

            // 有可能已经到达指定的时间了
            if( span1.TotalMilliseconds <= 0 )
                break;

            // 如果等待时间不是特别长，就使用这个【时间参数】去等待
            if( span1.TotalSeconds <= s_waitSecond66 )
                await Wait0(span1);
            else
                // 否则，先等待 60 秒，下次循环时再计算剩余等待时间，再等待…………
                await Wait0(TimeSpan.FromSeconds(s_waitSecond60));

            // 如果用户在界面点击了【立即执行】，此时需要立即结束等待
            if( _tokenSource.IsCancellationRequested )
                break;
        }

        CheckWakeTime(time);
    }

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    private async Task Wait0(TimeSpan waitTime)
    {
        if( _tokenSource != null ) {
            if( _tokenSource.TryReset() == false ) {   // TryReset() 是 .NET 6 新增的
                _tokenSource.Dispose();
                _tokenSource = new CancellationTokenSource();
            }
        }

        try {
            await Task.Delay(waitTime, _tokenSource.Token);
        }
        catch( TaskCanceledException ) {
            // 到达等待超时时间
        }
        catch( Exception ex ) {
            if( EnvUtils.IsDevEnv )
                Console2.Error(ex);
        }
    }

    /// <summary>
    /// 如果用户在界面点击了【立即执行】，将会调用此方法。
    /// </summary>
    internal override void StopWait()
    {
        try {
            // 发出一个【取消(停止)等待】的信号
            _tokenSource.Cancel();
        }
        catch( Exception ex ) {
            if( EnvUtils.IsDevEnv )
                Console2.Error(ex);

            // 有可能在调用当前方法时，另外一个线程正在执行 _tokenSource.Dispose();
            // 所有这里吃掉所有异常。
        }
    }

}
#endif
