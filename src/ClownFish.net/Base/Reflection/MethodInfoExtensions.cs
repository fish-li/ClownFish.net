namespace ClownFish.Base.Reflection;

/// <summary>
/// MethodInfo 相关的扩展方法，用于性能优化。
/// </summary>
public static class MethodInfoExtensions
{
    /// <summary>
    /// 快速调用方法
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="instance"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
    {
        if( methodInfo == null )
            throw new ArgumentNullException(nameof(methodInfo));

        IInvokeMethod method = MethodInvokerFactory.GetMethodInvokerWrapper(methodInfo);
        return method.Invoke(instance, parameters);
    }


    /// <summary>
    /// 获取一个方法的全名。结果格式：方法所在的类型全名/方法名
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static string GetMethodFullName(this MethodInfo method)
    {
        if( method == null )
            throw new ArgumentNullException(nameof(method));

        return method.ReflectedType.FullName + "/" + method.Name;
    }


    /// <summary>
    /// 调用一个方法并记录到OprLog
    /// </summary>
    /// <param name="method"></param>
    /// <param name="instance"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object InvokeAndLog(this MethodInfo method, object instance, params object[] parameters)
    {
        if( method == null )
            throw new ArgumentNullException(nameof(method));

        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return method.Invoke(instance, parameters);



        string stepName = $"{method.DeclaringType.FullName}.{method.Name}";
        DateTime start = DateTime.Now;
        Exception lastError = null;
        try {
            return method.Invoke(instance, parameters);
        }
        catch( TargetInvocationException ex ) {
            lastError = ex.InnerException;
            throw ex.InnerException;
        }
        finally {
            scope.AddStep(start, stepName, "", DateTime.Now, lastError);
        }
    }

}


