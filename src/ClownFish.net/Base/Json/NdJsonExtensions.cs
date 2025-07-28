namespace ClownFish.Base;

/// <summary>
/// NdJSON序列化的工具类
/// </summary>
public static class NdJsonExtensions
{

    /// <summary>
    /// 将一个对象序列化为 ndjson 字符串。
    /// </summary>
    /// <param name="list">要序列化的对象</param>
    /// <param name="settings"></param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static string ToMultiLineJson<T>(this IEnumerable<T> list, JsonSerializerSettings settings = null)
    {
        if( list == null )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            using( StringWriter writer = new StringWriter(sb) ) {

                ToMultiLineJson<T>(list, writer, settings);

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
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="writer"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static int ToMultiLineJson<T>(this IEnumerable<T> list, TextWriter writer, JsonSerializerSettings settings = null)
    {
        if( list == null )
            return 0;

        JsonSerializer jsonSerializer = settings.CreateJsonSerializer();

        int count = 0;
        using( JsonTextWriter jsonTextWriter = new JsonTextWriter(writer) ) {
            jsonTextWriter.Formatting = jsonSerializer.Formatting;

            foreach( var x in list ) {
                count++;
                jsonSerializer.Serialize(jsonTextWriter, x);
                //jsonTextWriter.Flush();   // ##### 注意是：这行代码不能启用，它会导致 writer 的缓冲区失效，如果外层是一个GZIP流，它会影响压缩率！
                writer.Write('\n');

                // 最后以“换行符”结束，这里参考了 elasticsearch 的要求
                // The final line of data must end with a newline character \n
                // https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html#docs-bulk-api-desc
            }
        }
        writer.Flush();
        return count;
    }



    /// <summary>
    /// 将一个 ndjson 字符串反序列化为列表对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="multiLineJson">以换行符为分隔的多行JSON字符串</param>
    /// <param name="capacity">返回列表的初始容量</param>
    /// <param name="settings">json序列化参数</param>
    /// <returns>反序列化得到的结果</returns>
    public static List<T> FromMultiLineJson<T>(this string multiLineJson, int capacity = 32, JsonSerializerSettings settings = null)
    {
        if( multiLineJson == null )
            return null;

        if( multiLineJson.Length == 0 )
            return new List<T>(0);

        using( StringReader reader = new StringReader(multiLineJson) ) {
            return FromMultiLineJson<T>(reader, capacity, settings);
        }
    }


    /// <summary>
    /// 将一个 ndjson 字符串反序列化为列表对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="reader">用于获取 ndjson 的数据读取器</param>
    /// <param name="capacity">返回列表的初始容量</param>
    /// <param name="settings">json序列化参数</param>
    /// <returns></returns>
    public static List<T> FromMultiLineJson<T>(this TextReader reader, int capacity = 32, JsonSerializerSettings settings = null)
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

                using( JsonTextReader reader2 = new JsonTextReader(new StringReader(line)) ) {
                    T log = (T)jsonSerializer.Deserialize(reader2, destType);
                    list.Add(log);
                }
            }
        }

        return list;
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
