namespace ClownFish.Http.Utils;

/// <summary>
/// FileStream对临时文件的封装，在流关闭时会自动删除文件
/// </summary>
public sealed class TempFileStream : Stream
{
    private string _filePath;
    private readonly FileStream _stream;

    /// <summary>
    /// 临时文件的保存路径
    /// </summary>
    public string FilePath => _filePath;

    /// <summary>
    /// 构造方法
    /// </summary>
    public TempFileStream()
    {
        _filePath = TempFile.GenTempFileFullName(".dat");
        _stream = RetryFile.Create(_filePath);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if( _filePath != null ) {
            _stream.Close();

            TempFile.DeleteFile(_filePath);
            _filePath = null;
        }
    }

    /// <inheritdoc/>
    public override bool CanRead => _stream.CanRead;

    /// <inheritdoc/>
    public override bool CanSeek => _stream.CanSeek;

    /// <inheritdoc/>
    public override bool CanWrite => _stream.CanWrite;

    /// <inheritdoc/>
    public override long Length => _stream.Length;

    /// <inheritdoc/>
    public override long Position {
        get => _stream.Position;
        set => _stream.Position = value;
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
        return _stream.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _stream.ReadAsync(buffer, offset, count, cancellationToken);
    }


    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }


    /// <inheritdoc/>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

#if NETCOREAPP

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stream.WriteAsync(buffer, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stream.ReadAsync(buffer, cancellationToken);
    }

    /// <inheritdoc/>
    public override void CopyTo(Stream destination, int bufferSize)
    {
        base.CopyTo(destination, bufferSize);
    }
#endif

    /// <inheritdoc/>
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return _stream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    

}


