namespace ClownFish.Log.Writers;

/// <summary>
/// 不做任何写入操作的Writer
/// </summary>
internal sealed class NullWriter : ILogWriter
{

    public void Init(LogConfiguration config, Type dataType)
    {
    }


    public void WriteList<T>(List<T> list) where T : class, IMsgObject
    {
        // 不执行写入操作
    }

}
