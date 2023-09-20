namespace ClownFish.Tasks;
#if NETCOREAPP
/// <summary>
/// 所有后台任务的基类
/// </summary>
public abstract class BaseTaskObject
{
    /// <summary>
    /// 是否需要在每次执行Execute方法时自动生成日志(OprLog + InvokeLog)
    /// 默认值：true
    /// </summary>
    public virtual bool EnableLog => ClownFish.Log.LoggingOptions.BackgroundTaskEnableLog;
}
#endif
