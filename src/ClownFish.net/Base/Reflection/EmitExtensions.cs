namespace ClownFish.Base.Reflection;

/// <summary>
/// 优化反射性能的工具类
/// </summary>
[RequiresUnreferencedCode("This class uses reflection, incompatible with trimming.")]
public static class EmitExtensions
{
    private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
    private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));
    private static readonly Hashtable s_methodDict = Hashtable.Synchronized(new Hashtable(10240));

    /// <summary>
    /// 用优化的方式快速读取PropertyInfo
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object FastGetValue2(this PropertyInfo propertyInfo, object obj)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException(nameof(propertyInfo));

        if( EnvArgs0.IsAot ) {
            return propertyInfo.GetValue(obj);
        }

        GetValueDelegate getter = (GetValueDelegate)s_getterDict[propertyInfo];
        if( getter == null ) {
            getter = DynamicMethodFactory.CreatePropertyGetter(propertyInfo);
            s_getterDict[propertyInfo] = getter;
        }

        return getter(obj);
    }

    /// <summary>
    /// 用优化的方式快速写PropertyInfo
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void FastSetValue2(this PropertyInfo propertyInfo, object obj, object value)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException(nameof(propertyInfo));

        if( EnvArgs0.IsAot ) {
            propertyInfo.SetValue(obj, value);
            return;
        }

        SetValueDelegate setter = (SetValueDelegate)s_setterDict[propertyInfo];
        if( setter == null ) {
            setter = DynamicMethodFactory.CreatePropertySetter(propertyInfo);
            s_setterDict[propertyInfo] = setter;
        }

        setter(obj, value);
    }


    /// <summary>
    /// 用优化的方式快速调用一个方法
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="obj"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object FastInvoke2(this MethodInfo methodInfo, object obj, params object[] parameters)
    {
        if( methodInfo == null )
            throw new ArgumentNullException(nameof(methodInfo));

        if( EnvArgs0.IsAot ) {
            return methodInfo.Invoke(obj, parameters);
        }

        MethodDelegate invoker = (MethodDelegate)s_methodDict[methodInfo];
        if( invoker == null ) {
            invoker = DynamicMethodFactory.CreateMethod(methodInfo);
            s_methodDict[methodInfo] = invoker;
        }

        return invoker(obj, parameters);
    }

}


