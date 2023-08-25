namespace ClownFish.Data;

/// <summary>
/// 免得老是写typeof(xxx)，定义一个静态类型方便写代码
/// </summary>
internal static class TypeList
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DataFieldAttribute : Attribute
    {
        public string CsName { get; set; }
        public string Method { get; set; }
    }

#pragma warning disable IDE1006 // 命名样式

    // 说明：这个类型中的字段命名并不规范，之所以不规范是因为：
    // 1、大小写：主要是希望保持与C#代码中看到的类型名称一致！
    // 2、下划线：如果没有下划线，字段名与C#的类型关键字同名了。

    [DataField(CsName = "string", Method = "ToString")]
    public static readonly Type _string = typeof(string);

    [DataField(CsName = "bool", Method = "ToBool")]
    public static readonly Type _bool = typeof(bool);

    [DataField(CsName = "bool?", Method = "ToBoolNull")]
    public static readonly Type _bool_null = typeof(bool?);

    [DataField(CsName = "byte", Method = "ToByte")]
    public static readonly Type _byte = typeof(byte);

    [DataField(CsName = "byte?", Method = "ToByteNull")]
    public static readonly Type _byte_null = typeof(byte?);

    [DataField(CsName = "sbyte", Method = "ToSByte")]
    public static readonly Type _sbyte = typeof(sbyte);

    [DataField(CsName = "sbyte?", Method = "ToSByteNull")]
    public static readonly Type _sbyte_null = typeof(sbyte?);

    [DataField(CsName = "byte[]", Method = "ToByteArray")]
    public static readonly Type _byteArray = typeof(byte[]);

    [DataField(CsName = "char", Method = "ToChar")]
    public static readonly Type _char = typeof(char);

    [DataField(CsName = "char?", Method = "ToCharNull")]
    public static readonly Type _char_null = typeof(char?);

    [DataField(CsName = "short", Method = "ToShort")]
    public static readonly Type _short = typeof(short);

    [DataField(CsName = "short?", Method = "ToShortNull")]
    public static readonly Type _short_null = typeof(short?);

    [DataField(CsName = "ushort", Method = "ToUShort")]
    public static readonly Type _ushort = typeof(ushort);

    [DataField(CsName = "ushort?", Method = "ToUShortNull")]
    public static readonly Type _ushort_null = typeof(ushort?);

    [DataField(CsName = "int", Method = "ToInt")]
    public static readonly Type _int = typeof(int);

    [DataField(CsName = "int?", Method = "ToIntNull")]
    public static readonly Type _int_null = typeof(int?);

    [DataField(CsName = "uint", Method = "ToUInt")]
    public static readonly Type _uint = typeof(uint);

    [DataField(CsName = "uint?", Method = "ToUIntNull")]
    public static readonly Type _uint_null = typeof(uint?);

    [DataField(CsName = "long", Method = "ToLong")]
    public static readonly Type _long = typeof(long);

    [DataField(CsName = "long?", Method = "ToLongNull")]
    public static readonly Type _long_null = typeof(long?);

    [DataField(CsName = "ulong", Method = "ToULong")]
    public static readonly Type _ulong = typeof(ulong);

    [DataField(CsName = "ulong?", Method = "ToULongNull")]
    public static readonly Type _ulong_null = typeof(ulong?);

    [DataField(CsName = "float", Method = "ToFloat")]
    public static readonly Type _float = typeof(float);

    [DataField(CsName = "float?", Method = "ToFloatNull")]
    public static readonly Type _float_null = typeof(float?);

    [DataField(CsName = "double", Method = "ToDouble")]
    public static readonly Type _double = typeof(double);

    [DataField(CsName = "double?", Method = "ToDoubleNull")]
    public static readonly Type _double_null = typeof(double?);

    [DataField(CsName = "decimal", Method = "ToDecimal")]
    public static readonly Type _decimal = typeof(decimal);

    [DataField(CsName = "decimal?", Method = "ToDecimalNull")]
    public static readonly Type _decimal_null = typeof(decimal?);

    [DataField(CsName = "DateTime", Method = "ToDateTime")]
    public static readonly Type _DateTime = typeof(DateTime);

    [DataField(CsName = "DateTime?", Method = "ToDateTimeNull")]
    public static readonly Type _DateTime_null = typeof(DateTime?);

    [DataField(CsName = "TimeSpan", Method = "ToTimeSpan")]
    public static readonly Type _TimeSpan = typeof(TimeSpan);

    [DataField(CsName = "TimeSpan?", Method = "ToTimeSpanNull")]
    public static readonly Type _TimeSpan_null = typeof(TimeSpan?);

    [DataField(CsName = "Guid", Method = "ToGuid")]
    public static readonly Type _Guid = typeof(Guid);

    [DataField(CsName = "Guid?", Method = "ToGuidNull")]
    public static readonly Type _Guid_null = typeof(Guid?);

//#if NET6_0_OR_GREATER
//    [DataField(CsName = "TimeOnly", Method = "ToTimeOnly")]
//    public static readonly Type _XTime = typeof(TimeOnly);

//    [DataField(CsName = "TimeOnly?", Method = "ToTimeOnlyNull")]
//    public static readonly Type _XTime_null = typeof(TimeOnly?);
//#endif

    // ###################  注意 ###################

    //  下划线开头的字段，用于表示实体的属性数据类型
    // 例如： _int  ， _string

    // ###################  注意 ###################



    public static readonly Type Object = typeof(object);
    public static readonly Type Void = typeof(void);
    public static readonly Type Nullable = typeof(Nullable<>);

    public static readonly Type Entity = typeof(Entity);
    public static readonly Type IEntityProxy = typeof(IEntityProxy);


#pragma warning restore IDE1006 // 命名样式




    /// <summary>
    /// 类型Type 映射到 类型名称，例如： typeof(decimal) => _decimal
    /// </summary>
    private static readonly Dictionary<Type, DataFieldAttribute> s_typeDict;

    /// <summary>
    /// 类型名称 遇到 方法名称，例如：_decimal => ToDecimal
    /// </summary>
    private static readonly Dictionary<string, string> s_methodDict;

    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    static TypeList()
    {
        s_typeDict = new Dictionary<Type, DataFieldAttribute>();
        s_methodDict = new Dictionary<string, string>();

        var methods1 = typeof(DataReaderUtils).GetMethods(BindingFlags.Static | BindingFlags.Public);
        var methods2 = typeof(DataTableUtils).GetMethods(BindingFlags.Static | BindingFlags.Public);

        foreach( var x in typeof(TypeList).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) ) {

            DataFieldAttribute attr = x.GetCustomAttribute<DataFieldAttribute>();
            if( attr == null )
                continue;


            Type t = (Type)x.GetValue(null);
            s_typeDict.Add(t, attr);
            s_methodDict.Add(attr.CsName, attr.Method);

            MethodInfo method = methods1.FirstOrDefault(m => m.Name.Is(attr.Method) && m.ReturnType == t);
            if( method == null )
                throw new InvalidCodeException($"类型 DataReaderUtils 中不存在方法 {attr.Method}");

            method = methods2.FirstOrDefault(m => m.Name.Is(attr.Method) && m.ReturnType == t);
            if( method == null )
                throw new InvalidCodeException($"类型 DataTableUtils 中不存在方法 {attr.Method}");
        }
    }



    public static string GetTypeName(this Type t)
    {
        if( t == null )
            throw new ArgumentNullException(nameof(t));

        DataFieldAttribute attr = null;
        s_typeDict.TryGetValue(t, out attr);
        return attr?.CsName;
    }


    public static string GetMethodName(string typeName)
    {
        if( typeName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(typeName));

        return s_methodDict[typeName];
    }




}
