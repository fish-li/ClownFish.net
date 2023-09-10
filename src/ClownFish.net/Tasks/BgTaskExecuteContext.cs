namespace ClownFish.Tasks;
#if NET6_0_OR_GREATER
/// <summary>
/// 与BackgroundTask相关的执行上下文对象
/// </summary>
public sealed class BgTaskExecuteContext : BasePipelineContext, IDisposable
{
    /// <summary>
    /// BaseTaskObject instance
    /// </summary>
    public BaseTaskObject Executor { get; init; }


    /// <summary>
    /// OprLog
    /// </summary>
    public OprLog OprLog => this.OprLogScope.OprLog;


    /// <summary>
    /// 构造方法
    /// </summary>
    public BgTaskExecuteContext(BaseTaskObject executor)
    {
        if( executor == null )
            throw new ArgumentNullException(nameof(executor));

        this.Executor = executor;

        // 后台作业通常会比较复杂，而且不同的作业差异巨大，没办法取一个统一的阀值
        // 如果某个作业需要做性能检测，可以单独指定这个属性
        this.PerformanceThresholdMs = 0;

        this.CreateOprLogScope();

        ClownFishCounters.Concurrents.BgTaskConcurrent.Increment();
    }


    void IDisposable.Dispose()
    {
        this.End();

        ClownFishCounters.Concurrents.BgTaskConcurrent.Decrement();

        ClownFishCounters.ExecuteTimes.BgTaskCount.Increment();

        if( this.LastException != null ) {
            ClownFishCounters.ExecuteTimes.BgTaskError.Increment();
        }

        if( this.OprLogScope.IsNull == false ) {
            // 记录日志(OprLog + InvokeLog)
            this.SaveLog();
            this.DisposeOprLogScope();
        }
    }


    /// <summary>
    /// GetTittle
    /// </summary>
    /// <returns></returns>
    public override string GetTitle()
    {
        string typeName = this.Executor.GetType().Name;
        return "task://" + typeName + "/" + this.ProcessId;
    }

    
}
#endif
