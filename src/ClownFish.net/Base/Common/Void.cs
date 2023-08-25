namespace ClownFish.Base;

/// <summary>
/// 表示不需要具体返回结果的类型
/// </summary>
public sealed class Void
{
    private Void() { }

    /// <summary>
    /// 一个静态的单例
    /// </summary>
    public static readonly Void Value = new Void();
}
