//using System.Runtime.Serialization.Formatters.Binary;

//namespace ClownFish.Base;

//// .net5 默认已不支持二进制序列化
//// https://docs.microsoft.com/zh-cn/dotnet/core/compatibility/core-libraries/5.0/binaryformatter-serialization-obsolete

///// <summary>
///// 二进制序列化的工具类（已不建议使用）
///// </summary>
//public static class BinSerializer
//{
//    /// <summary>
//    /// 将对象序列化成二进制数组
//    /// </summary>
//    /// <param name="obj"></param>
//    /// <returns></returns>
//    internal static byte[] Serialize(object obj)
//    {
//        if( obj == null )
//            throw new ArgumentNullException(nameof(obj));

//        using( MemoryStream stream = new MemoryStream() ) {
//            BinaryFormatter formatter = new BinaryFormatter();
//#pragma warning disable SYSLIB0011 // 类型或成员已过时
//            formatter.Serialize(stream, obj);
//#pragma warning restore SYSLIB0011 // 类型或成员已过时

//            stream.Position = 0;
//            return stream.ToArray();
//        }
//    }

//    /// <summary>
//    /// 从二进制数组中反序列化
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="buffer"></param>
//    /// <returns></returns>
//    internal static T Deserialize<T>(byte[] buffer)
//    {
//        return (T)DeserializeObject(buffer);
//    }

//    /// <summary>
//    /// 从二进制数组中反序列化
//    /// </summary>
//    /// <param name="buffer"></param>
//    /// <returns></returns>
//    internal static object DeserializeObject(byte[] buffer)
//    {
//        if( buffer == null || buffer.Length == 0 )
//            throw new ArgumentNullException(nameof(buffer));

//        using( MemoryStream stream = new MemoryStream(buffer, false) ) {
//            stream.Position = 0;

//            BinaryFormatter formatter = new BinaryFormatter();
//#pragma warning disable SYSLIB0011 // 类型或成员已过时
//            return formatter.Deserialize(stream);
//#pragma warning restore SYSLIB0011 // 类型或成员已过时
//        }
//    }

//    /// <summary>
//    /// 采用二进制序列化的方式克隆对象
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="obj"></param>
//    /// <returns></returns>
//    public static T CloneObject<T>(this T obj)
//    {
//#pragma warning disable SYSLIB0011 // 类型或成员已过时
//        byte[] bb = Serialize(obj);
//        return Deserialize<T>(bb);
//#pragma warning restore SYSLIB0011 // 类型或成员已过时
//    }
//}
