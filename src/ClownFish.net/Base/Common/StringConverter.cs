namespace ClownFish.Base;

/// <summary>
/// 字符串转换器
/// </summary>
public static class StringConverter
{

    /// <summary>
    /// 将一个 key=value;key=value 格式的字符串转换成指定的类型对象
    /// </summary>
    /// <typeparam name="T">返回值的类型参数</typeparam>
    /// <param name="keyValues">key=value;key=value 格式的字符串</param>
    /// <returns></returns>
    public static T ToObject<T>(this string keyValues) where T : class, new()
    {
        if( keyValues.IsNullOrEmpty() )
            return null;


        // 检查是否为JSON格式
        if( keyValues.Length > 2 ) {
            if( keyValues[0] == '{' && keyValues[keyValues.Length - 1] == '}' )
                return keyValues.FromJson<T>();

            if( keyValues[0] == '[' && keyValues[keyValues.Length - 1] == ']' )
                return keyValues.FromJson<T>();
        }



        T result = new T();
        SetObjectValues(result, keyValues);

        return result;
    }

    private static readonly char[] s_itemSeparators = new char[] { ';', '\r', '\n' };

    /// <summary>
    /// 拆分一个 key=value;key=value 格式的字符串，给指定的对象属性赋值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">将要被赋值的对象</param>
    /// <param name="keyValues">key=value;key=value 格式的字符串</param>
    public static void SetObjectValues<T>(T obj, string keyValues) where T : class, new()
    {
        if( obj == null )
            throw new ArgumentNullException(nameof(obj));

        if( keyValues.IsNullOrEmpty() )
            return;


        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);


        // 拆分参数值
        List<NameValue> values = keyValues.ToKVList(s_itemSeparators, '=');

        foreach( NameValue nv in values ) {

            PropertyInfo p = properties.FirstOrDefault(x => x.Name.EqualsIgnoreCase(nv.Name));
            if( p != null ) {

                try {
                    object pValue = ChangeType(nv.Value, p.PropertyType.GetRealType());
                    p.FastSetValue(obj, pValue);
                }
                catch( Exception ex ) {
                    throw new InvalidCastException($"类型转换失败，目标类型：{p.PropertyType.Name}，当前属性名： {nv.Name}，当前字符串值：{nv.Value}", ex);
                }
            }
        }
    }


    /// <summary>
    /// 将字符串转换成指定的数据类型
    /// </summary>
    /// <param name="value"></param>
    /// <param name="conversionType"></param>
    /// <returns></returns>
    public static object ChangeType(string value, Type conversionType)
    {
        // 注意：这个方法应该与 IsSupportableType 保持一致性。
        // 如果 conversionType.IsSupportableType() 返回 true，这里应该能转换，除非字符串的格式不正确。

        if( conversionType == typeof(string) )
            return value;

        if( value == null || value.Length == 0 ) {
            // 空字符串根本不能做任何转换，所以直接返回null
            return null;
        }

        if( conversionType == typeof(bool) )
            return value == "1" || value.Is("true");

        if( conversionType.IsPrimitive || conversionType == typeof(DateTime) || conversionType == typeof(decimal) ) {
            // 为了简单，直接调用 .net framework中的方法。
            // 如果转换失败，将会抛出异常。
            return Convert.ChangeType(value, conversionType);
        }

        if( conversionType == typeof(Guid) )
            return new Guid(value);

        if( conversionType.IsEnum )
            //return Enum.Parse(conversionType, value);  // 这个方法不支持 枚举名称 的场景
            return ParseEnumValue(value, conversionType);

        if( conversionType == typeof(TimeSpan) )
            return ToTimeSpan(value);

        if( conversionType == typeof(byte[]) )
            return Convert.FromBase64String(value);


        // 尝试使用类型的隐式转换（从字符串转换）
        MethodInfo stringImplicit = GetStringImplicit(conversionType);
        if( stringImplicit != null )
            return stringImplicit.FastInvoke(null, value);


        throw new NotSupportedException("不支持字符串到目标类型的转换：" + conversionType.Name);
    }


    private static object ParseEnumValue(string value, Type conversionType)
    {
#if NETFRAMEWORK
        try {
            return Enum.Parse(conversionType, value);
        }
        catch {
            return ParseEnumName(value, conversionType);
        }
#else
        if( Enum.TryParse(conversionType, value, false, out object result) )
            return result;

        return ParseEnumName(value, conversionType);
#endif
    }


    internal static object ParseEnumName(string value, Type conversionType)
    {
        Array values = Enum.GetValues(conversionType);
        string[] names = Enum.GetNames(conversionType);

        for( int i = 0; i < names.Length; i++ ) {
            string name = names[i];
            if( name.Is(value) )
                return values.GetValue(i);
        }

        return Enum.Parse(conversionType, value);  // 抛异常
    }


    /// <summary>
    /// 判断指定的类型是否能从String类型做隐式类型转换，如果可以，则返回相应的方法
    /// </summary>
    /// <param name="conversionType"></param>
    /// <returns></returns>
    private static MethodInfo GetStringImplicit(Type conversionType)
    {
        MethodInfo m = conversionType.GetMethod("op_Implicit",
                                                BindingFlags.Static | BindingFlags.Public, null,
                                                new Type[] { typeof(string) }, null);

        if( m != null && m.IsSpecialName && m.ReturnType == conversionType )
            return m;
        else
            return null;
    }


    internal static TimeSpan ToTimeSpan(string text)
    {
        if( text.IsNullOrEmpty() )
            return TimeSpan.Zero;

        char end = text[text.Length - 1];

#if NETFRAMEWORK
        if( end == 's' ) {
            int second = int.Parse(text.Substring(0, text.Length - 1));
            return TimeSpan.FromSeconds(second);
        }

        if( end == 'f' ) {
            int milliseconds = int.Parse(text.Substring(0, text.Length - 1));
            return TimeSpan.FromMilliseconds(milliseconds);
        }
#else
        if( end == 's' ) {
            int second = int.Parse(text.AsSpan(0, text.Length - 1));
            return TimeSpan.FromSeconds(second);
        }

        if( end == 'f' ) {
            int milliseconds = int.Parse(text.AsSpan(0, text.Length - 1));
            return TimeSpan.FromMilliseconds(milliseconds);
        }
#endif

        if( text.IndexOf(':') > 0 )
            return TimeSpan.Parse(text);
        else
            return TimeSpan.FromTicks(long.Parse(text));
    }

}
