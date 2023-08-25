namespace ClownFish.Data;

/// <summary>
/// 表示一个SQL片段，用于和CPQuery一起拼接
/// </summary>
public sealed class SqlFragment
{
    /// <summary>
    /// 对象的值
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="text"></param>
    public SqlFragment(string text)
    {
        if( string.IsNullOrEmpty(text) )
            throw new ArgumentNullException("text");

        this.Value = text;
    }

    /// <summary>
    /// 重写ToString方法
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.Value;
    }
}
