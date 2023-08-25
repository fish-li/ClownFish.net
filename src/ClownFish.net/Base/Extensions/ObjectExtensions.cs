namespace ClownFish.Base;

/// <summary>
/// object 相关扩展方法工具类
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// 将一个数据对象转成另一个类型的数据对象，类似于 AutoMapper 的功能。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public static T ConvertTo<T>(this object input) where T : new()
    {
        // 说明：这个方法就是用于代替 AutoMapper 的使用。
        // 虽然 AutoMapper 功能强大，但是绝大多数的使用场景都非常简单，可参考下面的 CopyData 方法
        // 反而 AutoMapper 出现的异常非常难以理解，例如：

        // System.ArgumentException: GenericArguments[0], 'System.Char', on 'T MaxFloat[T](System.Collections.Generic.IEnumerable`1[T])' violates the constraint of type 'T'.
        //    --->System.Security.VerificationException: Method System.Linq.Enumerable.MaxFloat: type argument 'System.Char' violates the constraint of type parameter 'T'.

        // 在上面这个异常场景中，2个数据类型根本没有使用 char ，所以对于这样的异常完全看不懂，根本没办法解决，只能放弃！！！
        // 网上也有类似这种异常的反馈，https://github.com/AutoMapper/AutoMapper/issues/4066

        T result = new T();
        CopyData(input, result);
        return result;
    }


    /// <summary>
    /// 将source的所有pulic属性值复制到destination中，复制要求：属性名称相同，数据类型相同，属性可读可写
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    public static void CopyData(this object source, object destination)
    {
        if( source == null )
            throw new ArgumentNullException(nameof(source));
        if( destination == null )
            throw new ArgumentNullException(nameof(destination));

        PropertyInfo[] properties1 = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        PropertyInfo[] properties2 = destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach( PropertyInfo p1 in properties1 ) {
            if( p1.CanRead == false || p1.CanWrite == false )
                continue;

            PropertyInfo p2 = destination.GetType().GetProperty(p1.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if( p2 == null || p2.CanWrite == false || p2.CanRead == false )
                continue;

            if( p1.PropertyType != p2.PropertyType )
                continue;

            object value = p1.FastGetValue(source);
            p2.FastSetValue(destination, value);
        }
    }
}
