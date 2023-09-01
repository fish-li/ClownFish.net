namespace ClownFish.Log.Attributes;

/// <summary>
/// 指标某个消息类型在日志写入时【可以】做批次操作。
/// 这个标记只是一个建议设置，最终如何执行写入操作与具体的Writer有关。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class BatchWritableAttribute : Attribute
{

}
