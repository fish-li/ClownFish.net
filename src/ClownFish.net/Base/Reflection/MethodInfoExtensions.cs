using ClownFish.Base.Reflection;

namespace ClownFish.Base;

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
            throw new ArgumentNullException("methodInfo");

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
        return method.ReflectedType.FullName + "/" + method.Name;
    }

}


