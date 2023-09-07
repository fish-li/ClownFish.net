namespace ClownFish.WebApi;

/// <summary>
/// 指示某个类型需要包含一个HttpContext实例，
/// 实现这个接口的HTTP处理类型（Service/Controller），框架将会自动给HttpContext属性赋值。
/// </summary>
public interface IRequireHttpContext
{
    /// <summary>
    /// 一个HttpContext实例。
    /// </summary>
    NHttpContext NHttpContext { get; set; }


}
