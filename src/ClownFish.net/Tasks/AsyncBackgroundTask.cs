namespace ClownFish.Tasks;

/// <summary>
/// 表示一个后台运行的任务，它的子类会在程序启动时自动创建并运行
/// </summary>
public abstract class AsyncBackgroundTask : BaseBackgroundTask
{
    internal async Task RunAsync()
    {
        this.Status = -1;
        this.LastStatus = -1;

        try {
            // 这个方法不做异常处理，因为有可能会包含一些初始化的操作。
            if( Init0() == false ) {
                this.Status = 2;
                return;
            }

            ClownFishInit.AppExitToken.Register(OnAppExit);

            if( this.CronValue.HasValue() ) {
                await RunByCronAsync();
            }
            else if( this.SleepSeconds.GetValueOrDefault() > 0 ) {
                await RunWithSleepSecondsAsync();
            }
            else {
                this.Status = 2;
                throw new InvalidCodeException("没有设置执行间隔属性：SleepSeconds 或者 CronValue ");
            }
        }
        catch( Exception ex ) {
            this.UnhandledException = ex;
            Console2.Error(ex);
            Console2.Info($"##### BackgroundTask {this.GetType().FullName} 出现未预期异常，它不会继续运行！");
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
        this.RunMode = 1;

        // Cron表达式的值不允许执行过程中修改，所以放在循环前只获取一次
        NbCronExpression cron = new NbCronExpression(this.CronValue);

        if( this.FirstRun ) {
            await Execute0Async();
            if( this.ExitFlag )
                return;
        }

        while( true ) {

            // cron表达式的场景下，第一次执行前必须要“等待”，因为触发时间可能是“每天12点”之类的定点时间。
            DateTime? nextTime = cron.GetNextLocalTime(DateTime.Now);
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
        DateTime now = DateTime.Now;

        this.Status = 1;
        this.LastStatus = 1;
        this.LastRunTime.Set(now.Ticks);

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

        if( this.RunMode == 1 && (DateTime.Now - now).TotalMilliseconds < 10 ) {

            // 一个很特殊场景（真实案例）：
            // 有个 “清理临时目录” 的作业，Cron表达式  "0 0/2 * * * ?"   2分钟运行一次

            // 作业某一次的【实际】触发时间为： 2024-12-30 14:09:59.999
            // 【应该】触发时间为： 2024-12-30 14:10:00.000

            // 实际执行时，由于临时目录为空（没有文件），作业立即完成！  作业的执行时间为0
            // 此时计算下一次的执行时间仍然为 2024-12-30 14:10:00.000，于是又立即执行，
            // 由于作业的执行时间为0，这个过程要持续很多次（50+）~~~~

            // 解决办法：在作业中增加一行 Thread.Sleep(100)
            // 虽然解决了这个问题，但是这个问题存在很长时间了，作者一直不看日志，根本发现不了这个问题！
            // 再者，用 “Thread.Sleep” 来解决BUG，感觉也很奇怪的~~~，所以还是在框架中解决

            Thread.Sleep(100);
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
            if( span1.TotalSeconds <= ClownFishOptions.AsyncBackgroundTask_WaitSeconds2 )
                await Wait0(span1);
            else
                // 否则，先等待 60 秒，下次循环时再计算剩余等待时间，再等待…………
                await Wait0(TimeSpan.FromSeconds(ClownFishOptions.AsyncBackgroundTask_WaitSeconds1));

#if NET6_0_OR_GREATER
            // 如果用户在界面点击了【立即执行】，此时需要立即结束等待
            if( _tokenSource.IsCancellationRequested )
                break;
#endif
        }

        CheckWakeTime(time);
    }

#if NET6_0_OR_GREATER
    private CancellationTokenSource _tokenSource = new CancellationTokenSource();


    private async Task Wait0(TimeSpan waitTime)
    {
        if( _tokenSource != null ) {

            // 实际运行中，后台任务被取消的【可能性】极低，所以为了节省资源，这里尽量重用 _tokenSource 对象
            if( _tokenSource.TryReset() == false ) {   // TryReset() 是 .NET 6 新增的
                _tokenSource.Dispose();
                _tokenSource = new CancellationTokenSource();
            }
        }

        try {
            await Task.Delay(waitTime, _tokenSource.Token);
        }
        catch( TaskCanceledException ) {
            // 到达等待超时时间，或者被 StopWait() 触发
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
#else   // .net framework 的项目，使用AsyncBackgroundTask的可能性就不高，所以就简单处理（不支持 取消 功能）

    private async Task Wait0(TimeSpan waitTime)
    {
        try {
            await Task.Delay(waitTime);   // 不支持取消
        }
        catch( TaskCanceledException ) {
            // 到达等待超时时间，或者被 StopWait() 触发
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
        // 不支持
    }
#endif



}
