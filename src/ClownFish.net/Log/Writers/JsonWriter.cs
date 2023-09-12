namespace ClownFish.Log.Writers;

/// <summary>
/// 将日志记录到JSON文件的写入器
/// </summary>
internal sealed class JsonWriter : FileWriter
{
    protected override string FileExtName => ".json.log";

    protected override ValueCounter WriteCounter => ClownFishCounters.Logging.JsonWriteCount;

    /// <summary>
    /// 将对象转成要保存的文本
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override string ObjectToText(object obj)
    {
        return JsonExtensions.ToJson(obj);
    }

}
