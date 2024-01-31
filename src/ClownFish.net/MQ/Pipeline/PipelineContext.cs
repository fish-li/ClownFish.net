namespace ClownFish.MQ.Pipeline;
#if NETCOREAPP

/// <summary>
/// 消息处理管道上下文对象
/// </summary>
public class PipelineContext<T> : BasePipelineContext, IDisposable where T : class
{
    /// <summary>
    /// MqRequest Object
    /// </summary>
    public MqRequest Request { get; init; }

    internal bool IsAsync { get; init; }

    internal BaseMessageHandlerObject<T> Handler { get; init; }


    /// <summary>
    /// 第 X 次重试执行，从零开始计数。
    /// </summary>
    public int RetryN { get; init; }


    /// <summary>
    /// 消息的实体对象
    /// </summary>
    public T MessageData => (T)this.Request.MessageObject;


    /// <summary>
    /// 默认行为：返回消息对象
    /// </summary>
    /// <returns></returns>
    public override object GetRequest()
    {
        return this.Request.MessageObject;
    }


    /// <summary>
    /// 构造方法（设置一些田默认值）
    /// </summary>
    internal PipelineContext(MqRequest request, BaseMessageHandlerObject<T> handler, bool isAsync, int retryN)
    {
        this.PerformanceThresholdMs = ClownFish.Log.LogConfig.Instance.Performance.HandleMessage;

        this.Request = request;
        this.Handler = handler;
        this.IsAsync = isAsync;
        this.RetryN = retryN;

        this.CreateOprLogScope();

        ClownFishCounters.Concurrents.MessageConcurrent.Increment();
    }

    void IDisposable.Dispose()
    {
        this.End();

        ClownFishCounters.Concurrents.MessageConcurrent.Decrement();

        ClownFishCounters.ExecuteTimes.MessageCount.Increment();

        if( this.LastException != null ) {
            ClownFishCounters.ExecuteTimes.MessageError.Increment();
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
        string typeName = this.Handler.GetType().Name;
        string flag = this.IsAsync ? "async" : "sync";
        return $"msg://{this.Request.MqKind}/{flag}/{typeName}";
    }

   
}

#endif





