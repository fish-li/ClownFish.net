namespace ClownFish.Http.MockTest;

/// <summary>
/// 
/// </summary>
public sealed class MockNetworkStream : Stream
{
    private readonly MemoryStream _stream;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="data"></param>
    public MockNetworkStream(byte[] data)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        _stream = new MemoryStream(data, false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool CanRead => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool CanSeek => false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool CanWrite => false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override long Length => throw new NotImplementedException();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Flush()
    {
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}
