namespace ClownFish.Log.Attributes;

/// <summary>
/// 定义Controller类型的功能性描述
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ControllerTitleAttribute : Attribute
{
    /// <summary>
    /// 所属功能模块的功能描述
    /// </summary>
    public string Module { get; set; }

    /// <summary>
    /// 当前Controller的功能描述
    /// </summary>
    public string Name { get; set; }
}
