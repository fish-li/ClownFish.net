namespace ClownFish.Log.Writers;

internal sealed class XmlWriter : FileWriter
{
    protected override string FileExtName => ".xml.log";

    protected override bool NeedFlagLine => true;

    protected override ValueCounter WriteCounter => ClownFishCounters.Logging.XmlWriteCount;

    public override string ObjectToText(object obj)
    {
        return XmlHelper.XmlSerializerObject(obj);
    }


}

