namespace ClownFish.Log;

/// <summary>
/// 定义Action的功能性描述
/// </summary>

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ActionTitleAttribute : Attribute
{
    /// <summary>
    /// 当前方法的功能描述
    /// </summary>
    public string Name { get; set; }
}
