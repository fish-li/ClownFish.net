namespace ClownFish.MQ.Pipeline;
#if NET6_0_OR_GREATER

/// <summary>
/// 消息处理器基类。
/// </summary>
/// <remarks>
/// 说明：
/// 1、此类型的实例随队列订阅者一直存在，基本上与进程的生命周期一样长。
/// 2、继承类型中所有数据成员也会在每个消息处理时持续可用，类似于缓存效果，但由于有实例隔离，会比静态成员安全。
/// 3、继承类型的实例化由框架来完成，自行实例化是没有意义的。
/// </remarks>
/// <typeparam name="T">消息的数据类型参数</typeparam>
public abstract class BaseMessageHandler<T> : BaseMessageHandlerObject<T> where T : class
{
    /// <summary>
    /// 验证消息的数据是否有效
    /// </summary>
    /// <param name="context"></param>
    public virtual void ValidateMessage(PipelineContext<T> context)
    {
        // do nothing
    }

    /// <summary>
    /// 完善消息，可以做一些字段的补充
    /// </summary>
    /// <param name="context"></param>
    public virtual void PrepareMessage(PipelineContext<T> context)
    {
        // do nothing
    }

    /// <summary>
    /// 保存消息
    /// </summary>
    /// <param name="context"></param>
    public virtual void SaveMessage(PipelineContext<T> context)
    {
        // do nothing
    }

    /// <summary>
    /// 处理消息，例如做告警分析
    /// </summary>
    /// <param name="context">管道上下文</param>
    public virtual void ProcessMessage(PipelineContext<T> context)
    {
        // do nothing
    }


    /// <summary>
    /// 在调用  ProcessMessage(...) 【后】执行
    /// </summary>
    /// <param name="context"></param>
    public virtual void AfterProcess(PipelineContext<T> context)
    {
        // do nothing
    }

    /// <summary>
    /// 在调用  AfterProcess(...) 【后】执行， 用于保存一些中间状态
    /// </summary>
    /// <param name="context"></param>
    public virtual void SaveState(PipelineContext<T> context)
    {
        // do nothing
    }

    /// <summary>
    /// 在结束管道时调用。
    /// 不管发生什么情况，一定会调用。
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnEnd(PipelineContext<T> context)
    {
        // do nothing
    }


    /// <summary>
    /// 在处理过程中出现异常时调用。
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnError(PipelineContext<T> context)
    {
        // do nothing
    }



}


#endif
