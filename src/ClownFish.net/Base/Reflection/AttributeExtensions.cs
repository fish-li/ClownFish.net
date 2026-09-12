namespace ClownFish.Base.Reflection;

/// <summary>
/// 扩展方法工具类
/// </summary>
public static class AttributeExtensions
{
    /// <summary>
    /// 获取类型成员的Attribute（单个定义）
    /// </summary>
    /// <typeparam name="T">要查找的修饰属性类型</typeparam>
    /// <param name="m">包含attribute的类成员对象</param>
    /// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetMyAttribute<T>(this MemberInfo m, bool inherit = false) where T : Attribute
    {
        return m.GetCustomAttribute<T>(inherit);
        //return AttributeCache.GetOne<T>(m, inherit);
    }


    // 由于其它项目在引用 这里的扩展方法，所以暂且保留几个在用的方法~~~~


    ///// <summary>
    ///// 获取类型成员的Attribute（多个定义）
    ///// </summary>
    ///// <typeparam name="T">要查找的修饰属性类型</typeparam>
    ///// <param name="m">包含attribute的类成员对象</param>
    ///// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static T[] GetMyAttributes<T>(this MemberInfo m, bool inherit = false) where T : Attribute
    //{
    //    return m.GetCustomAttributes<T>(inherit).ToArray();
    //    //return AttributeCache.GetArray<T>(m, inherit);
    //}


    ///// <summary>
    ///// 获取参数对象的Attribute（单个定义）
    ///// </summary>
    ///// <typeparam name="T">要查找的修饰属性类型</typeparam>
    ///// <param name="p">包含attribute的参数对象</param>
    ///// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static T GetMyAttribute<T>(this ParameterInfo p, bool inherit = false) where T : Attribute
    //{
    //    return p.GetCustomAttribute<T>(inherit);
    //    //return AttributeCache.GetOne<T>(p, inherit);
    //}



    ///// <summary>
    ///// 获取参数对象的Attribute（多个定义）
    ///// </summary>
    ///// <typeparam name="T">要查找的修饰属性类型</typeparam>
    ///// <param name="p">包含attribute的参数对象</param>
    ///// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static T[] GetMyAttributes<T>(this ParameterInfo p, bool inherit = false) where T : Attribute
    //{
    //    return p.GetCustomAttributes<T>(inherit).ToArray();
    //    //return AttributeCache.GetArray<T>(p, inherit);
    //}


    /// <summary>
    /// 获取类型对象的Attribute（单个定义）
    /// </summary>
    /// <typeparam name="T">要查找的修饰属性类型</typeparam>
    /// <param name="t">包含attribute的类型</param>
    /// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetMyAttribute<T>(this Type t, bool inherit = false) where T : Attribute
    {
        return t.GetCustomAttribute<T>(inherit);
        //return AttributeCache.GetOne<T>(t, inherit);
    }



    ///// <summary>
    ///// 获取类型对象的Attribute（多个定义）
    ///// </summary>
    ///// <typeparam name="T">要查找的修饰属性类型</typeparam>
    ///// <param name="t">包含attribute的类型</param>
    ///// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static T[] GetMyAttributes<T>(this Type t, bool inherit = false) where T : Attribute
    //{
    //    return t.GetCustomAttributes<T>(inherit).ToArray();
    //    //return AttributeCache.GetArray<T>(t, inherit);
    //}





}
