namespace ClownFish.Log.Writers;

/// <summary>
/// 不做任何写入操作的Writer
/// </summary>
internal sealed class NullWriter : ILogWriter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="section"></param>
    public void Init(LogConfiguration config, WriterConfig section)
    {
    }



    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void WriteList<T>(List<T> list) where T : class, IMsgObject
    {
        // 不执行写入操作
    }

}
