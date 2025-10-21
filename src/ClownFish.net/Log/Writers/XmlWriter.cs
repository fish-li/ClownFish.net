
namespace ClownFish.Log.Writers;

internal sealed class XmlWriter : FileWriter
{
    protected override string FileExtName => ".xml.log";

    protected override ValueCounter WriteCounter => ClownFishCounters.Logging.XmlWriteCount;

#if NETCOREAPP
    [UnconditionalSuppressMessage("Trimming", "IL2026: XmlSerialize")]
#endif
    public override string ObjectToText(object obj)
    {
        return XmlHelper.XmlSerialize(obj, Encoding.UTF8);
    }


}
