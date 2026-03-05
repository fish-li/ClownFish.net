namespace ClownFish.Log.Writers;

/// <summary>
/// 日志的持久化接口
/// </summary>
public interface ILogWriter
{
    /// <summary>
    /// 第一次触发写日志时的初始化动作，例如：检查数据库连接是否已配置
    /// </summary>
    /// <param name="config">整个日志组件的配置对象</param>
    /// <param name="dataType">使用此写入器的日志对象的数据类型</param>
    void Init(LogConfiguration config, Type dataType);



    /// <summary>
    /// 批量写入日志信息
    /// </summary>
    /// <typeparam name="T">消息的数据类型</typeparam>
    /// <param name="list">要写入的日志信息</param>
    void WriteList<T>(List<T> list) where T : class, IMsgObject;


}
