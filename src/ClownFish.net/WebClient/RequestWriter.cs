namespace ClownFish.WebClient;

internal struct RequestWriter
{
    private static readonly Encoding s_defaultEncoding = Encoding.UTF8;

    public string ContentType { get; private set; }


    public void Write(Stream stream, object data, SerializeFormat format)
    {
        if( stream == null || data == null )
            return;

        switch( format ) {
            case SerializeFormat.Text:
                WriteAsTextFormat(stream, data);
                break;

            case SerializeFormat.Json:
                WriteAsJsonFormat(stream, data);
                break;

            case SerializeFormat.Json2:
                WriteAsJson2Format(stream, data);
                break;

            case SerializeFormat.Xml:
                WriteAsXmlFormat(stream, data);
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

    private void WriteText(Stream stream, string text)
    {
        if( text != null && text.Length > 0 ) {
            byte[] bb = s_defaultEncoding.GetBytes(text);

            if( bb != null && bb.Length > 0 ) {
                stream.Write(bb, 0, bb.Length);
            }
        }
    }

    private void WriteBinary(Stream stream, byte[] bb)
    {
        if( bb != null && bb.Length > 0 ) {
            stream.Write(bb, 0, bb.Length);
        }
    }

    private void WriteBinary(Stream destStream, Stream srcStream)
    {
        if( srcStream == null )
            return;

        if( srcStream.CanSeek )
            srcStream.Position = 0;

        srcStream.CopyTo(destStream);
    }

    private void WriteAsTextFormat(Stream stream, object data)
    {
        this.ContentType = RequestContentType.Text;
        WriteText(stream, data.ToString());
    }

    private void WriteAsJsonFormat(Stream stream, object data)
    {
        this.ContentType = RequestContentType.Json;
        string text = (data.GetType() == typeof(string))
                        ? (string)data
                        : JsonExtensions.ToJson(data);
        WriteText(stream, text);
    }

    private void WriteAsJson2Format(Stream stream, object data)
    {
        this.ContentType = RequestContentType.Json;
        string text = (data.GetType() == typeof(string))
                        ? (string)data
                        : JsonExtensions.ToJson(data, JsonStyle.KeepType);    // 序列化时保留类型信息
        WriteText(stream, text);
    }

    private void WriteAsXmlFormat(Stream stream, object data)
    {
        this.ContentType = RequestContentType.Xml;
        string text = (data.GetType() == typeof(string))
                            ? (string)data
                             : XmlHelper.XmlSerialize(data, Encoding.UTF8);
        WriteText(stream, text);
    }

    private void WriteAsFormFormat(Stream stream, object data)
    {
        if( data.GetType() == typeof(string) ) {
            this.ContentType = RequestContentType.Form;
            WriteText(stream, (string)data);
        }
        else {
            FormDataCollection form = FormDataCollection.Create(data);

            if( form.HasFile )
                this.ContentType = form.GetMultipartContentType();
            else
                this.ContentType = RequestContentType.Form;

            form.WriteToStream(stream, Encoding.UTF8);
        }
    }

    private void WriteAsBinFormat(Stream stream, object data)
    {
        if( data.GetType() == typeof(byte[]) ) {
            this.ContentType = RequestContentType.Binary;
            WriteBinary(stream, (byte[])data);
        }
        else if( data is Stream ) {
            this.ContentType = RequestContentType.Binary;
            WriteBinary(stream, (Stream)data);
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
            WriteText(stream, (string)data);
        }
        else if( dataType == typeof(byte[]) ) {
            WriteBinary(stream, (byte[])data);
        }
        else if( data is Stream ) {
            WriteBinary(stream, (Stream)data);
        }
        else {
            throw new NotSupportedException();
        }
    }





}
