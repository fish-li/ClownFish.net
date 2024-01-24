namespace ClownFish.Log.Writers;

/// <summary>
/// 日志的持久化接口
/// </summary>
public interface ILogWriter
{
    /// <summary>
    /// 第一次触发写日志时的初始化动作，例如：检查数据库连接是否已配置
    /// </summary>
    /// <param name="config"></param>
    /// <param name="section"></param>
    void Init(LogConfiguration config, WriterConfig section);



    /// <summary>
    /// 批量写入日志信息
    /// </summary>
    /// <typeparam name="T">消息的数据类型</typeparam>
    /// <param name="list">要写入的日志信息</param>
    void WriteList<T>(List<T> list) where T : class, IMsgObject;


}
