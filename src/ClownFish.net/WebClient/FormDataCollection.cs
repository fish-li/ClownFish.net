namespace ClownFish.WebClient;

/// <summary>
/// 表示HTTP表单的数据集合（key=value ）
/// </summary>
public sealed class FormDataCollection
{
    private static readonly string s_boundary = "2c7ad4d5617d449992786e4d5d4a75ed";
    private static readonly byte[] s_boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + s_boundary + "\r\n");
    private static readonly byte[] s_endboundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + s_boundary + "--\r\n");

    private readonly List<KeyValuePair<string, object>> _list = new List<KeyValuePair<string, object>>(32);

    /// <summary>
    /// 是否包含上传文件
    /// </summary>
    public bool HasFile { get; private set; }

    /// <summary>
    /// 获取上传文件的请求头
    /// </summary>
    /// <returns></returns>
    public string GetMultipartContentType()
    {
        if( HasFile == false )
            throw new InvalidOperationException();

        return "multipart/form-data; boundary=" + s_boundary;
    }



    /// <summary>
    /// 往集合中添加一个键值对（允许key重复）
    /// </summary>
    /// <param name="key">数据项的名称</param>
    /// <param name="value">数据值</param>
    public FormDataCollection AddString(string key, string value)
    {
        if( string.IsNullOrEmpty(key) )
            throw new ArgumentNullException("key");

        _list.Add(new KeyValuePair<string, object>(key, value ?? string.Empty));
        return this;
    }

    /// <summary>
    /// 往集合中添加一个上传文件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public FormDataCollection AddFile(string key, FileInfo file)
    {
        if( string.IsNullOrEmpty(key) )
            throw new ArgumentNullException("key");
        if( file == null )
            throw new ArgumentNullException(nameof(file));


        HttpFile httFile = HttpFile.CreateFromFileInfo(file);
        _list.Add(new KeyValuePair<string, object>(key, httFile));

        // ----------标记包含上传文件----------------------
        HasFile = true;
        // -----------------------------------------------
        return this;
    }


    /// <summary>
    /// 往集合中添加一个上传文件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public FormDataCollection AddFile(string key, HttpFile file)
    {
        if( string.IsNullOrEmpty(key) )
            throw new ArgumentNullException("key");
        if( file == null )
            throw new ArgumentNullException(nameof(file));

        file.Validate();

        _list.Add(new KeyValuePair<string, object>(key, file));

        // ----------标记包含上传文件----------------------
        HasFile = true;
        // -----------------------------------------------
        return this;
    }


    /// <summary>
    /// 往集合中添加一个键值对（允许key重复）
    /// </summary>
    /// <param name="key">数据项的名称</param>
    /// <param name="value">数据值</param>
    public FormDataCollection AddObject(string key, object value)
    {
        if( string.IsNullOrEmpty(key) )
            throw new ArgumentNullException("key");

        // 除了上传文件之外，其它数据都转换成字符串。

        if( value == null )
            return AddString(key, string.Empty);

        Type valueType = value.GetType();

        if( valueType == typeof(string) )
            return AddString(key, (string)value);


        if( valueType == typeof(FileInfo) ) {
            return AddFile(key, (FileInfo)value);
        }

        if( valueType == typeof(HttpFile) ) {
            return AddFile(key, (HttpFile)value);
        }

        if( valueType == typeof(byte[]) ) {
            string text = Convert.ToBase64String((byte[])value);
            return AddString(key, text);
        }

        // string[] ，不处理，可以通过给 Data 设置来解决（用一个KEY多次指定值）。

        _list.Add(new KeyValuePair<string, object>(key, value.ToString()));
        return this;
    }

    /// <summary>
    /// 输出集合数据为 "application/x-www-form-urlencoded" 格式。
    /// 注意：1、忽略上传文件
    ///      2、每次调用都会重新计算（因此尽量避免重复调用）
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( KeyValuePair<string, object> kvp in _list ) {
                if( kvp.Value.GetType() == typeof(string) ) {
                    if( sb.Length > 0 )
                        sb.Append('&');

                    sb.Append(System.Web.HttpUtility.UrlEncode(kvp.Key))
                        .Append('=')
                        .Append(System.Web.HttpUtility.UrlEncode((string)kvp.Value ?? string.Empty));
                }
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }




    /// <summary>
    /// 将收集的表单数据写入流
    /// </summary>
    /// <param name="stream">Stream实例，用于写入</param>
    /// <param name="encoding">字符编码方式</param>
    public void WriteToStream(Stream stream, Encoding encoding)
    {
        if( stream == null )
            throw new ArgumentNullException("stream");
        if( encoding == null )
            throw new ArgumentNullException("encoding");

        if( HasFile == false )
            WriteSimpleTextToStream(stream, encoding);

        else
            WriteMultiFormToStream(stream, encoding);
    }

    private void WriteSimpleTextToStream(Stream stream, Encoding encoding)
    {
        // 获取编码后的字符串
        string text = this.ToString();

        if( string.IsNullOrEmpty(text) == false ) {
            byte[] postData = encoding.GetBytes(text);

            // 写输出流
            using( BinaryWriter bw = new BinaryWriter(stream,
                encoding /* 指定的编码其实不起作用！ .net API 设计不合理！ */,
                true /* 保持流打开状态，由方法外面关闭 */ ) ) {

                bw.Write(postData);
            }
        }
    }


    private void WriteMultiFormToStream(Stream stream, Encoding encoding)
    {
        // 写入非文件的key/value部分
        foreach( KeyValuePair<string, object> kvp in _list ) {
            if( kvp.Value.GetType() == typeof(string) ) {

                // 写入数据块的分隔标记
                stream.Write(s_boundaryBytes, 0, s_boundaryBytes.Length);

                // 写入数据项描述头
                string disposition = string.Format(
                        "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n",
                        kvp.Key);

                byte[] header = encoding.GetBytes(disposition);
                stream.Write(header, 0, header.Length);

                byte[] data = encoding.GetBytes(kvp.Value.ToString());
                stream.Write(data, 0, data.Length);
            }
        }


        // 写入要上传的文件
        foreach( KeyValuePair<string, object> kvp in _list ) {
            if( kvp.Value.GetType() == typeof(HttpFile) ) {
                HttpFile file = (HttpFile)kvp.Value;

                // 写入数据块的分隔标记
                stream.Write(s_boundaryBytes, 0, s_boundaryBytes.Length);

                // 写入文件描述头，这里设置一个通用的类型描述：application/octet-stream，具体的描述在注册表里有。
                string disposition = string.Format(
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                        "Content-Type: application/octet-stream\r\n\r\n",
                        kvp.Key, Path.GetFileName(file.FileName));

                byte[] header = encoding.GetBytes(disposition);
                stream.Write(header, 0, header.Length);

                // 写入文件内容
                if( file.FileBody != null ) {
                    stream.Write(file.FileBody, 0, file.FileBody.Length);
                }
                else if( file.FileInfo != null ) {
                    using( FileStream fileStream = file.FileInfo.OpenRead() ) {
                        fileStream.CopyTo(stream);
                    }
                }
                else if( file.BodyStream != null ) {
                    file.BodyStream.CopyTo(stream);
                }
            }
        }

        // 写入结束标记
        stream.Write(s_endboundaryBytes, 0, s_endboundaryBytes.Length);
    }

    /// <summary>
    /// 根据一个匿名对象的键值，生成URL查询字符串参数
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetQueryString(object obj)
    {
        FormDataCollection form = Create(obj);
        return form.ToString();
    }



    /// <summary>
    /// 将一个对象按"application/x-www-form-urlencoded" 方式序列化
    /// 说明：这个实现与浏览器的实现是有差别的，它不支持数组，也不支持上传文件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    internal static FormDataCollection Create(object obj)
    {
        if( obj == null )
            throw new ArgumentNullException("obj");

        // 对参数做个简单的判断，防止传入了错误的类型
        if( obj.GetType().IsPrimitive )
            throw new ArgumentException("参数类型不正确，不应该是一个基础类型。");

        if( obj.GetType() == typeof(string) )
            throw new ArgumentException("参数类型不正确，不应该是一个字符串类型。");


        FormDataCollection collection = obj as FormDataCollection;
        if( collection != null )
            return collection;


        IDictionary dict = obj as IDictionary;
        if( dict != null )
            return CreateFromDictionary(dict);

        IDictionary<string, object> dict2 = obj as IDictionary<string, object>;  // include System.Dynamic.ExpandoObject
        if( dict2 != null )
            return CreateFromDictionary2(dict2);

        NameValueCollection nvCollection = obj as NameValueCollection;
        if( nvCollection != null )
            return CreateFromCollection(nvCollection);

        // 按自定义类型来处理
        return CreateFromObject(obj);
    }


    private static FormDataCollection CreateFromCollection(NameValueCollection nvCollection)
    {
        FormDataCollection collection = new FormDataCollection();

        foreach( string key in nvCollection.AllKeys ) {
            string[] values = nvCollection.GetValues(key);

            foreach( string value in values )
                collection.AddString(key, value);
        }

        return collection;
    }

    private static FormDataCollection CreateFromDictionary(IDictionary dict)
    {
        FormDataCollection collection = new FormDataCollection();

        foreach( DictionaryEntry de in dict )
            collection.AddObject(de.Key.ToString(), de.Value);

        return collection;
    }

    private static FormDataCollection CreateFromDictionary2(IDictionary<string, object> dict)
    {
        FormDataCollection collection = new FormDataCollection();

        foreach( KeyValuePair<string, object> kv in dict )
            collection.AddObject(kv.Key, kv.Value);

        return collection;
    }


    private static FormDataCollection CreateFromObject(object obj)
    {
        FormDataCollection collection = new FormDataCollection();
        PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach( PropertyInfo p in properties ) {
            object value = p.FastGetValue(obj);
            if( value == null ) {
                collection.AddString(p.Name, string.Empty);
                continue;
            }

            if( value.GetType() == typeof(string[]) ) {
                foreach( string s in (string[])value )
                    collection.AddString(p.Name, (s ?? string.Empty));

            }
            else {
                collection.AddObject(p.Name, value);
            }
        }

        return collection;
    }


}
