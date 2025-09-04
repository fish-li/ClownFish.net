namespace ClownFish.Base;

/// <summary>
/// NdJSON序列化的工具类
/// </summary>
public static class NdJsonExtensions
{
    #region 已废弃的方法

    /// <summary>
    /// 已废弃的方法
    /// </summary>
    /// <param name="list"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    [Obsolete("请用 ToNdjson 方法来代替")]
    public static string ToMultiLineJson(this ICollection list, JsonSerializerSettings settings = null)
    {
        return ToNdjson(list, settings);
    }

    /// <summary>
    /// 已废弃的方法
    /// </summary>
    /// <param name="list"></param>
    /// <param name="writer"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    [Obsolete("请用 ToNdjson 方法来代替")]
    public static int ToMultiLineJson(this ICollection list, TextWriter writer, JsonSerializerSettings settings = null)
    {
        return ToNdjson(list, writer, settings);
    }

    /// <summary>
    /// 已废弃的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ndjson"></param>
    /// <param name="capacity"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    [Obsolete("请用 FromNdjson 方法来代替")]
    public static List<T> FromMultiLineJson<T>(this string ndjson, int capacity = 100, JsonSerializerSettings settings = null)
    {
        return FromNdjson<T>(ndjson, capacity, settings);
    }

    /// <summary>
    /// 已废弃的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="capacity"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    [Obsolete("请用 FromNdjson 方法来代替")]
    public static List<T> FromMultiLineJson<T>(this TextReader reader, int capacity = 100, JsonSerializerSettings settings = null)
    {
        return FromNdjson<T>(reader, capacity, settings);
    }

    #endregion


    /// <summary>
    /// 将一个对象序列化为 ndjson 字符串。
    /// </summary>
    /// <param name="list">要序列化的对象</param>
    /// <param name="settings"></param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static string ToNdjson(this ICollection list, JsonSerializerSettings settings = null)
    {
        if( list == null )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            using( StringWriter writer = new StringWriter(sb) ) {

                ToNdjson(list, writer, settings);

                return sb.ToString();
            }
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    
    /// <summary>
    /// 将一个对象序列化为 ndjson 字符串。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="writer"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static int ToNdjson(this ICollection list, TextWriter writer, JsonSerializerSettings settings = null)
    {
        if( list == null )
            return 0;

        JsonSerializer jsonSerializer = settings.CreateJsonSerializer();

        int count = 0;
        using( JsonTextWriter jsonTextWriter = new JsonTextWriter(writer) ) {
            jsonTextWriter.CloseOutput = false;
            jsonTextWriter.Formatting = jsonSerializer.Formatting;

            foreach( var x in list ) {
                count++;
                jsonSerializer.Serialize(jsonTextWriter, x);
                writer.Write('\n');

                // 最后以“换行符”结束，这里参考了 elasticsearch 的要求
                // The final line of data must end with a newline character \n
                // https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html#docs-bulk-api-desc
            }
        }

        // writer.Flush() 这个调用也没发现有什么实质意义~~~
        // 但是可以检测一个异常，System.ObjectDisposedException: Cannot write to a closed TextWriter. Object name: 'StreamWriter'.

        // 异常重现方法：
        // 1，不设置 jsonTextWriter.CloseOutput = false;     // 此属性默认是 true
        // 2，外层调用代码：
        //      using( StreamWriter writer = stream.CreateGzipWriter(4096) ) {
        //         list.ToNdjson(writer);
        //      }
        // 在调试模式下，发现 writer._disposed = true，其实也【符合预期】，毕竟 jsonTextWriter.CloseOutput = true


        // 早期版本 不出现 异常的做法：
        // 1，不设置 jsonTextWriter.CloseOutput = false;      // 以前也不知道有这个属性，和它相关
        // 2，外层调用代码： 
        //       using( GZipStream gzip = new GZipStream(stream, CompressionMode.Compress, true) ) {
        //            using( StreamWriter writer = new StreamWriter(gzip, EncodingUtils.UTF8NoBOM, 1024 * 4, true) ) {
        //                list.ToNdjson(writer);
        //            }
        //       }
        // 在调试模式下，发现 writer._disposed = false 【居然没有关闭】~~~~~~~~~~

        // 所以，最终决定，保留下面这行代码：writer.Flush();
        // 然后设置 jsonTextWriter.CloseOutput = false;    明确指定不让jsonTextWriter瞎操作！

        writer.Flush();
        return count;
    }


    

    /// <summary>
    /// 将一个 ndjson 字符串反序列化为列表对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="ndjson">以换行符为分隔的多行JSON字符串</param>
    /// <param name="capacity">返回列表的初始容量</param>
    /// <param name="settings">json序列化参数</param>
    /// <returns>反序列化得到的结果</returns>
    public static List<T> FromNdjson<T>(this string ndjson, int capacity = 100, JsonSerializerSettings settings = null)
    {
        if( ndjson == null )
            return null;

        if( ndjson.Length == 0 )
            return new List<T>(0);

        using( StringReader reader = new StringReader(ndjson) ) {
            return FromNdjson<T>(reader, capacity, settings);
        }
    }


    /// <summary>
    /// 从TextReader对象中逐行读取并转成对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="reader">用于获取 ndjson 的数据读取器</param>
    /// <param name="capacity">返回列表的初始容量</param>
    /// <param name="settings">json序列化参数</param>
    /// <returns></returns>
    public static List<T> FromNdjson<T>(this TextReader reader, int capacity = 100, JsonSerializerSettings settings = null)
    {
        if( reader == null )
            return new List<T>(0);

        JsonSerializer jsonSerializer = settings.CreateJsonSerializer();

        List<T> list = new List<T>(capacity);
        Type destType = typeof(T);

        while( true ) {
            string line = reader.ReadLine();
            if( line == null )
                break;

            if( line.Length > 0 ) {

                T item = (T)jsonSerializer.Deserialize(new StringReader(line), destType);
                list.Add(item);
            }
        }

        return list;
    }


    private static readonly MethodInfo s_method1 = typeof(NdJsonExtensions).GetMethod("FromNdjson",
                                                    BindingFlags.Static | BindingFlags.Public, null,
                                                    new Type[] { typeof(TextReader), typeof(int), typeof(JsonSerializerSettings) }
                                                    , null);

    internal static object LoadListFromNdjson(this TextReader reader, Type listType)
    {
        Type elementType = listType.GetGenericArguments()[0];

        MethodInfo method2 = s_method1.MakeGenericMethod(elementType);

        return method2.FastInvoke(null, new object[] { reader, 64, null });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="maxRows"></param>
    /// <param name="writer"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static int DbReaderToNdJson(this DbDataReader reader, int maxRows, TextWriter writer, JsonSerializerSettings settings = null)
    {
        if( reader == null )
            throw new ArgumentNullException(nameof(reader));
        if( writer == null )
            throw new ArgumentNullException(nameof(writer));

        int count = 0;
        int columnCount = reader.FieldCount;
        Dictionary<string, object> row = new Dictionary<string, object>(300);

        JsonSerializer jsonSerializer = settings.CreateJsonSerializer();

        using( JsonTextWriter jsonTextWriter = new JsonTextWriter(writer) ) {
            jsonTextWriter.CloseOutput = false;
            jsonTextWriter.Formatting = jsonSerializer.Formatting;

            while( reader.Read() ) {   // 为了代码共用，这里固定使用同步调用。这里再使用异步也不会对吞吐量有多大改善了~~~
                count++;

                for( int i = 0; i < columnCount; i++ ) {
                    string name = reader.GetName(i);
                    object value = reader.GetValue(i);

                    if( value == null || value == DBNull.Value ) {
                        // ignore
                    }
                    else {
                        row[name] = value;
                    }
                }
                jsonSerializer.Serialize(jsonTextWriter, row);
                jsonTextWriter.Flush();
                writer.Write('\n');

                //string json = row.ToJson();
                //writer.WriteLine(json);

                row.Clear();

                if( maxRows > 0 && count >= maxRows )
                    break;
            }
            writer.Flush();
        }

        return count;
    }


}
