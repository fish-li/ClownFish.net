using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers;

internal class XFileWriter : FileWriter
{
    internal static readonly ValueCounter WriteCount = new ValueCounter("WriteCount");

    protected override string FileExtName => ".xlog";

    public int LastWriteResult { get; private set; }

    protected override ValueCounter WriteCounter => throw new NotImplementedException();

    public override void Write<T>(T info)
    {
        string line = XmlHelper.XmlSerialize(info, Encoding.UTF8);

        LastWriteResult = WriteToFile<T>(line, true);

        WriteCount.Increment();
    }

    public override string ObjectToText(object obj)
    {
        throw new NotImplementedException();
    }
}
