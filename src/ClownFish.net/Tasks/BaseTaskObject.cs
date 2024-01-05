namespace ClownFish.Tasks;
#if NETCOREAPP
/// <summary>
/// 所有后台任务的基类
/// </summary>
public abstract class BaseTaskObject
{
    private readonly bool? _enableLogValue;

    /// <summary>
    /// ctor
    /// </summary>
    public BaseTaskObject()
    {
        _enableLogValue = GetEnableLogValue(this.GetType());
    }

    /// <summary>
    /// 是否需要在每次执行Execute方法时自动生成日志(OprLog + InvokeLog)
    /// 默认值：true
    /// </summary>
    public virtual bool EnableLog => _enableLogValue.HasValue ? _enableLogValue.Value : ClownFish.Log.LoggingOptions.BackgroundTaskEnableLog;

    internal static bool? GetEnableLogValue(Type type)
    {
        string configName = type.GetType().FullName.Replace('.', '_') + "_EnableLog";
        string value = LocalSettings.GetSetting(configName);

        // 如果已明确配置 type_fullname_EnableLog，那么最终结果以配置参数为准，
        // 否则，最终结果取 ClownFish.Log.LoggingOptions.BackgroundTaskEnableLog
        if( value.IsNullOrEmpty() )
            return null;
        else
            return value.TryToBool();
    }
}
#endif
