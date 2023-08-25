namespace ClownFish.Base;

/// <summary>
/// Type相关操作的工具类
/// </summary>
internal static class TypeHelper
{
    /// <summary>
    /// 根据类型名称获取类型对象，如果失败会明确指出调用的名称。
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="throwOnError"></param>
    /// <returns></returns>
    public static Type GetType(string typeName, bool throwOnError)
    {
        if( string.IsNullOrEmpty(typeName) )
            throw new ArgumentNullException(nameof(typeName));

        if( throwOnError == false )
            return Type.GetType(typeName, false);

        try {
            return Type.GetType(typeName, true);
        }
        catch( Exception ex ) {
            throw new ArgumentOutOfRangeException("获取类型失败：" + typeName, ex);
        }
    }


    /// <summary>
    /// 根据一个类型的全名称，获取其中的短名称，
    /// 例如："ClownFish.Log.Models.ExceptionInfo, Nebula.net"， 返回 "ExceptionInfo"
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
