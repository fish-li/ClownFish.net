namespace ClownFish.WebClient;

/// <summary>
/// HTTP头的存储集合
/// </summary>
public sealed class HttpHeaderCollection : List<NameValue>
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public HttpHeaderCollection() : base(16) { }


    /// <summary>
    /// 将一个匿名对象转换成HttpHeaderCollection实例
    /// </summary>
    /// <param name="obj"></param>
    public static HttpHeaderCollection Create(object obj)
    {
        if( obj == null )
            throw new ArgumentNullException(nameof(obj));

        // 说明： C# 编译器不允许 从 object 到 HttpHeaderCollection 的类型转换，所以只能这样定义一个方法。

        // 对参数做个简单的判断，防止传入了错误的类型
        if( obj.GetType().IsPrimitive )
            throw new ArgumentException("参数类型不正确，不应该是一个基础类型。");

        if( obj.GetType() == typeof(string) )
            throw new ArgumentException("参数类型不正确，不应该是一个字符串类型。");


        HttpHeaderCollection collection = obj as HttpHeaderCollection;
        if( collection != null )
            return collection;


        IDictionary<string, string> dict = obj as IDictionary<string, string>;
        if( dict != null )
            return CreateFromDictionary(dict);


        NameValueCollection nvCollection = obj as NameValueCollection;
        if( nvCollection != null )
            return CreateFromCollection(nvCollection);


        // 按自定义类型来处理
        return CreateFromObject(obj);
    }


    private static HttpHeaderCollection CreateFromDictionary(IDictionary<string, string> dict)
    {
        HttpHeaderCollection result = new HttpHeaderCollection();

        foreach( var kv in dict )
            result.Add(kv.Key, kv.Value);

        return result;
    }


    private static HttpHeaderCollection CreateFromCollection(NameValueCollection nvCollection)
    {
        HttpHeaderCollection result = new HttpHeaderCollection();

        foreach( string key in nvCollection.AllKeys ) {
            string[] values = nvCollection.GetValues(key);

            foreach( string value in values )
                result.Add(key, value);
        }

        return result;
    }


    private static HttpHeaderCollection CreateFromObject(object obj)
    {
        HttpHeaderCollection result = new HttpHeaderCollection();

        var ps = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach( var p in ps ) {
            string name = p.Name.Replace('_', '-');
            string value = (p.FastGetValue(obj) ?? string.Empty).ToString();
            result.Add(name, value);
        }

        return result;
    }


    /// <summary>
    /// 将 HttpHeaderCollection 隐式转换为 NameValueCollection
    /// </summary>
    /// <param name="headers"></param>
    public static implicit operator NameValueCollection(HttpHeaderCollection headers)
    {
        if( headers == null )
            throw new ArgumentNullException(nameof(headers));

        NameValueCollection collection = new NameValueCollection();

        foreach( var x in headers )
            collection.Add(x.Name, x.Value);

        return collection;
    }


    /// <summary>
    /// 将 NameValueCollection 隐式转换为 HttpHeaderCollection
    /// </summary>
    /// <param name="collection"></param>
    public static implicit operator HttpHeaderCollection(NameValueCollection collection)
    {
        if( collection == null )
            throw new ArgumentNullException(nameof(collection));

        return CreateFromCollection(collection);
    }


    /// <summary>
    /// 将一个字典转换成HttpHeaderCollection实例
    /// </summary>
    /// <param name="dictionary"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065")]
    public static implicit operator HttpHeaderCollection(Dictionary<string, string> dictionary)
    {
        // 说明：
        // 1、这个方法不能定义成支持匿名对象，因为编译器不让通过
        // 2、请求头的KEY名经常包含横线，这个也是匿名对象不能支持的

        if( dictionary == null )
            throw new ArgumentNullException(nameof(dictionary));

        return CreateFromDictionary(dictionary);
    }



    /// <summary>
    /// 索引器，根据名称访问集合
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string this[string name] {
        get {
            // 如果KEY重复，这里只返回第一个匹配的结果
            // KEY重复的场景需要提供GetValues方法，暂且先不实现
            foreach( var x in this ) {
                if( x.Name.Is(name) )
                    return x.Value;
            }
            return null;
        }
        set {
            foreach( var x in this ) {
                if( x.Name.Is(name) ) {
                    x.Value = value;
                    return;
                }
            }
            NameValue nv = new NameValue { Name = name, Value = value };
            this.Add(nv);
        }
    }


    /// <summary>
    /// 增加一个键值对
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void Add(string name, string value)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException("name");

        //if( string.IsNullOrEmpty(value) )
        if( value == null )
            throw new ArgumentNullException("value");

        NameValue nv = new NameValue { Name = name, Value = value };
        this.Add(nv);
    }

    /// <summary>
    /// 根据指定的名称删除键值列表元素
    /// </summary>
    /// <param name="name"></param>
    public int Remove(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException("name");

        List<int> indexs = new List<int>();

        for( int i = 0; i < this.Count; i++ ) {
            if( this[i].Name.Is(name) ) {
                indexs.Add(i);
            }
        }

        if( indexs.Count > 0 ) {
            for( int i = indexs.Count - 1; i >= 0; i-- ) {
                int index = indexs[i];
                this.RemoveAt(index);
            }
        }

        return indexs.Count;
    }


    /// <summary>
    /// 是否包含某个名称
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool ContainsName(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException("name");

        foreach( var x in this ) {
            if( x.Name.Is(name) )
                return true;
        }
        return false;
    }

}
