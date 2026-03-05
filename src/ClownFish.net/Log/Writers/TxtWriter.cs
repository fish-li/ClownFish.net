namespace ClownFish.Log.Writers;

internal sealed class TxtWriter : FileWriter
{
    protected override string FileExtName => ".log";

    protected override bool NeedFlagLine => true;

    protected override ValueCounter WriteCounter => ClownFishCounters.Logging.TxtWriteCount;

    [UnconditionalSuppressMessage("Trimming", "IL2075: call type.GetProperties")]
    public override string ObjectToText(object obj)
    {
        PropertyInfo[] ps = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        if( ps.Length == 0 )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( PropertyInfo p in ps ) {
                object value = p.FastGetValue(obj);
                if( value == null )
                    continue;

                sb.AppendLine($"[{p.Name}]: {value.ToString2()}");
            }
            sb.Append("[--end--]");
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


}
