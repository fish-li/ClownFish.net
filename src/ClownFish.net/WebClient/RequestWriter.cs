namespace ClownFish.WebClient;

internal struct RequestWriter
{
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

            case SerializeFormat.Ndjson:
                WriteAsNdjsonFormat(stream, data, autoGzip);
                break;

            case SerializeFormat.Xml:
                WriteAsXmlFormat(stream, data, autoGzip);
                break;

            case SerializeFormat.Form:
            case SerializeFormat.Multipart:
                WriteAsWebFormFormat(stream, data);
                break;

            case SerializeFormat.Binary:
                WriteAsBinFormat(stream, data);
                break;

            default:
                WriteAsUnknownFormat(stream, data);
                break;
        }
    }

    private void WriteTextAutoGzip(Stream stream, string text, bool autoGzip)
    {
        if( text != null && text.Length > 0 ) {
            if( autoGzip && text.Length > ClownFishOptions.HttpClient_GzipThreshold ) {

                using( StreamWriter writer = stream.CreateGzipWriter(4096) ) {
                    writer.Write(text);
                }
                IsGzip = true;
                IsBinaryData = true;
            }
            else {
                WriteText(stream, text);        // WriteTextAutoGzip
            }
        }
    }

    private static void WriteText(Stream stream, string text)
    {
        if( text != null && text.Length > 0 ) {

            using( StreamWriter writer = new StreamWriter(stream, EncodingUtils.UTF8NoBOM, 1024, true) ) {

                writer.Write(text);
            }
        }
    }


    private void WriteBinary(Stream stream, byte[] data)    // 正常调用下，这个方法其实并不会进入
    {
        if( data != null && data.Length > 0 ) {
            stream.Write(data, 0, data.Length);
        }
    }

    private void WriteStream(Stream destStream, Stream srcStream)    // 正常调用下，这个方法其实并不会进入
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
        WriteTextAutoGzip(stream, text, autoGzip);    // text
    }

    private void WriteAsJsonFormat(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.JsonUtf8;
        string text = (data.GetType() == typeof(string))
                        ? (string)data
                        : JsonExtensions.ToJson(data);
        WriteTextAutoGzip(stream, text, autoGzip);    // json
    }

    private void WriteAsJson2Format(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.JsonUtf8;
        string text = (data.GetType() == typeof(string))
                        ? (string)data
                        : JsonExtensions.ToJson(data, JsonStyle.KeepType);    // 序列化时保留类型信息
        WriteTextAutoGzip(stream, text, autoGzip);    // json2
    }

    private void WriteAsNdjsonFormat(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.Ndjson;

        if( data is string text ) {
            WriteTextAutoGzip(stream, text, autoGzip);    // ndjson
        }
        else {
            if( data is ICollection list ) {
                WriteNdjsonToStream(stream, list, autoGzip);
            }
            else {
                throw new ArgumentException("HttpOption.Data is not List<T>");
            }
        }
    }

    private void WriteNdjsonToStream(Stream stream, ICollection list, bool autoGzip)
    {
        if( list.IsNullOrEmpty() ) {
            return;
        }

        if( autoGzip ) {   // 这里不做长度判断，直接Gzip压缩
            using( StreamWriter writer = stream.CreateGzipWriter(4096) ) {
                list.ToNdjson(writer);
            }
            IsGzip = true;
            IsBinaryData = true;
        }
        else {
            using( StreamWriter writer = new StreamWriter(stream, EncodingUtils.UTF8NoBOM, 1024, true) ) {
                list.ToNdjson(writer);
            }
        }
    }


    [UnconditionalSuppressMessage("Trimming", "IL2026: XmlSerializer")]
    private void WriteAsXmlFormat(Stream stream, object data, bool autoGzip)
    {
        this.ContentType = ResponseContentType.XmlUtf8;
        string text = (data.GetType() == typeof(string))
                            ? (string)data
                             : XmlHelper.XmlSerialize(data, Encoding.UTF8);
        WriteTextAutoGzip(stream, text, autoGzip);    // xml
    }


    private void WriteAsWebFormFormat(Stream stream, object data)
    {
        if( data is string text ) {
            this.ContentType = RequestContentType.FormUtf8;
            WriteText(stream, text);   // 这里不做gzip压缩, WriteAsWebFormFormat
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
        if( data is byte[] bytes ) {
            this.ContentType = RequestContentType.Binary;
            WriteBinary(stream, bytes);     // WriteAsBinFormat
            IsBinaryData = true;
        }
        else if( data is Stream dataStream ) {
            this.ContentType = RequestContentType.Binary;
            WriteStream(stream, dataStream);     // WriteAsBinFormat
            IsBinaryData = true;
        }
        else {
            throw new NotSupportedException();
        }
    }

    private void WriteAsUnknownFormat(Stream stream, object data)
    {
        // 迹个方法不指定 Content-Type，由外部来指定

        if( data is string text ) {
            WriteText(stream, text);       // 这里不做gzip压缩,WriteAsUnknownFormat
        }
        else if( data is byte[] bytes ) {
            WriteBinary(stream, bytes);     // WriteAsUnknownFormat
            IsBinaryData = true;
        }
        else if( data is Stream dataStream ) {
            WriteStream(stream, dataStream);     // WriteAsUnknownFormat
            IsBinaryData = true;
        }
        else {
            throw new NotSupportedException();
        }
    }





}
