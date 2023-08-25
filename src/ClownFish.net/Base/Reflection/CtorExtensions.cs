using ClownFish.Base.Reflection;

namespace ClownFish.Base;

/// <summary>
/// Type 相关的扩展方法，用于性能优化。
/// </summary>
public static class CtorExtensions
{
    private static readonly Hashtable s_methodDict = Hashtable.Synchronized(new Hashtable(10240));


    /// <summary>
    /// 快速实例化一个对象
    /// </summary>
    /// <param name="instanceType"></param>
    /// <returns></returns>
    public static object FastNew(this Type instanceType)
    {
        if( instanceType == null )
            throw new ArgumentNullException("instanceType");

        CtorDelegate ctor = (CtorDelegate)s_methodDict[instanceType];
        if( ctor == null ) {
            ConstructorInfo ctorInfo = instanceType.GetConstructor(Type.EmptyTypes);

            if( ctorInfo == null )
                throw new NotSupportedException(string.Format("类型\"{0}\"没有无参的构造方法。", instanceType.ToString()));

            ctor = DynamicMethodFactory.CreateConstructor(ctorInfo);
            s_methodDict[instanceType] = ctor;
        }

        return ctor();
        //return Activator.CreateInstance(instanceType);
    }

}


