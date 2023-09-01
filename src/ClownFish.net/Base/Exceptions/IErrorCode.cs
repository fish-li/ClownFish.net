namespace ClownFish.Base.Exceptions;

/// <summary>
/// 获取异常的状态码接口定义
/// </summary>
public interface IErrorCode
{
    /// <summary>
    /// 获取异常的状态码
    /// </summary>
    /// <returns></returns>
    int GetErrorCode();
}
