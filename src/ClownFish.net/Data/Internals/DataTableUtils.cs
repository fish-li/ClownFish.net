namespace ClownFish.Data.Internals;

/// <summary>
/// 用于数据加载相关的扩展工具类，仅供框架内部使用（不考虑升级兼容问题）！
/// </summary>
public static class DataTableUtils
{

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static object GetValue(this DataRow row, int index)
    {
        return row[index];
    }

    //private static object GetFieldValue(DataRow row, int index)
    //{
    //    object value = row[index];

    //    if( DBNull.Value.Equals(value) )
    //        value = null;

    //    return value;
    //}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static string ToString(DataRow row, int index, Type entityType, string propertyName)
    {
        object value = row[index];
        if( value == DBNull.Value )
            return null;
        else
            return value.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool ToBool(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];

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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool? ToBoolNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static byte ToByte(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static byte? ToByteNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static sbyte ToSByte(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static sbyte? ToSByteNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static char ToChar(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];

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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static char? ToCharNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static short ToShort(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static short? ToShortNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ushort ToUShort(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ushort? ToUShortNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static int ToInt(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static int? ToIntNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static uint ToUInt(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static uint? ToUIntNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static long ToLong(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static long? ToLongNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ulong ToULong(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static ulong? ToULongNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static float ToFloat(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static float? ToFloatNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static double ToDouble(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static double? ToDoubleNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static decimal ToDecimal(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static decimal? ToDecimalNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static DateTime? ToDateTimeNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpan(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static TimeSpan? ToTimeSpanNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Guid ToGuid(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
            return (Guid)value;
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
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="entityType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Guid? ToGuidNull(DataRow row, int index, Type entityType, string propertyName)
    {
        try {
            object value = row[index];
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
//    /// <param name="row"></param>
//    /// <param name="index"></param>
//    /// <param name="entityType"></param>
//    /// <param name="propertyName"></param>
//    /// <returns></returns>
//    public static TimeOnly ToTimeOnly(DataRow row, int index, Type entityType, string propertyName)
//    {
//        try {
//            object value = row[index];
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
//    /// <param name="row"></param>
//    /// <param name="index"></param>
//    /// <param name="entityType"></param>
//    /// <param name="propertyName"></param>
//    /// <returns></returns>
//    public static TimeOnly? ToTimeOnlyNull(DataRow row, int index, Type entityType, string propertyName)
//    {
//        try {
//            object value = row[index];
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
