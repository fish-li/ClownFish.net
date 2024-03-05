namespace ClownFish.Base.Reflection;

/// <summary>
/// Type相关操作的工具类
/// </summary>
public static class TypeHelper
{
    private static readonly TSafeDictionary<string, Type> s_dictionary = new TSafeDictionary<string, Type>(256, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        string configValues = ConfigFile.GetFile("ClownFish_Public_TypeAliasMaps.txt", false);
        InitFormText(configValues);
    }

    internal static void InitFormText(string configValues)
    {
        if( configValues.IsNullOrEmpty() )
            return;

        // 配置文件的格式：
        // # 注释行
        // alias = dest-type-name

        List<NameValue> list = (from line in configValues.ToLines()
                                where line.StartsWith0("#") == false
                                let nv = NameValue.Parse(line, '=')
                                where nv != null && nv.Name.HasValue() && nv.Value.HasValue()
                                select nv).ToList();

        foreach( var x in list ) {
            Type type = Type.GetType(x.Value, true);
            RegisterAlias(x.Name, type);
        }
    }
    

    /// <summary>
    /// 注册类型的别名
    /// </summary>
    /// <param name="typeAlias"></param>
    /// <param name="type"></param>
    public static void RegisterAlias(string typeAlias, Type type)
    {
        if( string.IsNullOrEmpty(typeAlias) )
            throw new ArgumentNullException(nameof(typeAlias));
        if( type == null )
            throw new ArgumentNullException(nameof(type));

        s_dictionary[typeAlias] = type;
    }

    /// <summary>
    /// 根据类型名称获取类型对象
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="throwOnError"></param>
    /// <returns></returns>
    public static Type GetType(string typeName, bool throwOnError)
    {
        if( string.IsNullOrEmpty(typeName) )
            throw new ArgumentNullException(nameof(typeName));

        Type type = s_dictionary.TryGet(typeName);

        return type ?? Type.GetType(typeName, throwOnError);
    }



    /// <summary>
    /// 根据一个类型的全名称，获取其中的短名称，
    /// 例如："ClownFish.Log.Models.ExceptionInfo, ClownFish.net"， 返回 "ExceptionInfo"
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    internal static string GetShortName(string typeName)
    {
        if( typeName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(typeName));

        string name = typeName;

        int p = name.IndexOf(',');
        if( p > 0 )
            name = name.Substring(0, p).TrimEnd(' ');

        int p2 = name.LastIndexOf('.');
        if( p2 > 0 )
            name = name.Substring(p2 + 1);

        return name;
    }
}
