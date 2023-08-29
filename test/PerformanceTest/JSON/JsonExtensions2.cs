using System.Text.Json;

namespace PerformanceTest.JSON;

/// <summary>
/// 使用System.Text.Json的JSON序列化的工具类
/// </summary>
public static class JsonExtensions2
{
    /// <summary>
    /// 将一个对象序列化为JSON字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="options">JSON序列化参数</param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static string ToJson2<T>(this T obj, JsonSerializerOptions options = null)
    {
        if( obj == null )
            throw new ArgumentNullException(nameof(obj));

        return System.Text.Json.JsonSerializer.Serialize<T>(obj, options);
    }



    /// <summary>
    /// 将一个JSON字符串反序列化为对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="json">JSON字符串</param>
    /// <param name="options"></param>
    /// <returns>反序列化得到的结果</returns>
    public static T FromJson2<T>(this string json, JsonSerializerOptions options = null)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
    }


    /// <summary>
    /// 将一个对象序列化为JSON字符串。
    /// </summary>
    /// <param name="list">要序列化的对象</param>
    /// <param name="options"></param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static byte[] ToMultiLineJson2<T>(this ICollection<T> list, JsonSerializerOptions options = null)
    {
        if( list == null || list.Count == 0 )
            return Empty.Array<byte>();

        using( MemoryStream ms = new MemoryStream() ) {

            // 写入一个标记
            ms.WriteByte((byte)255);
            ms.WriteByte((byte)3);

            foreach( var x in list ) {

                byte[] buffer = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<T>(x, options);

                byte[] lenBytes = BitConverter.GetBytes(buffer.Length);
                ms.Write(lenBytes, 0, lenBytes.Length);
                ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
                ms.Write(buffer, 0, buffer.Length);
            }

            byte[] endBytes = BitConverter.GetBytes(0);
            ms.Write(endBytes, 0, endBytes.Length);

            return ms.ToArray();
        }
    }


    /// <summary>
    /// 将一个多行JSON字符串反序列化为列表对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="multiLineJson">以换行符为分隔的多行JSON字符串</param>
    /// <param name="options"></param>
    /// <returns>反序列化得到的结果</returns>
    public static List<T> FromMultiLineJson2<T>(this byte[] multiLineJson, JsonSerializerOptions options = null)
    {
        if( multiLineJson == null )
            return default(List<T>);

        if( multiLineJson.Length == 0 )
            return new List<T>();

        if( multiLineJson.Length <= 6 )
            throw new ArgumentException("输入数据的格式不正确1。", nameof(multiLineJson));


        ReadOnlySpan<byte> span = multiLineJson;
        if( span[0] != (byte)255 || span[1] != (byte)3 )
            throw new ArgumentException("输入数据的格式不正确2。", nameof(multiLineJson));

        List<T> list = new List<T>(32);
        int start = 2;  // 跳过2个标记字符

        while( true ) {

            int len = BitConverter.ToInt32(span.Slice(start, 4));
            if( len <= 0 )
                break;

            start += 4;
            start++; // 跳过 \n 符号

            ReadOnlySpan<byte> line = span.Slice(start, len);
            start += len;

            T log = System.Text.Json.JsonSerializer.Deserialize<T>(line, options);
            list.Add(log);
        }

        return list;
    }



}

