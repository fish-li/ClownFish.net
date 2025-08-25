namespace ClownFish.Base;

/// <summary>
/// 二进制数组的内存列表
/// </summary>
public struct BytesList
{
    /// <summary>
    /// 表示一个 "\n" 字符的字节数组
    /// </summary>
    internal static readonly byte[] LnBytes = new byte[] { (byte)'\n' };

    private readonly List<byte[]> _items = new List<byte[]>(10);

    /// <summary>
    /// ctor
    /// </summary>
    public BytesList() { }

    /// <summary>
    /// 写入一个数组
    /// </summary>
    /// <param name="data"></param>
    public void Write(byte[] data)
    {
        if( data == null || data.Length == 0 )
            return;

        _items.Add(data);
    }

    /// <summary>
    /// 将 int 转 byte[]，再写入
    /// </summary>
    /// <param name="value"></param>
    public void Write(int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        _items.Add(bytes);
    }

    /// <summary>
    /// 写入一个换行的二进制数据
    /// </summary>
    public void WriteLn()
    {
        _items.Add(LnBytes);
    }

    /// <summary>
    /// 获取数据总长
    /// </summary>
    /// <returns></returns>
    public int GetSumLength()
    {
        int sumLen = 0;

        foreach( byte[] item in _items ) {
            sumLen += item.Length;
        }
        return sumLen;
    }

    /// <summary>
    /// 将所有写入数据合并在一起返回。  ###注意：此方法只能调用一次。第二次调用没有结果！
    /// </summary>
    /// <returns></returns>
    public byte[] ToArray()
    {
        int sumLen = GetSumLength();

        byte[] buffer = new byte[sumLen];
        int offset = 0;

        foreach( byte[] data in _items ) {
            Array.Copy(data, 0, buffer, offset, data.Length);
            offset += data.Length;
        }

        _items.Clear();   // 防止多次调用，强制纠正低效用法
        return buffer;
    }

    /// <summary>
    /// 将所有内部数据复制到指定的流对象中，并清空内部数据
    /// </summary>
    /// <param name="stream"></param>
    public int CopyToStream(Stream stream)
    {
        if( stream == null )
            throw new ArgumentNullException(nameof(stream));

        int count = 0;

        foreach( byte[] data in _items ) {
            stream.Write(data, 0, data.Length);
            count += data.Length;
        }

        _items.Clear();   // 防止多次调用，强制纠正低效用法
        return count;
    }

    /// <summary>
    /// 将内部压缩到指定的数据流中
    /// </summary>
    /// <param name="stream"></param>
    public int GzipToStream(Stream stream)
    {
        if( stream == null )
            throw new ArgumentNullException(nameof(stream));

        int sumLen = GetSumLength();
        if( sumLen == 0 )
            return 0;

#if NET6_0_OR_GREATER
        CompressionLevel level = CompressionLevel.SmallestSize;
#else
        CompressionMode level = CompressionMode.Compress;
#endif
        using( GZipStream gZipStream = new GZipStream(stream, level, true) ) {
            this.CopyToStream(gZipStream);
        }

        return sumLen;
    }

    /// <summary>
    /// 将内部数据压缩并返回
    /// </summary>
    /// <returns></returns>
    public byte[] ToGzip()   // 调用方：TxClient，Nebula.FoxFairy
    {
        int sumLen = GetSumLength();
        if( sumLen == 0 )
            return Empty.Array<byte>();

        using( MemoryStream resultStream = MemoryStreamPool.GetStream() ) {
            GzipToStream(resultStream);
            return resultStream.ToArray();
        }
    }


}
