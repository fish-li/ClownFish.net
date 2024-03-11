namespace ClownFish.Http.MockTest;

/// <summary>
/// 
/// </summary>
public class MockHttpResponse : NHttpResponse
{
    private bool _hasStarted = false;
    private readonly MemoryStream _stream = new MemoryStream();

    /// <summary>
    /// 
    /// </summary>
    public readonly NameValueCollection OutHeaders = new NameValueCollection();

    /// <summary>
    /// 
    /// </summary>
    public readonly NameValueCollection OutCookies = new NameValueCollection();


    /// <summary>
    /// 
    /// </summary>
    public override Stream OutputStream => _stream;

    internal MockHttpResponse(MockHttpContext context) :base(context)
    {

    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override object OriginalHttpResponse => null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override int StatusCode { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ContentType {
        get {
            return this.OutHeaders.Get("Content-Type");
        }
        set {
            this.SetHeader("Content-Type", value, true);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override Encoding ContentEncoding { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool HasStarted => _hasStarted;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override long ContentLength { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void ClearHeaders()
    {
        this.OutCookies.Clear();
        this.OutHeaders.Clear();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string GetResponseAsText()
    {
        if( this.OutputStream.Length > 0 )
            return this.OutputStream.ToArray().ToUtf8String();

        return string.Empty;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Close()
    {
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void SetCookie2(string name, string value, DateTime? expires = null)
    {
        this.OutCookies.Add(name, value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool SetHeader(string name, string value, bool ifExistThenIgnore)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        this.OutHeaders.Add(name, value);
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetHeader(string name)
    {
        string[] values = this.OutHeaders.GetValues(name);
        if( values == null )
            return null;
        else
            return string.Join(", ", values);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool RemoveHeader(string name)
    {
        this.OutHeaders.Remove(name);
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool SetHeaders(string name, string[] values, bool ifExistThenIgnore)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        if( values.HasValue() ) {
            foreach( var x in values )
                this.OutHeaders.Add(name, x);
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    public void Write(string text)
    {
        if( text.IsNullOrEmpty() )
            return;

        _hasStarted = true;

        byte[] bb = Encoding.UTF8.GetBytes(text);
        _stream.Write(bb, 0, bb.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    public override void Write(byte[] buffer)
    {
        if( buffer.IsNullOrEmpty() )
            return;

        _hasStarted = true;

        _stream.Write(buffer, 0, buffer.Length);
    }


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void WriteAll(byte[] buffer)
    {
        Write(buffer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Task WriteAsync(string text)
    {
        this.Write(text);

        return Task.CompletedTask;
    }


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override Task WriteAsync(byte[] buffer)
    {
        this.Write(buffer);

        return Task.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override Task WriteAllAsync(byte[] buffer)
    {
        return WriteAsync(buffer);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetAllHeaders()
    {
        foreach( var name in this.OutHeaders.AllKeys ) {
            string[] values = this.OutHeaders.GetValues(name);
            yield return new KeyValuePair<string, IEnumerable<string>>(name, values);
        }
    }


}
