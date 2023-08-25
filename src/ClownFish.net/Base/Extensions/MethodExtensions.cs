namespace ClownFish.Base;

/// <summary>
/// 一些扩展方法
/// </summary>
public static class MethodExtensions
{

    /// <summary>
    /// 判断方法是不是有返回值
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static bool HasReturn(this MethodInfo m)
    {
        return m.ReturnType != typeof(void);
    }

    /// <summary>
    /// 判断是不是一个 Task 方法
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsTaskMethod(this MethodInfo method)
    {
        if( method.ReturnType == typeof(Task) )
            return true;

        if( method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) )
            return true;


        return false;
    }


    /// <summary>
    /// 检查是不是 Task&lt;T&gt; 方法，如果是，则返回类型参数T，否则返回 null
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Type GetTaskMethodResultType(this MethodInfo method)
    {
        Type type = method.ReturnType;

        if( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>) )
            return type.GetGenericArguments()[0];


        return null;
    }


#if NETCOREAPP

    /// <summary>
    /// 判断是不是一个 Task 方法
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsValueTaskMethod(this MethodInfo method)
    {
        if( method.ReturnType == typeof(ValueTask) )
            return true;

        if( method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>) )
            return true;


        return false;
    }


    /// <summary>
    /// 检查是不是 ValueTask&lt;T&gt; 方法，如果是，则返回类型参数T，否则返回 null
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Type GetValueTaskMethodResultType(this MethodInfo method)
    {
        Type type = method.ReturnType;

        if( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>) )
            return type.GetGenericArguments()[0];


        return null;
    }

#endif

}
