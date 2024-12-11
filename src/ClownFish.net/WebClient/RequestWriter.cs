namespace ClownFish.WebClient;

internal struct RequestWriter
{
    private static readonly Encoding s_defaultEncoding = Encoding.UTF8;

    public string ContentType { get; private set; }

    public bool IsGzip { get; private set; }

    public bool IsBinaryData { get; private set; }


    public void Write(Stream stream, object data, SerializeFormat format, bool autoGzip = false)
    {
        if( stream == null || data == null )
            return;

        switch( format ) {
            case SerializeFormat.Text:
                WriteAsTextFormat(stream, data, autoGzip);
                break;

            case SerializeFormat.Json:
                WriteAsJsonFormat(stream, data, autoGzip);
                break;

            case SerializeFormat.Json2:
                WriteAsJson2Format(stream, data, autoGzip);
                break;

            case SerializeFormat.Xml:
                WriteAsXmlFormat(stream, data, autoGzip);
                break;

            case SerializeFormat.Form:
            case SerializeFormat.Multipart:
                WriteAsFormFormat(stream, data);
                break;

            case SerializeFormat.Binary:
                WriteAsBinFormat(stream, data);
                break;

            default:
                WriteAsUnknownFormat(stream, data);
                break;
        }
    }

    private void WriteText2(Stream stream, string text, bool autoGzip)
    {
        if( autoGzip && text.Length > ClownFishOptions.HttpClient_GzipThreshold ) {
            byte[] gzipData = text.ToGzip();
            WriteBinary(stream, gzipData);    // WriteText2
            IsGzip = true;
            IsBinaryData = true;
        }
        else {
            WriteText(stream, text);        // WriteText2
        }
    }

    private static void WriteText(Stream stream, string text)
    {
        if( text != null && text.Length > 0 ) {
            byte[] bb = s_defaultEncoding.GetBytes(text);

            if( bb != null && bb.Length > 0 ) {
                stream.Write(bb, 0, bb.Length);
            }
        }
    }

    private static void WriteBinary(Stream stream, byte[] bb)
    {
        if( bb != null && bb.Length > 0 ) {
            stream.Write(bb, 0, bb.Length);
        }
    }

    private static void WriteBinary(Stream destStream, Stream srcStream)
    {
        if( srcStream == null )
            return;

        if( srcStream.CanSeek )
            srcStream.Position = 0;

        srcStream.CopyTo(destStream);
    }

    private void WriteAsTextFormat(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.TextUtf8;
        string text = data.ToString();
        WriteText2(stream, text, autoGzip);    // text
    }

    private void WriteAsJsonFormat(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.JsonUtf8;
        string text = (data.GetType() == typeof(string))
                        ? (string)data
                        : JsonExtensions.ToJson(data);
        WriteText2(stream, text, autoGzip);    // json
    }

    private void WriteAsJson2Format(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.JsonUtf8;
        string text = (data.GetType() == typeof(string))
                        ? (string)data
                        : JsonExtensions.ToJson(data, JsonStyle.KeepType);    // 序列化时保留类型信息
        WriteText2(stream, text, autoGzip);    // json2
    }

    private void WriteAsXmlFormat(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.XmlUtf8;
        string text = (data.GetType() == typeof(string))
                            ? (string)data
                             : XmlHelper.XmlSerialize(data, Encoding.UTF8);
        WriteText2(stream, text, autoGzip);    // xml
    }

    private void WriteAsFormFormat(Stream stream, object data)
    {
        if( data.GetType() == typeof(string) ) {
            this.ContentType = RequestContentType.FormUtf8;
            WriteText(stream, (string)data);   // 这里不做gzip压缩
        }
        else {
            FormDataCollection form = FormDataCollection.Create(data);

            if( form.HasFile ) {
                this.ContentType = form.GetMultipartContentType();
                this.IsBinaryData = true;
            }
            else {
                this.ContentType = RequestContentType.FormUtf8;
            }

            form.WriteToStream(stream, Encoding.UTF8);
        }
    }

    private void WriteAsBinFormat(Stream stream, object data)
    {
        if( data.GetType() == typeof(byte[]) ) {
            this.ContentType = RequestContentType.Binary;
            WriteBinary(stream, (byte[])data);     // WriteAsBinFormat
            IsBinaryData = true;
        }
        else if( data is Stream ) {
            this.ContentType = RequestContentType.Binary;
            WriteBinary(stream, (Stream)data);     // WriteAsBinFormat
            IsBinaryData = true;
        }
        else {
            throw new NotSupportedException();
        }
    }

    private void WriteAsUnknownFormat(Stream stream, object data)
    {
        // 迹个方法不指定 Content-Type，由外部来指定

        Type dataType = data.GetType();

        if( dataType == typeof(string) ) {
            WriteText(stream, (string)data);       // 这里不做gzip压缩
        }
        else if( dataType == typeof(byte[]) ) {
            WriteBinary(stream, (byte[])data);     // WriteAsUnknownFormat
            IsBinaryData = true;
        }
        else if( data is Stream ) {
            WriteBinary(stream, (Stream)data);     // WriteAsUnknownFormat
            IsBinaryData = true;
        }
        else {
            throw new NotSupportedException();
        }
    }





}
