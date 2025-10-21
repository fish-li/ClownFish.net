namespace ClownFish.Base.Reflection;

/// <summary>
/// 反射相关工具类
/// </summary>
public static class ReflectionUtils
{
    /// <summary>
    /// 用反射方式读取一个对象的 属性/字段 值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
#if NETCOREAPP
    [RequiresUnreferencedCode("This method uses reflection, incompatible with trimming.")]
#endif
    public static T Get<T>(this object data, string propName)
    {
        if( data == null)
            throw new ArgumentNullException(nameof(data));
        if( propName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(propName));

        Type type = data.GetType();
        PropertyInfo p = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
        if( p != null ) {
            return (T)p.FastGetValue(data);
        }

        FieldInfo f = type.GetField(propName, BindingFlags.Instance | BindingFlags.Public);
        if( f != null ) {
            return (T)f.GetValue(data);
        }

        throw new ArgumentOutOfRangeException(nameof(propName));
    }


    /// <summary>
    /// 用反射的方式查找一个类型，然后调用它的一个【静态无参方法】
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
#if NETCOREAPP
    [RequiresUnreferencedCode("This method uses reflection, incompatible with trimming.")]
#endif
    public static int CallStaticMethod(string typeFullName, string methodName)
    {
        if( typeFullName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(typeFullName));
        if( methodName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(methodName));

        Type type = Type.GetType(typeFullName, false, true);
        if( type == null )
            return -1;

        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if( method == null )
            return -2;

        method.Invoke(null, null);
        return 1;
    }


    /// <summary>
    /// 用反射的方式调用一个（同步/异常）方法
    /// </summary>
    /// <param name="instance">定义方法的类型实例，如果调用静态方法可以传递null</param>
    /// <param name="method">要调用的方法</param>
    /// <param name="args">调用方法所需要的参数</param>
    /// <returns></returns>
#if NETCOREAPP
    [RequiresUnreferencedCode("This method uses reflection, incompatible with trimming.")]
#endif
    public static async Task<object> CallMethod(object instance, MethodInfo method, object[] args)
    {
        if( method == null )
            throw new ArgumentNullException(nameof(method));

        object result = null;

        if( method.IsTaskMethod() ) {
            bool hasReturn = method.GetTaskMethodResultType() != null;
            if( hasReturn ) {
                Task task = (Task)method.FastInvoke(instance, args);
                await task;

                // 从 Task<T> 中获取返回值
                PropertyInfo property = task.GetType().GetProperty("Result", BindingFlags.Instance | BindingFlags.Public);
                result = property.FastGetValue(task);
            }
            else {
                await (Task)method.FastInvoke(instance, args);
            }
        }
        else {
            if( method.HasReturn() )
                result = method.FastInvoke(instance, args);
            else
                method.FastInvoke(instance, args);
        }

        return result;
    }
}

