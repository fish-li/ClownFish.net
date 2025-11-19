//#if NET6_0_OR_GREATER

//using System.Text.Json;
//using JsonSerializer = System.Text.Json.JsonSerializer;

//namespace ClownFish.Base.Json;

///// <summary>
///// NdJSON序列化的工具类
///// </summary>
//[UnconditionalSuppressMessage("TrimAnalyzer", "IL2026: JsonSerializer")]
//[UnconditionalSuppressMessage("TrimAnalyzer", "IL3050: JsonSerializer")]
//public static class NdJsonExtensions2
//{
//    /// <summary>
//    /// 将一个对象序列化为 ndjson 字符串。
//    /// </summary>
//    /// <param name="list">要序列化的对象</param>
//    /// <param name="settings"></param>
//    /// <returns>序列化得到的JSON字符串</returns>
//    public static string ToNdJson2(this ICollection list, JsonSerializerOptions settings = null)
//    {
//        if( list == null )
//            return string.Empty;

//        using MemoryStream stream = MemoryStreamPool.GetStream();
//        ToNdJson2(list, stream, settings);

//        stream.Position = 0;
//        using( StreamReader reader = new StreamReader(stream, Encoding.UTF8) ) {
//            return reader.ReadToEnd();
//        }
//    }

//    private static readonly byte[] s_newLineBytes = Encoding.UTF8.GetBytes("\n");

//    /// <summary>
//    /// 将一个对象序列化为 ndjson 字符串。
//    /// </summary>
//    /// <param name="list"></param>
//    /// <param name="stream"></param>
//    /// <param name="settings"></param>
//    /// <returns></returns>
//    public static int ToNdJson2(this ICollection list, Stream stream, JsonSerializerOptions settings = null)
//    {
//        if( list == null )
//            return 0;

//        int count = 0;

//        foreach( var x in list ) {
//            count++;
//            JsonSerializer.Serialize(stream, x, settings);
//            stream.Write(s_newLineBytes, 0, s_newLineBytes.Length);

//        }
//        return count;
//    }



//    /// <summary>
//    /// 将一个 ndjson 字符串反序列化为列表对象
//    /// </summary>
//    /// <typeparam name="T">反序列的对象类型参数</typeparam>
//    /// <param name="ndJson">以换行符为分隔的多行JSON字符串</param>
//    /// <param name="capacity">返回列表的初始容量</param>
//    /// <param name="settings">json序列化参数</param>
//    /// <returns>反序列化得到的结果</returns>
//    public static List<T> FromNdJson2<T>(this string ndJson, int capacity = 32, JsonSerializerOptions settings = null)
//    {
//        if( ndJson == null )
//            return null;

//        if( ndJson.Length == 0 )
//            return new List<T>(0);

//        List<T> list = new List<T>(capacity);
//        Type destType = typeof(T);

//        using( StringReader reader = new StringReader(ndJson) ) {
//            while( true ) {
//                string line = reader.ReadLine();
//                if( line == null )
//                    break;

//                if( line.Length > 0 ) {

//                    byte[] buffer = line.ToUtf8Bytes();
//                    T log = (T)JsonSerializer.Deserialize(buffer, destType, settings);
//                    list.Add(log);
//                }
//            }
//        }

//        return list;
//    }


//    /// <summary>
//    /// 将一个 ndjson 字符串反序列化为列表对象
//    /// </summary>
//    /// <typeparam name="T">反序列的对象类型参数</typeparam>
//    /// <param name="stream">包含 ndjson 的数据流</param>
//    /// <param name="capacity">返回列表的初始容量</param>
//    /// <param name="settings">json序列化参数</param>
//    /// <returns></returns>
//    public static List<T> FromNdJson2<T>(this Stream stream, int capacity = 32, JsonSerializerOptions settings = null)
//    {
//        if( stream == null )
//            return new List<T>(0);


//        List<T> list = new List<T>(capacity);
//        Type destType = typeof(T);

//        using( StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true) ) {

//            while( true ) {
//                string line = reader.ReadLine();
//                if( line == null )
//                    break;

//                if( line.Length > 0 ) {

//                    byte[] buffer = line.ToUtf8Bytes();
//                    T log = (T)JsonSerializer.Deserialize(buffer, destType, settings);
//                    list.Add(log);
//                }
//            }
//        }

//        return list;
//    }

//}
//#endif
