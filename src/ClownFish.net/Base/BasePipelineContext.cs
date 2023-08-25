namespace ClownFish.Base;

/// <summary>
/// 管道上下文基类
/// </summary>
public abstract class BasePipelineContext
{
    /// <summary>
    /// 操作ID，唯一不重复。对于有重试类操作，每次操作或者重试都生成一个ID
    /// </summary>
    public string ProcessId { get; } = Guid.NewGuid().ToString("N");


    // 下面2个时间属性用于各类日志中的时间取值保持一致，
    // 反之如果各个日志各自取DateTime.Now得到的结果会有偏差，而且这种误差很难重现和排查！

    /// <summary>
    /// 代码执行主体(含框架部分)的开始执行时间
    /// </summary>
    public DateTime StartTime { get; } = DateTime.Now;
    /// <summary>
    /// 代码执行主体(含框架部分)的结束执行时间
    /// </summary>
    public DateTime EndTime { get; private set; }


    /// <summary>
    /// 性能阀值。
    /// 如果大于零则判断是否超过性能阀值-IsSlow，否则不做判断。
    /// </summary>
    public long PerformanceThresholdMs { get; set; }


    /// <summary>
    /// 标记当前操作是不是一个【长任务】，
    /// 长任务不统计SUM服务的【总执行时间-SumTime】，因此也不计入服务的【平均响应时间-AvgTime】，对于每次请求仍然会计算【执行时间-ExecuteTime】
    /// OprLog不会关注这个属性。
    /// </summary>
    internal bool IsLongTask { get; private set; }
    

    /// <summary>
    /// 最近产生的异常
    /// </summary>
    public Exception LastException { get; private set; }



    /// <summary>
    /// OprLogScope 实例。
    /// 当日志开关不启用时，此属性为 NULL
    /// </summary>
    public OprLogScope OprLogScope { get; private set; }


    internal void SetOprLogScope(OprLogScope scope)
    {
        if( scope == null )
            throw new ArgumentNullException(nameof(scope));

        this.OprLogScope = scope;
    }


    internal void DisposeOprLogScope()
    {
        if( this.OprLogScope != null ) {
            (this.OprLogScope as IDisposable).Dispose();
            this.OprLogScope = null;
        }
    }
    /// <summary>
    /// 将当前操作标记为【长任务】
    /// </summary>
    public void SetAsLongTask()
    {
        this.IsLongTask = true;
        this.PerformanceThresholdMs = 0;
    }

    /// <summary>
    /// 记录发生的异常
    /// </summary>
    /// <param name="ex"></param>
    public void SetException(Exception ex)
    {
        if( ex != null ) {
            this.LastException = ex;
        }
    }
    /// <summary>
    /// 清除最近产生的异常
    /// </summary>
    public void ClearErrors()
    {
        this.LastException = null;
    }

    /// <summary>
    /// 标记管道执行已结束
    /// </summary>
    public void End()
    {
        this.EndTime = DateTime.Now;
    }


    /// <summary>
    /// 获取当前操作的执行状态
    /// </summary>
    public virtual int GetStatus()
    {
        return this.LastException == null ? 200 : this.LastException.GetErrorCode();
    }

    /// <summary>
    /// 获取当前操作的简要描述
    /// </summary>
    /// <returns></returns>
    public virtual string GetTitle() => string.Empty;

    /// <summary>
    /// 获取当前操作的请求数据
    /// </summary>
    /// <returns></returns>
    public virtual object GetRequest() => null;


}
