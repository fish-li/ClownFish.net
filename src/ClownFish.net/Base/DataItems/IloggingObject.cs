namespace ClownFish.Base;

/// <summary>
/// 定义一个包含日志输出方法的接口
/// </summary>
public interface ILoggingObject
{
    /// <summary>
    /// 获取当前对象的日志展示文本
    /// </summary>
    /// <returns></returns>
    string ToLoggingText();

}
