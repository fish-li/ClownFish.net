namespace ClownFish.Base;

/// <summary>
/// 获取缓冲区的工具类
/// </summary>
public struct ByteBuffer : IDisposable
{
    private readonly byte[] _buffer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    public ByteBuffer(int size)
    {
        if( size <= 0 )
            throw new ArgumentOutOfRangeException(nameof(size));

#if NETCOREAPP
        _buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(size);
#else
			_buffer = new byte[size];
#endif
    }

    /// <summary>
    /// 获取缓冲区
    /// </summary>
    public byte[] Buffer { get => _buffer; }


    /// <summary>
    /// 
    /// </summary>
    void IDisposable.Dispose()
    {
#if NETCOREAPP
        System.Buffers.ArrayPool<byte>.Shared.Return(_buffer, true);
#endif
    }


}
