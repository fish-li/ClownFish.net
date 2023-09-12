namespace ClownFish.MQ.Pipeline;
#if NETCOREAPP

/// <summary>
/// BaseMessageHandler/AsyncBaseMessageHandler 的共同基类
/// </summary>
public abstract class BaseMessageHandlerObject<T> where T : class
{
    /// <summary>
    /// 是否启用日志（InvokeLog+OprLog），默认值：true
    /// </summary>
    public virtual bool EnableLog => ClownFish.Log.LoggingOptions.MessageHandlerEnableLog;

    /// <summary>
    /// 管道初始化时调用。
    /// 说明：此方法有可能会被多次调用。 
    /// </summary>
    public virtual void OnInit()
    {
        // do nothing
    }



    /// <summary>
    /// 【请求】结束管道处理过程
    /// </summary>
    protected void EndProcess()
    {
        throw new AbortRequestException();
    }


    /// <summary>
    /// 当出现异常时，判断是否需要重试。
    /// 默认行为：针对一些明确的异常会返回false 表示不启用重试。
    /// </summary>
    /// <param name="context"></param>
    /// <returns>返回 false 表示异常不需要重试，返回 true 表示异常需要重试。</returns>
    public virtual bool IsNeedRetry(PipelineContext<T> context)
    {
        return PipelineUtils.ExceptionIsNeedRetry(context.LastException);
    }


    /// <summary>
    /// 处理【无法处理】的死消息。
    /// 默认行为：将消息写入 temp\deadmsg 目录下，每个消息一个文件。
    /// </summary>
    /// <param name="context">管道上下文</param>
    public virtual void ProcessDeadMessage(PipelineContext<T> context)
    {
        DeadMessageUtils.HandlerDeadMessage(context.MessageData);
    }

}
#endif
