namespace ClownFish.Base.Reflection;

/// <summary>
/// 缓存反射读取到的Attribute对象
/// </summary>
public static class AttributeCache
{
    /// <summary>
    /// 缓存 单个Attribute
    /// </summary>
    private static readonly Hashtable s_table1 = Hashtable.Synchronized(new Hashtable(2048));
    /// <summary>
    /// 缓存 Attribute[]
    /// </summary>
    private static readonly Hashtable s_table2 = Hashtable.Synchronized(new Hashtable(512));


    // 说明： 组合 Register 和 GetOne，可实现在不改变代码的情况下实现动态Attribute注入效果。

    /// <summary>
    /// 为指定的对象注册Attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="attr"></param>
    public static void Register<T>(object obj, T attr) where T : Attribute
    {
        var key = new AttributeKey { Target = obj, AttributeType = typeof(T) };

        s_table1[key] = attr;
    }

    /// <summary>
    /// 为指定的对象注册Attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="attrs"></param>
    public static void Register<T>(object obj, T[] attrs) where T : Attribute
    {
        var key = new AttributeKey { Target = obj, AttributeType = typeof(T) };

        s_table2[key] = attrs;
    }


    /// <summary>
    /// 检索应用于指定对象的指定类型的自定义特性（返回单个Attribute）
    /// </summary>
    /// <typeparam name="T">类型参数，要求从Attribute继承</typeparam>
    /// <param name="obj">包含Attribute的对象</param>
    /// <param name="inherit">如果检查 element 的上级，则为 true；否则为 false。</param>
    /// <returns>与 T 相匹配的自定义属性；否则，如果没有找到这类属性，则为 null。</returns>
    public static T GetOne<T>(object obj, bool inherit = false) where T : Attribute
    {
        var key = new AttributeKey { Target = obj, AttributeType = typeof(T) };

        object result = s_table1[key];

        if( DBNull.Value.Equals(result) )
            return null;

        if( result == null ) {
            if( obj is MemberInfo )
                result = (obj as MemberInfo).GetCustomAttribute<T>(inherit);

            else if( obj is ParameterInfo )
                result = (obj as ParameterInfo).GetCustomAttribute<T>(inherit);

            else
                throw new NotSupportedException();

            if( result == null )
                s_table1[key] = DBNull.Value;
            else
                s_table1[key] = result;
        }

        return result as T;
    }


    /// <summary>
    /// 检索应用于指定对象的指定类型的自定义特性（返回Attribute数组）
    /// </summary>
    /// <typeparam name="T">类型参数，要求从Attribute继承</typeparam>
    /// <param name="obj">包含Attribute的对象</param>
    /// <param name="inherit">如果检查 element 的上级，则为 true；否则为 false。</param>
    /// <returns>与 T 相匹配的自定义属性；否则，如果没有找到这类属性，则为 null。</returns>
    public static T[] GetArray<T>(object obj, bool inherit = false) where T : Attribute
    {
        var key = new AttributeKey { Target = obj, AttributeType = typeof(T) };

        object result = s_table2[key];

        if( result == null ) {
            if( obj is MemberInfo )
                result = (obj as MemberInfo).GetCustomAttributes<T>(inherit);

            else if( obj is ParameterInfo )
                result = (obj as ParameterInfo).GetCustomAttributes<T>(inherit);

            else
                throw new NotSupportedException();

            // 就算是找不到指定的Attribute，GetCustomAttributes()会返回一个空数组，所以不需要引用空值判断
            s_table2[key] = result;
        }

        return result as T[];
    }

    private class AttributeKey
    {
        public object Target { get; set; }
        public Type AttributeType { get; set; }

        public override bool Equals(object obj)
        {
            if( obj == null )
                return false;

            AttributeKey key = (AttributeKey)obj;
            return key.Target == this.Target && key.AttributeType == this.AttributeType;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Target.GetHashCode();
            hash = (hash * 7) + AttributeType.GetHashCode();

            return hash;
        }
    }
}
