namespace ClownFish.Log.Models;

/// <summary>
/// 定义消息在显示时如何简短的展示
/// </summary>
public interface ISubject
{
    /// <summary>
    /// 获取某个数据的标题
    /// </summary>
    string GetSubject();
}
