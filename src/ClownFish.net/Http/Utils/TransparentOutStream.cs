namespace ClownFish.Http.Utils;


/// <summary>
/// 用于记录写入字节长度的【透明写入】流，它本身不存储任何数据
/// </summary>
public sealed class TransparentOutStream : Stream
{
    private readonly Stream _stream;
    private readonly OprLog _oprlog;
    private long _outLen = 0;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="oprLog"></param>
    public TransparentOutStream(Stream stream, OprLog oprLog = null)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _oprlog = oprLog;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // 使用这个类的场景都会产生 Transfer-Encoding: chunked 的输出，导致日志组件不能获取响应的长度，所以这里直接指定
        if( _oprlog != null ) {
            _oprlog.OutSize = this.GetOutSize();
        }
    }

    //#if DEBUG
    //    private bool _disposed;
    //    internal bool IsDisposed => _disposed;

    //    /// <inheritdoc/>
    //    protected override void Dispose(bool disposing)
    //    {
    //        _disposed = true;
    //    }
    //#endif

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

}
