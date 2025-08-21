namespace ClownFish.Http.Utils;


/// <summary>
/// 用于记录写入字节长度的【透明写入】流，它本身不存储任何数据
/// </summary>
public sealed class TransparentOutStream : Stream
{
    private readonly Stream _stream;
    private long _outLen = 0;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="stream"></param>
    public TransparentOutStream(Stream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }

    /// <summary>
    /// 获取累计的写入长度
    /// </summary>
    /// <returns></returns>
    public long GetOutSize() => _outLen;

    /// <inheritdoc/>
    public override bool CanRead => false;   // 不支持

    /// <inheritdoc/>
    public override bool CanSeek => false;   // 不支持

    /// <inheritdoc/>
    public override bool CanWrite => _stream.CanWrite;

    /// <inheritdoc/>
    public override long Length => throw new NotImplementedException();

    /// <inheritdoc/>
    public override long Position {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void Flush()
    {
        _stream.Flush();
    }

    /// <inheritdoc/>
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _stream.FlushAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        _outLen += count;
        _stream.Write(buffer, offset, count);
    }
    /// <inheritdoc/>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        _outLen += count;
        return _stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

#if NETCOREAPP
    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _outLen += buffer.Length;
        _stream.Write(buffer);
    }

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        _outLen += buffer.Length;
        return _stream.WriteAsync(buffer, cancellationToken);
    }

#endif

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
        _outLen++;
        _stream.WriteByte(value);
    }

}
