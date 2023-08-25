namespace ClownFish.Log.Writers;

/// <summary>
/// 将日志记录到JSON文件的写入器。采用缩进方式写入JSON，方便阅读。
/// </summary>
internal sealed class Json2Writer : FileWriter
{
    protected override string FileExtName => ".json2.log";

    protected override ValueCounter WriteCounter => ClownFishCounters.Logging.Json2WriterCount;

    /// <summary>
    /// 将对象转成要保存的文本
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override string ObjectToText(object obj)
    {
        return JsonExtensions.ToJson(obj, JsonStyle.Indented);
    }

}
