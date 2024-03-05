namespace ClownFish.Base.Reflection;

/// <summary>
/// FieldInfo 相关的扩展方法，用于性能优化。
/// </summary>
public static class FieldInfoExtensions
{
    private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
    private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));

    /// <summary>
    /// 快速读取字段值
    /// </summary>
    /// <param name="fieldInfo"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static object FastGetValue(this FieldInfo fieldInfo, object instance)
    {
        if( fieldInfo == null )
            throw new ArgumentNullException("fieldInfo");

        GetValueDelegate getter = (GetValueDelegate)s_getterDict[fieldInfo];
        if( getter == null ) {
            getter = DynamicMethodFactory.CreateFieldGetter(fieldInfo);
            s_getterDict[fieldInfo] = getter;
        }

        return getter(instance);
        //return fieldInfo.GetValue(instance);
    }

    /// <summary>
    /// 快速给字段赋值
    /// </summary>
    /// <param name="fieldInfo"></param>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public static void FastSetValue(this FieldInfo fieldInfo, object instance, object value)
    {
        if( fieldInfo == null )
            throw new ArgumentNullException("fieldInfo");

        SetValueDelegate setter = (SetValueDelegate)s_setterDict[fieldInfo];
        if( setter == null ) {
            setter = DynamicMethodFactory.CreateFieldSetter(fieldInfo);
            s_setterDict[fieldInfo] = setter;
        }

        setter(instance, value);
        //fieldInfo.SetValue(instance, value);
    }
}


