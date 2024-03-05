namespace ClownFish.Base.Reflection;

/// <summary>
/// PropertyInfo 相关的扩展方法，用于性能优化。
/// </summary>
public static class PropertyInfoExtensions
{
    /// <summary>
    /// 快速读取属性值
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException("propertyInfo");

        return GetterSetterFactory.GetPropertyGetterWrapper(propertyInfo).Get(instance);
    }

    /// <summary>
    /// 快速给属性赋值
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException("propertyInfo");

        GetterSetterFactory.GetPropertySetterWrapper(propertyInfo).Set(instance, value);
    }
}


