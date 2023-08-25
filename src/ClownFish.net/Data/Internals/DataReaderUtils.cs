namespace ClownFish.Data.Internals;

/// <summary>
/// 供框架内部使用（不考虑升级兼容问题）！
/// </summary>
public struct DataFieldMapKV
{
    /// <summary>
    /// 
    /// </summary>
    public readonly int Index;
    /// <summary>
    /// 
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    public DataFieldMapKV(int index, string name)
    {
        this.Index = index;
        this.Name = name;
    }
}


/// <summary>
/// 用于数据加载相关的扩展工具类，仅供框架内部使用（不考虑升级兼容问题）！
/// </summary>
public static class DataReaderUtils
{
    /// <summary>
    /// 供实体加载器使用的内部方法，请不要在代码中调用！
    /// </summary>
    /// <param name="dataSource"></param>
    /// <param name="len"></param>
    /// <param name="kvs"></param>
    /// <returns></returns>
    public static int[] CreateNameMapIndex(object dataSource, int len, params DataFieldMapKV[] kvs)
    //public static int[] CreateNameMapIndex(object dataSource, int len, params (int, string)[] kvs)
    {
        string[] names = null;
        DbDataReader reader = dataSource as DbDataReader;
        if( reader != null )
            names = DataExtensions.GetColumnNames(reader);
        else
            names = DataExtensions.GetColumnNames((DataTable)dataSource);


        // 优化这段代码的性能 （性能提升 60%）
        //int[] cols = new int[19];
        //cols[0] = DataExtensions.FindIndex(names, "rid");
        //cols[1] = DataExtensions.FindIndex(names, "intA");
        //cols[2] = DataExtensions.FindIndex(names, "timeA");
        //cols[3] = DataExtensions.FindIndex(names, "moneyA");
        // ............................

        int[] cols = new int[len];

        foreach( var kv in kvs ) {
            int index = -1;

            for( int i = 0; i < names.Length; i++ ) {
                if( names[i] != null
                    && string.Compare(names[i], kv.Name, StringComparison.OrdinalIgnoreCase) == 0 ) {

                    index = i;
                    names[i] = null;    // 节省后续查找时间
                    break;
                }
            }
            cols[kv.Index] = index;
        }
        return cols;
    }


    //private static void IfNllThrowException(object value)
    //{
    //    if( value == null )
    //        throw new InvalidCastException("NULL值不能转成值类型。");
    //}

    //private static T Exec<T>(Func<T> func, Type entityType, string propertyName)
    //{
    //    try {
    //        return func();
    //    }
    //    catch( InvalidCastException ex ) {
    //        throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
    //    }
    //    catch( Exception ex2 ) {
    //        throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
    //    }
    //}


    //private static object GetFieldValue(DbDataReader reader, int index)
    //{
    //    object value = reader.GetValue(index);

    //    if( DBNull.Value.Equals(value) )
    //        value = null;

    //    return value;
    //}



    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static string ToString(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        object value = reader.GetValue(index);
        if( value == DBNull.Value )
            return null;

        return value.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool ToBool(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            //return reader.GetBoolean(index);

            object value = reader.GetValue(index);

            if( value is bool boolValue ) {
                return boolValue;
            }
            else {
                int x = Convert.ToInt32(value);
                return x != 0;
            }
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool? ToBoolNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            if( value is bool boolValue ) {
                return boolValue;
            }
            else {
                int x = Convert.ToInt32(value);
                return x != 0;
            }
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static byte ToByte(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            //return reader.GetByte(index);
            object value = reader.GetValue(index);
            return Convert.ToByte(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static byte? ToByteNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToByte(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static sbyte ToSByte(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            // 有些数据库没有 sbyte 对应的数据类型
            // 例如：SQLSERVER 只有 tinyint， 只能映射到 byte，所以下面的代码可能会出现异常
            //return reader.GetFieldValue<sbyte>(index);

            object value = reader.GetValue(index);
            return Convert.ToSByte(value);

        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static sbyte? ToSByteNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToSByte(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return (byte[])value;
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static char ToChar(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            //return reader.GetChar(index);  // NotSupported
            object value = reader.GetValue(index);

            string s = value.ToString();
            return s.Length > 0 ? s[0] : ' ';

        }
        //catch( InvalidCastException ex ) {
        //    throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        //}
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static char? ToCharNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            string s = value.ToString();
            return s.Length > 0 ? s[0] : ' ';
        }
        //catch( InvalidCastException ex ) {
        //    throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        //}
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }





    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static short ToShort(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetInt16(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static short? ToShortNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToInt16(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ushort ToUShort(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            //return reader.GetFieldValue<ushort>(index);
            object value = reader.GetValue(index);
            return Convert.ToUInt16(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ushort? ToUShortNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToUInt16(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static int ToInt(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetInt32(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static int? ToIntNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToInt32(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static uint ToUInt(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            //return reader.GetFieldValue<uint>(index);
            object value = reader.GetValue(index);
            return Convert.ToUInt32(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static uint? ToUIntNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToUInt32(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static long ToLong(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetInt64(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static long? ToLongNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToInt64(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ulong ToULong(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            //return reader.GetFieldValue<ulong>(index);
            object value = reader.GetValue(index);
            return Convert.ToUInt64(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ulong? ToULongNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToUInt64(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }





    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static float ToFloat(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetFloat(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static float? ToFloatNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToSingle(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static double ToDouble(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetDouble(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static double? ToDoubleNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToDouble(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static decimal ToDecimal(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetDecimal(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static decimal? ToDecimalNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return Convert.ToDecimal(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }






    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetDateTime(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static DateTime? ToDateTimeNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return (DateTime)value;
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpan(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            return DataConvertUtils.ToTimeSpan(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static TimeSpan? ToTimeSpanNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return DataConvertUtils.ToTimeSpan(value);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Guid ToGuid(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            return reader.GetGuid(index);
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Guid? ToGuidNull(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        try {
            object value = reader.GetValue(index);
            if( value == DBNull.Value )
                return null;

            return (Guid)value;
        }
        catch( InvalidCastException ex ) {
            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
        }
        catch( Exception ex2 ) {
            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
        }
    }

//#if NET6_0_OR_GREATER
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="reader"></param>
//    /// <param name="index"></param>
//    /// <param name="entityType"></param>
//    /// <param name="propertyName"></param>
//    /// <returns></returns>
//    public static TimeOnly ToTimeOnly(DbDataReader reader, int index, Type entityType, string propertyName)
//    {
//        try {
//            object value = reader.GetValue(index);
//            return DataConvertUtils.ToTimeOnly(value);
//        }
//        catch( InvalidCastException ex ) {
//            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
//        }
//        catch( Exception ex2 ) {
//            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
//        }
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="reader"></param>
//    /// <param name="index"></param>
//    /// <param name="entityType"></param>
//    /// <param name="propertyName"></param>
//    /// <returns></returns>
//    public static TimeOnly? ToTimeOnlyNull(DbDataReader reader, int index, Type entityType, string propertyName)
//    {
//        try {
//            object value = reader.GetValue(index);
//            if( value == DBNull.Value )
//                return null;
//            else
//                return DataConvertUtils.ToTimeOnly(value);
//        }
//        catch( InvalidCastException ex ) {
//            throw new InvalidCastException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex);
//        }
//        catch( Exception ex2 ) {
//            throw new InvalidOperationException($"加载数据实体时，数据类型转换失败，当前实体类型 {entityType.Name}, 属性 {propertyName}", ex2);
//        }
//    }
//#endif


}
