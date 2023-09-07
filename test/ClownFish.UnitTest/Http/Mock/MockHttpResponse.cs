namespace ClownFish.UnitTest.Http.Mock;

public class MockHttpResponse : NHttpResponse
{
    private bool _hasStarted = false;
    private readonly MemoryStream _stream = new MemoryStream();

    public readonly NameValueCollection OutHeaders = new NameValueCollection();
    public readonly NameValueCollection OutCookies = new NameValueCollection();

    public readonly StringBuilder OutText = new StringBuilder();
    public override Stream OutputStream => _stream;

    internal MockHttpResponse(MockHttpContext context) :base(context)
    {

    }
    public override object OriginalHttpResponse => null;

    public override int StatusCode { get; set; }
    public override string ContentType {
        get {
            return this.OutHeaders.Get("Content-Type");
        }
        set {
            this.SetHeader("Content-Type", value, true);
        }
    }
    public override Encoding ContentEncoding { get; set; }        

    public override bool HasStarted => _hasStarted;

    public override long ContentLength { get; set; }

    public override void ClearHeaders()
    {
        this.OutCookies.Clear();
        this.OutHeaders.Clear();
    }

    public string GetResponseAsText()
    {
        if( this.OutText.Length > 0 )
            return this.OutText.ToString();

        if( this.OutputStream.Length > 0 )
            return this.OutputStream.ToArray().ToUtf8String();

        return string.Empty;
    }

    public override void Close()
    {
    }

    public override void SetCookie2(string name, string value, DateTime? expires = null)
    {
        this.OutCookies.Add(name, value);
    }

    public override bool SetHeader(string name, string value, bool ignoreExist)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        this.OutHeaders.Add(name, value);
        return true;
    }

    public override bool RemoveHeader(string name)
    {
        this.OutHeaders.Remove(name);
        return true;
    }

    public override bool SetHeaders(string name, string[] values, bool ignoreExist)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        foreach( var x in values )
            this.OutHeaders.Add(name, x);

        return true;
    }

    public void Write(string text)
    {
        _hasStarted = true;

        if( text.IsNullOrEmpty() )
            return;

        byte[] bb = Encoding.UTF8.GetBytes(text);
        _stream.Write(bb, 0, bb.Length);

        this.OutText.AppendLine(text);
    }

    public override void Write(byte[] buffer)
    {
        _hasStarted = true;

        if( buffer.IsNullOrEmpty() )
            return;

        _stream.Write(buffer, 0, buffer.Length);
    }

    public override void WriteAll(byte[] buffer)
    {
        Write(buffer);
    }

    public Task WriteAsync(string text)
    {
        this.Write(text);

        this.OutText.AppendLine(text);

        return Task.CompletedTask;
    }

    public override Task WriteAsync(byte[] buffer)
    {
        this.Write(buffer);

        return Task.CompletedTask;
    }

    public override Task WriteAllAsync(byte[] buffer)
    {
        return WriteAsync(buffer);
    }

    public override IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetAllHeaders()
    {
        foreach( var name in this.OutHeaders.AllKeys ) {
            string[] values = this.OutHeaders.GetValues(name);
            yield return new KeyValuePair<string, IEnumerable<string>>(name, values);
        }
    }


}
