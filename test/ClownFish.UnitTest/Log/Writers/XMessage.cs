namespace ClownFish.UnitTest.Log.Writers;

public class XMessage : IMsgObject
{
    public object Message { get; private set; }


    public XMessage(object data)
    {
        this.Message = data;
    }


    public DateTime GetTime()
    {
        return DateTime.MinValue;
    }

    public string GetId()
    {
        return this.Message.GetHashCode().ToString();
    }
}


public class XMessage2 : XMessage
{
    public XMessage2(object data) : base(data)
    {
    }
}
