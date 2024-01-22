namespace ClownFish.Log;

/// <summary>
/// 基本的 "消息/日志" 对象接口
/// </summary>
public interface IMsgObject
{
    /// <summary>
    /// 获取对象的 ID
    /// </summary>
    string GetId();

    /// <summary>
    /// 对象的创建时间
    /// </summary>
    DateTime GetTime();
}


/// <summary>
/// IMsgBeforeWrite
/// </summary>
public interface IMsgBeforeWrite
{
    /// <summary>
    /// 日志对象即将被写入到持久化之前
    /// </summary>
    void BeforeWrite();
}